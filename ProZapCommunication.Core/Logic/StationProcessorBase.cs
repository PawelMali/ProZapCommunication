using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Write;
using Serilog;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace ProZapCommunication.Core.Logic;

public abstract class StationProcessorBase
{
    protected readonly int StationId;
    protected readonly string StationIdentity;

    protected readonly IDatabaseRepository DbRepository;
    protected readonly IPlcProvider Plc;

    protected ILogger Log => Serilog.Log.ForContext("StationIdentity", StationIdentity);

    // Kolejka FIFO dla zadań generowanych przez PC do wykonania na PLC
    private readonly ConcurrentQueue<int> _pcToPlcTaskQueue = new ConcurrentQueue<int>();

    // Lokalny bufor aktualnego stanu odczytanego z PLC (Struktura READ)
    protected StationReadBaseModel CurrentReadState;
    // Lokalny bufor struktury, którą wysyłamy do PLC (Struktura WRITE)
    protected StationWriteBaseModel CurrentWriteState;

    // Flaga chroniąca przed burzą wywołań funkcji biznesowych (zbocze narastające)
    private bool _isTaskProcessing = false;

    private string _visionSourcePath;
    private string _visionDestPath;
    private bool _isVisionConfigured = false;

    private DateTime? _taskSentAt;
    private readonly TimeSpan _taskTimeout = TimeSpan.FromSeconds(5);


    private readonly List<TaskHistoryEntry> _taskHistory = new List<TaskHistoryEntry>();
    private readonly object _historyLock = new object();


    protected StationProcessorBase(int stationId, IDatabaseRepository dbRepository, IPlcProvider plc, string stationIdentity, StationWriteBaseModel initialWriteState)
    {
        StationId = stationId;
        DbRepository = dbRepository;
        Plc = plc;
        StationIdentity = stationIdentity;

        CurrentWriteState = initialWriteState;

        //Log = Serilog.Log.ForContext("StationIdentity", stationIdentity);
    }

    public StationReadBaseModel GetReadState() => CurrentReadState;
    public StationWriteBaseModel GetWriteState() => CurrentWriteState;
    public List<TaskHistoryEntry> GetHistorySnapshot()
    {
        lock (_historyLock) return _taskHistory.ToList();
    }

    private void AddHistory(string direction, int code, string status, bool isOk)
    {
        short shortCode = (short)code;
        lock (_historyLock)
        {
            var description = Enum.IsDefined(typeof(PlcTaskType), shortCode) ? ((PlcTaskType)shortCode).ToString() : "Unknown task";

            string detailedStatus = status;

            if (!isOk)
                detailedStatus = Enum.IsDefined(typeof(PlcTaskStatus), (short)CurrentWriteState.TaskStatus) ? ((PlcTaskStatus)(short)CurrentWriteState.TaskStatus).ToString() : "Unknown error";


            _taskHistory.Insert(0, new TaskHistoryEntry
            {
                Direction = direction,
                Code = code,
                Description = description,
                Status = detailedStatus,
                IsOk = isOk
            });

            if (_taskHistory.Count > 6) _taskHistory.RemoveAt(6);
        }
    }

    /// <summary>
    /// Metoda wywoływana cyklicznie przez Scheduler (poprzez warstwę mapującą), 
    /// dostarczająca świeże dane ze sterownika PLC.
    /// </summary>
    public void UpdateHandshakeData(StationReadBaseModel freshReadData)
    {
        CurrentReadState = freshReadData;

        // Dla sygnalizacji pracy aplikacji, ustawiamy pole "Life" na aktualną sekundę, aby pokazać, że dane są aktualizowane.
        CurrentWriteState.Life = DateTime.Now.Second; 

        // Uruchomienie obsługi komunikatów w obu kierunkach
        ProcessPlcToPcHandshake();
        ProcessPcToPlcHandshake();
    }

    /// <summary>
    /// Pozwala zewnętrznym komponentom (np. kliknięcie przycisku w WPF) 
    /// na dodanie zadania do wykonania przez sterownik PLC.
    /// </summary>
    public void EnqueueTaskForPlc(int taskCode)
    {
        _pcToPlcTaskQueue.Enqueue(taskCode);
        Log.Information("S{Id} [TASK {TaskCode}]: Added task to FIFO queue for PLC.", StationId, taskCode);
    }

