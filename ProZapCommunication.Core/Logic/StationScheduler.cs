using ProZapCommunication.Core.Exceptions;
using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using Serilog;

namespace ProZapCommunication.Core.Logic;

public class StationScheduler
{
    private readonly int _stationId;
    private readonly string _stationIdentity;
    private readonly IPlcProvider _plc;
    private CancellationTokenSource _cts;

    // Zmienne do obsługi Watchdoga (Sygnału Życia)
    private int _lastLifeValue = -1;
    private DateTime _lastLifeChangeTime = DateTime.Now;
    private readonly TimeSpan _watchdogTimeout = TimeSpan.FromSeconds(5); // Czas braku odpowiedzi, np. 5s
    private bool _isWatchdogOk = false;
    private bool _wasWatchdogErrorLogged = false;

    // Flaga dostępna dla ViewModelu UI oraz procesora stacji
    public bool IsWatchdogOk => _isWatchdogOk;


    // Dynamiczne listy zadań odczytu dla każdego z 4 stopni komunikacji
    public List<PlcJob> Level1FastJobs { get; } = new List<PlcJob>();
    public List<PlcJob> Level2MediumJobs { get; } = new List<PlcJob>();
    public List<PlcJob> Level3SlowJobs { get; } = new List<PlcJob>();
    public List<PlcJob> Level4CustomJobs { get; } = new List<PlcJob>();

    public StationScheduler(int stationId, IPlcProvider plc)
    {
        _stationId = stationId;
      //  _stationIdentity = stationIdentity;
        _plc = plc;
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();

        // Uruchomienie 4 niezależnych pętli czasowych (stopni)
        // Jeśli lista zadań dla danego poziomu jest pusta, pętla natychmiast się zakończy, nie obciążając systemu
        Task.Run(() => RunLevelLoop(Level1FastJobs,100, _cts.Token));    // Stopień 1: Najszybszy (np. 100ms)
        Task.Run(() => RunLevelLoop(Level2MediumJobs, 1000, _cts.Token)); // Stopień 2: Średni (1000ms)
        Task.Run(() => RunLevelLoop(Level3SlowJobs, 2000, _cts.Token));   // Stopień 3: Wolny (2000ms)
        Task.Run(() => RunLevelLoop(Level4CustomJobs, 100, _cts.Token));  // Stopień 4: Indywidualny/Procesowy (100ms)

        // Niezależny wątek nadzorcy Watchdoga (sprawdza stan co 500ms)
        Task.Run(() => MonitorWatchdogLoop(_cts.Token));
    }

    public void Stop()
    {
        _cts?.Cancel();
        Log.Information("S{Id}: Stopped communication scheduler.", _stationId);
    }

    /// <summary>
    /// Uniwersalna pętla dla danego stopnia odświeżania, wykonująca sekwencyjny odczyt wszystkich przypisanych struktur.
    /// </summary>
    private async Task RunLevelLoop(List<PlcJob> jobs, int intervalMs, CancellationToken token)
    {
        if (jobs == null || jobs.Count == 0) return;

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));

        while (await timer.WaitForNextTickAsync(token) && !token.IsCancellationRequested)
        {
            // Wykonujemy odczyt wszystkich struktur zarejestrowanych na tym poziomie
            foreach (var job in jobs)
            {
                if (token.IsCancellationRequested) break;

                // !!! OCHRONA PRZED BŁĘDAMI KONFIGURACJI !!!
                // Jeśli zadanie zostało zdezaktywowane z powodu braku taga, pomijamy je całkowicie
                if (!job.IsActive) continue;

                try
                {
                    // Wykonanie operacji (np. odczytu lub zapisu zdefiniowanego w Bootstrapperze)
                    job.Execute();
                }
                // REAKCJA NA KRYTYCZNY BŁĄD ROZMIARU STRUKTURY (np. CIP 0x00000013)
                catch (PlcStructureMismatchException sizeEx)
                {
                    job.IsActive = false;
                    Log.Fatal("S{Id}: [BLOCK] Job '{JobName}' disabled due to STRUCTURE SIZE MISMATCH! Message: {Msg}",
                        _stationId, job.Name, sizeEx.Message);
                }
                // REAKCJA NA KRYTYCZNY BŁĄD BRAKU TAGA W PLC (np. CIP 0x00000104)
                catch (PlcMissingTagException missingEx)
                {
                    job.IsActive = false;
                    Log.Fatal("S{Id}: [BLOCK] Job '{JobName}' disabled – TAG DOES NOT EXIST IN PLC! Message: {Msg}",
                        _stationId, job.Name, missingEx.Message);
                }
                // Standardowy, przemijający błąd sieciowy (np. rozłączenie kabla, timeout)
                catch (Exception ex)
                {
                    if (_plc.IsConnected && _isWatchdogOk)
                    {
                        Log.Warning("S{Id}: [WARNING] Job '{JobName}' execution error ({Interval}ms): {Msg}",
                            _stationId, job.Name, intervalMs, ex.Message);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Niezależna pętla nadzorująca, czy sygnał życia nie zamarzł
    /// </summary>
    private async Task MonitorWatchdogLoop(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

        while (await timer.WaitForNextTickAsync(token) && !token.IsCancellationRequested)
        {
            // Jeśli fizyczny link sieciowy leży, to watchdog automatycznie też leży
            if (!_plc.IsConnected)
            {
                _isWatchdogOk = false;
                continue;
            }

            // Sprawdzamy, ile czasu minęło od ostatniej zmiany wartości Life
            if (DateTime.Now - _lastLifeChangeTime > _watchdogTimeout)
            {
                _isWatchdogOk = false;

                if (!_wasWatchdogErrorLogged)
                {
                    Log.Error("S{Id}: [WATCHDOG] The LIFE signal has been frozen at {Value} for more than {Timeout}s! The PLC may be in STOP mode",
                        _stationId, _lastLifeValue, _watchdogTimeout.TotalSeconds);

                    _wasWatchdogErrorLogged = true;
                }
            }
        }
    }

    /// <summary>
    /// Funkcja wywoływana przez Bootstrapper w szybkiej pętli po KAŻDYM udanym odczycie Handshake.
    /// </summary>
    public void FeedWatchdog(int currentLifeValue)
    {
        if (currentLifeValue != _lastLifeValue)
        {
            _lastLifeValue = currentLifeValue;
            _lastLifeChangeTime = DateTime.Now; // Resetujemy licznik czasu, bo sygnał żyje!
            _isWatchdogOk = true;

            if (_wasWatchdogErrorLogged)
            {
                Log.Information("S{Id}: [WATCHDOG] The LIFE signal is changing again ({Value}).", _stationId, currentLifeValue);
                _wasWatchdogErrorLogged = false;
            }
        }
    }

    public void ExecuteOnDemand(Action action)
    {
        Task.Run(() =>
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "S{Id}: Error occurred while executing user-requested operation.", _stationId);
            }
        });
    }
}