using ProZapCommunication.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace ProZapCommunication.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly DispatcherTimer _uiRefreshTimer;
    private readonly List<IPlcProvider> _registeredProviders = new List<IPlcProvider>();

    public ObservableCollection<StationViewModelBase> Stations { get; } = new ObservableCollection<StationViewModelBase>();

    public MainViewModel()
    {
        _uiRefreshTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(250) };
        _uiRefreshTimer.Tick += OnUiRefreshTick;
        _uiRefreshTimer.Start();
    }

    public void RegisterPlcProvider(IPlcProvider provider)
    {
        _registeredProviders.Add(provider);
    }

    private void OnUiRefreshTick(object sender, EventArgs e)
    {

        foreach (var stationVm in Stations)
        {
            // Możesz przypisać stan globalny lub rozbudować klasę o przypisanie stacji do konkretnego providera
            stationVm.IsConnected = stationVm.GetScheduler().IsWatchdogOk; ;
            stationVm.RefreshUiFromBuffers();
        }
    }
}