    // --- MASZYNA STANÓW: Zlecenia od PLC dla PC (Odczyt z PLC) ---
    private void ProcessPlcToPcHandshake()
    {
        // KROK 1: Wykrycie nowego zadania z PLC
        // Warunek: PLC wystawiło zadanie (>0), PLC jeszcze nie widzi potwierdzenia (TaskConfirmToPc == 0)
        // ORAZ dodatek: PC jeszcze nie zaczął ani nie skończył przetwarzać tego zadania (_isTaskProcessing == false)
        if (CurrentReadState.TaskSendToPc > 0 && CurrentWriteState.TaskConfirmToPLC == 0 && !_isTaskProcessing)
        {
            Log.Information("S{Id} [TASK {TaskCode}]: New task. Running logic...", StationId, CurrentReadState.TaskSendToPc);
            AddHistory("PLC -> PC", CurrentReadState.TaskSendToPc, "Processing...", true);

            // Natychmiast podnosimy flagę, aby w kolejnym cyklu za 50ms pomiąć ten blok!
            _isTaskProcessing = true;
            CurrentWriteState.TaskCounter++;
            

            // Wywołanie unikalnej logiki stacji (wykona się DOKŁADNIE RAZ)
            bool isTaskExecuted = ExecuteTask(CurrentReadState.TaskSendToPc);

            if (isTaskExecuted)
            {
                // Zapisujemy potwierdzenie w pamięci RAM struktury WRITE
                CurrentWriteState.TaskConfirmToPLC = CurrentReadState.TaskSendToPc;
                CurrentWriteState.TaskSuccessCounter++;

                Log.Information("S{Id} [TASK {TaskCode}]: Task executed successfully. Confirmation status sent.", StationId, CurrentReadState.TaskSendToPc);
                AddHistory("PLC -> PC", CurrentReadState.TaskSendToPc, $"OK", true);
            }
            else
            {
                // Jeśli zadanie się nie udało, również musimy odpowiedzieć do PLC (odpowiedni kod błędu ustawia ExecuteTask)
                // ale wystawiamy potwierdzenie, żeby zamknąć krok w PLC i nie blokować linii
                CurrentWriteState.TaskConfirmToPLC = CurrentReadState.TaskSendToPc;
                Log.Warning("S{Id} [TASK {TaskCode}]: Task failed. ", StationId, CurrentReadState.TaskSendToPc);
                AddHistory("PLC -> PC", CurrentReadState.TaskSendToPc, $"ERROR", false);
            }
        }

        // KROK 2: PLC zauważyło nasze potwierdzenie (TaskConfirmToPc == TaskConfirmFrom_PC) i wyzerowało rejestr zadania (TaskSendToPc == 0)
        // Wtedy PC czyści swoje potwierdzenie w RAM i opuszcza flagę blokady, przygotowując się na kolejny nowy Task
        else if (CurrentReadState.TaskSendToPc == 0 && CurrentWriteState.TaskConfirmToPLC > 0)
        {
            if (CurrentWriteState.TaskConfirmToPLC != 0)
            {
                Log.Debug("S{Id}: The PLC cleared the request. The PC clears the confirm register.", StationId);

                CurrentWriteState.TaskConfirmToPLC = 0;
                CurrentWriteState.TaskStatus = 0;

                // Resetujemy flagę – jesteśmy gotowi na przyjęcie kolejnego, nowego zadania!
                _isTaskProcessing = false;
            }
        }
    }

