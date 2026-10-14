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
    protected string _visionDestPath;
    private bool _isVisionConfigured = false;
    private int _lastLoggedCamRequest = 0;
    private string? _lastLoggedCamDmc;

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



    private string GetStationPrefix()
    {
        if (StationId >= 1 && StationId <= 4)
            return $"ST{StationId}_";
        if (StationId >= 5 && StationId <= 7)
            return $"MFS{StationId - 4}_";

        return $"ST{StationId}_"; // Fallback bezpieczeństwa
    }

    private async Task VisionSystemLoopAsync()
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        string stationPrefix = GetStationPrefix();

        while (await timer.WaitForNextTickAsync())
        {
            if (CurrentReadState == null) continue;

            int requestMask = CurrentReadState.CameraRequest;

            if (requestMask == 0)
            {
                _lastLoggedCamRequest = 0;
                _lastLoggedCamDmc = "";

                if (CurrentWriteState.CameraFound != 0 || CurrentWriteState.CameraMoved != 0)
                {
                    CurrentWriteState.CameraFound = 0;
                    CurrentWriteState.CameraMoved = 0;
                    Log.Debug("S{Id} [VISION]: PLC reset CameraRequest. Resetting PC flags.", StationId);
                }
                continue;
            }

            string currentDmc = CurrentReadState.DMCCamera;
            if (string.IsNullOrWhiteSpace(currentDmc))
            {
                Log.Warning("S{Id} [VISION]: PLC did not provide a DMC code.", StationId);
                continue;
            }

            if (_lastLoggedCamRequest != requestMask || _lastLoggedCamDmc != currentDmc)
            {
                Log.Information(
                    "S{Id} [VISION]: New request {RequestMask} for DMC {Dmc}.", StationId, requestMask, currentDmc);

                _lastLoggedCamRequest = requestMask;
                _lastLoggedCamDmc = currentDmc;
            }

            try
            {
                // Używamy EnumerateDirectories dla mniejszej alokacji pamięci w pętli
                var directories = Directory.EnumerateDirectories(_visionSourcePath);

                foreach (string dirPath in directories)
                {
                    string dirName = Path.GetFileName(dirPath);

                    // Filtrowanie: Przetwarzamy tylko katalogi należące do tej stacji
                    if (!dirName.StartsWith(stationPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    ProcessDeviceDirectory(dirPath, dirName, requestMask, currentDmc);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "S{Id} [VISION]: Error reading source directory.", StationId);
            }
        }
    }

    private void ProcessDeviceDirectory(string dirPath, string dirName, int requestMask, string dmc)
    {
        // Definicja dozwolonych rozszerzeń na sztywno wewnątrz funkcji (wykorzystanie C# 12+ collection expressions)
        string[] allowedExtensions = [".bmp", ".svg"];

        // Szukamy numeru urządzenia w nazwie folderu (np. "CAM1" lub "DMC1")
        Match match = Regex.Match(dirName, @"(?:CAM|DMC)(\d+)", RegexOptions.IgnoreCase);
        if (!match.Success || !int.TryParse(match.Groups[1].Value, out int deviceId)) return;

        if (deviceId < 1 || deviceId > 32) return;

        int cameraBit = 1 << (deviceId - 1);

        if ((requestMask & cameraBit) != 0)
        {
            var fileEnumerable = Directory.EnumerateFiles(dirPath)
                .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

            // Zabezpieczenie: Sprawdzamy czy w katalogu fizycznie znajduje się choć jeden pliko dozwolonym rozszerzeniu
            if (!fileEnumerable.Any())
            {
                // Katalog nie zawiera pożądanych plików – kamera/czytnik jeszcze ich nie zapisały
                // Nie podnosimy flagi CameraFound ani nie wykonujemy przenoszenia.
                return;
            }

            // Potwierdzamy znalezienie zdjęć danej kamery (ustawiamy bit w CameraFound)
            CurrentWriteState.CameraFound |= cameraBit;

            // Tworzymy strukturę docelową: Rok/Miesiąc/Dzień/DMC/Kamera
            DateTime now = DateTime.Now;
            string targetDir = Path.Combine(_visionDestPath, now.Year.ToString(), now.Month.ToString("D2"), now.Day.ToString("D2"), dmc, dirName);

            // Nazwa folderu docelowego jest identyczna z folderem źródłowym
            Directory.CreateDirectory(targetDir);

            // Próbujemy przenieść wszystkie pliki z katalogu źródłowego do docelowego, przekazując tablicę z dozwolonymi rozszerzeniami
            bool allFilesMovedSuccessfully = MoveFilesInDirectory(dirPath, targetDir, dmc, allowedExtensions);

            if (allFilesMovedSuccessfully)
            {
                // Zgłaszamy sukces do PLC (ustawiamy bit w CameraMoved)
                CurrentWriteState.CameraMoved |= cameraBit;
            }
        }
    }

    private bool MoveFilesInDirectory(string sourceDir, string targetDir, string dmc, string[] allowedExtensions)
    {
        bool allSuccess = true;
        bool anyFilesProcessed = false;

        // Filtrujemy pliki zgodnie z przekazanymi rozszerzeniami przed rozpoczęciem iteracji
        var files = Directory.EnumerateFiles(sourceDir)
            .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

        foreach (string filePath in files)
        {
            anyFilesProcessed = true;
            string fileName = Path.GetFileName(filePath);

            // Doklejanie DMC do nazwy pliku, zapobiegając wielokrotnemu doklejeniu w razie błędów
            // string newFileName = fileName.StartsWith(dmc, StringComparison.OrdinalIgnoreCase) ? fileName : $"{dmc}_{fileName}";
            string targetFilePath = Path.Combine(targetDir, fileName);

            bool moved = TryMoveFileWithRetry(filePath, targetFilePath, maxRetries: 20, delayMs: 100);

            if (moved)
            {
                Log.Debug("S{Id} [VISION]: File {File} moved successfully.", StationId, fileName);
            }
            else
            {
                Log.Warning("S{Id} [VISION]: File {File} locked by another process. Skipped this cycle.", StationId, fileName);
                allSuccess = false; // Błąd przeniesienia choć jednego pliku blokuje flagę CameraMoved dla PLC
            }
        }

        // Jeśli folder był pusty, PLC nie powinno otrzymać sygnału wykonania, dopóki kamera nie wrzuci plików
        return allSuccess && anyFilesProcessed;
    }

    private bool TryMoveFileWithRetry(string source, string dest, int maxRetries, int delayMs)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Używamy Move (wytnie z temp, wklei do docelowego)
                // W przypadku duplikatu nazwy pliku nadpisze go (true)
                File.Move(source, dest, overwrite: true);

                if (File.Exists(dest) && !File.Exists(source))
                    return true;

                Log.Warning("S{Id} File move verification failed. Attempt {Attempt}/{MaxRetries}",
                    StationId, i + 1, maxRetries);

                Thread.Sleep(delayMs);
            }
            catch (IOException ex) // Błąd braku dostępu do pliku (File in use)
            {
                Log.Debug(ex, "S{Id} File move retry {Attempt}/{MaxRetries}", StationId, i + 1, maxRetries);
                Thread.Sleep(delayMs);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "S{Id} Unexpected error while moving file.", StationId);
                return false; // Inny błąd (np. brak uprawnień) przerywa próbę
            }
        }
        return false;
    }
}