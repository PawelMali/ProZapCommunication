using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

public class Station1ResultsData : IStationProcessData
{
    public string SN { get; set; } 
    public int SN_Status { get; set; } = 0;
    public string Radiator_SN { get; set; }
    public int Radiator_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; }
}
