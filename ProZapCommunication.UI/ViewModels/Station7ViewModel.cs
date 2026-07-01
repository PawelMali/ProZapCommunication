using ProZapCommunication.Core.Logic;
using ProZapCommunication.Core.Models.StationData.Results;

namespace ProZapCommunication.UI.ViewModels;

public class Station7ViewModel : StationViewModelBase
{
    private Station7ResultsData _backgroundProcessBuffer;

    public Station7ViewModel(int stationId, string name, StationScheduler scheduler, StationProcessorBase processor)
        : base(stationId, name, scheduler, processor) { }

    // Metoda wywoływana przez wątek komunikacyjny (w tle) – tylko odkłada referencję do obiektu
    public void UpdateBackgroundBuffer(Station7ResultsData freshData)
    {
        _backgroundProcessBuffer = freshData;
    }

    // Metoda wywoływana cyklicznie przez Timer UI (co 250ms) – bezpiecznie i masowo odświeża interfejs
    public override void RefreshUiFromBuffers()
    {
        base.RefreshBaseDiagnostics();

        if (_backgroundProcessBuffer == null) return;

        // Przepisujemy wartości – SetProperty zadba o to, by UI odświeżyło się TYLKO przy realnej zmianie wartości

    }
}