    // --- MASZYNA STANÓW: Zlecenia od PC dla PLC (Wpis do PLC za pomocą FIFO) ---
    private void ProcessPcToPlcHandshake()
    {
        // Timeout check(ważne - na początku!)
        if (CurrentWriteState.TaskSendToPLC > 0 && _taskSentAt.HasValue)
        {
            var elapsed = DateTime.UtcNow - _taskSentAt.Value;

            if (elapsed > _taskTimeout)
            {
                Log.Error("S{Id} [TASK {TaskCode}]: TIMEOUT after {Elapsed} ms!",
                    StationId,
                    CurrentWriteState.TaskSendToPLC,
                    elapsed.TotalMilliseconds);
                AddHistory("PC -> PLC", CurrentWriteState.TaskSendToPLC, "TIMEOUT", false);

                // reset handshake
                CurrentWriteState.TaskSendToPLC = 0;
                _taskSentAt = null;

                return;
            }
        }


        // Sprawdzamy czy linia komunikacyjna jest wolna (nasze zadanie wyzerowane i brak aktywnego potwierdzenia z PLC)
        if (CurrentWriteState.TaskSendToPLC == 0 && CurrentReadState.TaskConfirmToPc == 0)
        {
            // Jeśli linia jest czysta i mamy coś w kolejce FIFO
            if (_pcToPlcTaskQueue.TryPeek(out int nextTask))
            { 
                CurrentWriteState.TaskSendToPLC = nextTask;
                CurrentWriteState.TaskCounter++; // Zwiększamy licznik wysłanych zadań PC
                _taskSentAt = DateTime.UtcNow;
                Log.Information("S{Id} [TASK {TaskCode}]: Fetched job from FIFO. Sending to PLC...", StationId, nextTask);
                AddHistory("PC -> PLC", nextTask, "Sent request", true);

            }
        }
        // Jeśli PLC odebrało zadanie i odpowiedziało tym samym statusem (Potwierdzenie Handshake)
        else if (CurrentWriteState.TaskSendToPLC > 0 && CurrentReadState.TaskConfirmToPc == CurrentWriteState.TaskSendToPLC)
        {
            Log.Information("S{Id} [TASK {TaskCode}]: PLC confirmed the execution of the PC task. Removing from FIFO.",
                StationId, CurrentWriteState.TaskSendToPLC);
            AddHistory("PC -> PLC", CurrentWriteState.TaskSendToPLC, "Completed OK", true);

            // Usuwamy trwale element z początku kolejki FIFO
            _pcToPlcTaskQueue.TryDequeue(out _);

            // Rozpoczynamy procedurę zerowania – czyścimy żądanie zadania
            CurrentWriteState.TaskSendToPLC = 0;
        }
    }

    /// <summary>
    /// Abstrakcyjna metoda implementowana przez unikalne klasy stacji. 
    /// Zwraca true, jeśli zadanie wykonano pomyślnie i można odpowiedzieć do PLC.
    /// </summary>
    protected abstract bool ExecuteTask(int taskCode);











    /// <summary>
    /// Wywoływane z Bootstrappera, jeśli stacja ma skonfigurowane ścieżki w JSON.
    /// </summary>
    public void ConfigureVisionSystem(string sourcePath, string destPath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(destPath)) return;

        _visionSourcePath = sourcePath;
        _visionDestPath = destPath;
        _isVisionConfigured = true;

        // Tworzymy katalog źródłowy, jeśli nie istnieje (zabezpieczenie przed błędem Directory.GetFiles)
        Directory.CreateDirectory(_visionSourcePath);

        Log.Information("S{Id} [VISION]: Vision system configured. Source: {Src}, Destination: {Dest}", StationId, _visionSourcePath, _visionDestPath);

