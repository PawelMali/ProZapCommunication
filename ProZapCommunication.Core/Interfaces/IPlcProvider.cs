using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;

namespace ProZapCommunication.Core.Interfaces;

public interface IPlcProvider
{
    void Disconnect();
    bool IsConnected { get; }

    // Zamiast generycznych objectów, używamy konkretnych modeli biznesowych
    StationReadBaseModel ReadBase(string tagName);
    Station8ReadBaseModel ReadStation8Base(string tagName);
    void WriteBase(string tagName, StationWriteBaseModel data);

    void WriteStation7Base(string tagName, Station7WriteBaseModel data);
    void WriteStation8Base(string tagName, Station8WriteBaseModel data);

    Station1ResultsData ReadStation1ResultsData(string tagName);
    Station2ResultsData ReadStation2ResultsData(string tagName);
    Station3ResultsData ReadStation3ResultsData(string tagName);
    Station4ResultsData ReadStation4ResultsData(string tagName);
    Station5ResultsData ReadStation5ResultsData(string tagName);
    Station6ResultsData ReadStation6ResultsData(string tagName);
    Station7ResultsData ReadStation7ResultsData(string tagName);
    Station8ResultsData ReadStation8ResultsData(string tagName);


    // Miejsce na kolejne metody dla innych stacji (np. testera szczelności)
    // LeakTestData ReadLeakData(string tagName);
}

