using ProZapCommunication.Core.Models.StationData.Results;

namespace ProZapCommunication.Core.Interfaces;

public interface IDatabaseRepository
{
    int SaveProductionStation1Data(string plcIdentity, int stationId, Station1ResultsData data);
    int SaveProductionStation2Data(string plcIdentity, int stationId, Station2ResultsData data);
    int SaveProductionStation3Data(string plcIdentity, int stationId, Station3ResultsData data);
    int SaveProductionStation4Data(string plcIdentity, int stationId, Station4ResultsData data);
    int SaveProductionStation5Data(string plcIdentity, int stationId, Station5ResultsData data);
    int SaveProductionStation6Data(string plcIdentity, int stationId, Station6ResultsData data);
    int SaveProductionStation7Data(string plcIdentity, int stationId, Station7ResultsData data);






    public int CheckSnCurrentStationStatus(string stationIdentity, int stationId, string sn);
    public int CheckSnPreviousStationStatus(string stationIdentity, int stationId, string sn);

    public int CheckMotorSnCurrentStationStatus(string stationIdentity, int stationId, string sn);
    public int CheckMotorSnPreviousStationStatus(string stationIdentity, int stationId, string sn);

    public int CheckMfsSnCurrentStationStatus(string stationIdentity, int stationId, string sn);
    public int CheckMfsSnPreviousStationStatus(string stationIdentity, int stationId, string sn);

    public int CheckRadiatorSnStatus(string stationIdentity, int stationId, string sn);


    int ProcessRadiatorRework(string radiatorSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity);
    int ProcessMotorRework(string motorSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity);
    int ProcessMfsRework(string mfsSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity);
    int ProcessMainSnRework(string mainSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity);

}
