using ProZapCommunication.Core.Logic;
using ProZapCommunication.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProZapCommunication.UI.ViewModels;

public abstract class StationViewModelBase : ViewModelBase
{
    protected readonly StationScheduler Scheduler;
    protected readonly StationProcessorBase Processor;

    private DateTime _lastHistoryTimestamp = DateTime.MinValue;

    private string _stationName;
    private bool _isConnected;
    private string _connectionStatusText = "Disconnected";

    // Pola diagnostyczne UI
    private int _plcLife;
    private int _plcToPcTask;
    private int _plcToPcConfirm;
    private int _pcToPlcTask;
    private int _pcToPlcConfirm;
    private string _currentDmc;

    public int StationId { get; }
    public string StationName { get => _stationName; set => SetProperty(ref _stationName, value); }
    public bool IsConnected
    {
        get => _isConnected;
        set { if (SetProperty(ref _isConnected, value)) ConnectionStatusText = value ? "Connected (OK)" : "Communication Error"; }
    }
    public string ConnectionStatusText { get => _connectionStatusText; set => SetProperty(ref _connectionStatusText, value); }


    // Właściwości bindowane w XAML diagnostyki
    public int PlcLife { get => _plcLife; set => SetProperty(ref _plcLife, value); }
    public int PlcToPcTask { get => _plcToPcTask; set => SetProperty(ref _plcToPcTask, value); }
    public int PlcToPcConfirm { get => _plcToPcConfirm; set => SetProperty(ref _plcToPcConfirm, value); }
    public int PcToPlcTask { get => _pcToPlcTask; set => SetProperty(ref _pcToPlcTask, value); }
    public int PcToPlcConfirm { get => _pcToPlcConfirm; set => SetProperty(ref _pcToPlcConfirm, value); }
    public string CurrentDmc { get => _currentDmc; set => SetProperty(ref _currentDmc, value); }
    public ObservableCollection<TaskHistoryEntry> TaskHistory { get; } = new ObservableCollection<TaskHistoryEntry>();

    public ICommand SendTestTaskCommand { get; }

    public StationScheduler GetScheduler() => Scheduler;

    protected StationViewModelBase(int stationId, string name, StationScheduler scheduler, StationProcessorBase processor)
    {
        StationId = stationId;
        StationName = name;
        Scheduler = scheduler;
        Processor = processor;
    }

    // Uniwersalne masowe przepisywanie danych diagnostycznych wywoływane przez timer UI
    public void RefreshBaseDiagnostics()
    {
        var read = Processor.GetReadState();
        var write = Processor.GetWriteState();

        if (write != null)
        {
            PlcToPcConfirm = write.TaskConfirmToPLC;
            PcToPlcTask = write.TaskSendToPLC;
        }

        if (read != null)
        {
            PlcLife = read.Life;
            PlcToPcTask = read.TaskSendToPc;
            PcToPlcConfirm = read.TaskConfirmToPc;
            CurrentDmc = read.DMC; // Przekazanie kodu DMC (SQL) do widoku
        }

        // Aktualizacja listy historycznej w UI
        var historySnapshot = Processor.GetHistorySnapshot();

        if (historySnapshot != null && historySnapshot.Count > 0)
        {
            // Sprawdzamy, czy najnowszy element ma inny czas niż ten, który już przetworzyliśmy.
            // Jeśli czas się zgadza i liczba elementów w UI jest taka sama, to znaczy, że nic się nie zmieniło.
            if (historySnapshot[0].Timestamp == _lastHistoryTimestamp && TaskHistory.Count == historySnapshot.Count)
            {
                return; // Kończymy natychmiast – brak nowych zadań, oszczędzamy CPU i UI!
            }

            // Zapamiętujemy znacznik czasu najnowszego zadania
            _lastHistoryTimestamp = historySnapshot[0].Timestamp;

            // Synchronizujemy kolekcję bez czyszczenia (Clear):
            // Przechodzimy od końca pobranej migawki (od najstarszych do najnowszych z tych, które doszły)
            for (int i = historySnapshot.Count - 1; i >= 0; i--)
            {
                var entry = historySnapshot[i];

                // Sprawdzamy, czy ten konkretny wpis (po unikalnym czasie) już istnieje w naszym UI
                bool alreadyExists = false;
                for (int j = 0; j < TaskHistory.Count; j++)
                {
                    if (TaskHistory[j].Timestamp == entry.Timestamp)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                // Jeśli wpisu nie ma w UI, wstawiamy go na sam początek (Insert 0)
                if (!alreadyExists)
                {
                    TaskHistory.Insert(0, entry);
                }
            }

            // Jeśli w UI uzbierało się więcej niż 6 elementów (bo doszły nowe), usuwamy nadmiar z końca
            while (TaskHistory.Count > 6)
            {
                TaskHistory.RemoveAt(TaskHistory.Count - 1);
            }
        }
        else if (TaskHistory.Count > 0)
        {
            // Czyścimy tylko wtedy, gdy historia w procesorze rzeczywiście stała się pusta
            TaskHistory.Clear();
            _lastHistoryTimestamp = DateTime.MinValue;
        }
    }

    // METODA ABSTRAKCYJNA: Każdy specyficzny ViewModel sam zdecyduje, 
    // jak bezpiecznie przepisać swoje unikalne dane z bufora tła do pól UI
    public abstract void RefreshUiFromBuffers();
}