        // Uruchamiamy niezależny wątek skanujący w tle (nigdy nie zablokuje to Handshake'a!)
        Task.Run(VisionSystemLoopAsync);
    }

    /// <summary>
    /// Niezależna pętla działająca w tle, odpowiedzialna wyłącznie za system wizyjny.
    /// </summary>
    private async Task VisionSystemLoopAsync()
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500)); // Próbkujemy folder co 100ms

        while (await timer.WaitForNextTickAsync())
        {
            // Jeśli sieć jeszcze nie odebrała pierwszej ramki, CurrentReadState to null.
            // Czekamy na kolejny tik timera (za 100ms), nie rzucając błędem.
            if (CurrentReadState == null)
            {
                continue;
            }

            // 1. Sprawdzamy czy PLC o cokolwiek prosi
            int requestMask = CurrentReadState.CameraRequest;

            if (requestMask == 0)
            {
                // Jeśli PLC wyzerowało żądanie, my też zerujemy nasze odpowiedzi
                if (CurrentWriteState.CameraFound != 0 || CurrentWriteState.CameraMoved != 0)
                {
                    CurrentWriteState.CameraFound = 0;
                    CurrentWriteState.CameraMoved = 0;
                    Log.Debug("S{Id} [VISION]: PLC reset CameraRequest. Resetting PC flags.", StationId);
                }
                continue; // Czekamy na kolejne żądanie
            }

            string currentDmc = CurrentReadState.DMCCamera;
            if (string.IsNullOrWhiteSpace(currentDmc))
            {
                Log.Warning("S{Id} [VISION]: PLC did not provide a DMC code.", StationId);
                continue;
            }

            // 2. Przeszukujemy folder źródłowy
            try
            {
                string[] files = Directory.GetFiles(_visionSourcePath);

                foreach (string file in files)
                {
                    ProcessVisionFile(file, requestMask, currentDmc);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "S{Id} [VISION]: Error reading source directory.", StationId);
            }
        }
    }

    private void ProcessVisionFile(string filePath, int requestMask, string dmc)
    {
        string fileName = Path.GetFileName(filePath);

        // Szukamy w nazwie pliku wzorca "Cam01", "Cam12" itd. (Omijamy sztywne "ST1_", żeby klasa była uniwersalna)
        //Match match = Regex.Match(fileName, @"Cam(\d+)", RegexOptions.IgnoreCase);
        Match match = Regex.Match(fileName, @"(?:Cam|DMC)(\d+)", RegexOptions.IgnoreCase);
        if (!match.Success) return; // To nie jest plik z kamery

        // Parsujemy numer kamery (np. "01" -> 1)
        if (!int.TryParse(match.Groups[1].Value, out int cameraId)) return;

        // Sprawdzamy czy numer kamery mieści się w zakresie 1-32
        if (cameraId < 1 || cameraId > 32) return;

        // Tworzymy bit reprezentujący tę kamerę (Kamera 1 = bit 0, Kamera 2 = bit 1)
        int cameraBit = 1 << (cameraId - 1);

        // Sprawdzamy używając koniunkcji bitowej, czy PLC prosiło o tę kamerę
        if ((requestMask & cameraBit) != 0)
        {
            // KROK A: ZGŁASZAMY ZNALEZIENIE (Ustawiamy bit operatorem OR)
            CurrentWriteState.CameraFound |= cameraBit;

            // KROK B: TWORZYMY STRUKTURĘ DOCELOWĄ (Rok/Miesiąc/Dzień/DMC)
            DateTime now = DateTime.Now;
            string targetDir = Path.Combine(_visionDestPath, now.Year.ToString(), now.Month.ToString("D2"), now.Day.ToString("D2"), dmc, GetStationCameraName(fileName));
            Directory.CreateDirectory(targetDir);

            // KROK C: PRZYGOTOWANIE NOWEJ NAZWY PLIKU
            // Sprawdzamy, czy plik już nie ma przedrostka DMC (na wypadek dziwnych zapętleń)
            string newFileName = fileName.StartsWith(dmc) ? fileName : $"{dmc}_{fileName}";
            string targetFilePath = Path.Combine(targetDir, newFileName);

            // KROK D: PRÓBA PRZENIESIENIA Z BLOKADĄ (FILE LOCK RETRY)
            bool moved = TryMoveFileWithRetry(filePath, targetFilePath, maxRetries: 10, delayMs: 50);

            if (moved)
            {
                // ZGŁASZAMY SUKCES DO PLC
                CurrentWriteState.CameraMoved |= cameraBit;
                Log.Debug("S{Id} [VISION]: Camera {CamId} -> File moved successfully.", StationId, cameraId);
            }
            else
            {
                Log.Warning("S{Id} [VISION]: Camera {CamId} -> File locked by another process (camera still writing?). Skipped this cycle.", StationId, cameraId);
            }
        }
    }

    /// <summary>
    /// Algorytm ponowień (Retry Pattern). Jeśli kamera przemysłowa wciąż zapisuje plik, 
    /// dostaniemy wyjątek IOException. Czekamy 50ms i próbujemy ponownie (max 500ms).
    /// </summary>
    private bool TryMoveFileWithRetry(string source, string dest, int maxRetries, int delayMs)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Używamy Move (wytnie z temp, wklei do docelowego)
                // W przypadku duplikatu nazwy pliku nadpisze go (true)
                File.Move(source, dest, overwrite: true);
                return true;
            }
            catch (IOException) // Błąd braku dostępu do pliku (File in use)
            {
                Thread.Sleep(delayMs);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "S{Id} [VISION]: Unexpected error while moving file.", StationId);
                return false; // Inny błąd (np. brak uprawnień) przerywa próbę
            }
        }
        return false;
    }

    public static string GetStationCameraName(string fileName)
    {
        string name = Path.GetFileNameWithoutExtension(fileName);

        string st = "X";
        string deviceType = "CAM";
        string deviceNo = "0X";

        var stMatch = Regex.Match(name, @"St(\d+)", RegexOptions.IgnoreCase);
        if (stMatch.Success)
        {
            st = stMatch.Groups[1].Value;
        }

        var deviceMatch = Regex.Match(name, @"(CAM|DMC)(\d+)", RegexOptions.IgnoreCase);
        if (deviceMatch.Success)
        {
            deviceType = deviceMatch.Groups[1].Value;
            deviceNo = int.Parse(deviceMatch.Groups[2].Value).ToString("00");
        }

        return $"St{st}_{deviceType}{deviceNo}";
    }



}