using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

public class Station7ResultsData : IStationProcessData
{
    public string Motor_SN { get; set; }
    public int Motor_Status { get; set; } = 0;
    public string Mfs_SN { get; set; }
    public int Mfs_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; }
}
