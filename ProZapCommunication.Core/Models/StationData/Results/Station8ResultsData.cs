using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

public class Station8ResultsData : IStationProcessData
{
    public string Operator_ID { get; set; }
    public int Reserve1_Status { get; set; } = 0;
    public string Reserve2_SN { get; set; }
    public int Reserve2_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
}
