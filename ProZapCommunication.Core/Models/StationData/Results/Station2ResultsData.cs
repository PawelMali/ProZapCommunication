using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

public class Station2ResultsData : IStationProcessData
{
    public string SN { get; set; }
    public int SN_Status { get; set; } = 0;
    public string MFS_SN { get; set; }
    public int MFS_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; }
}
