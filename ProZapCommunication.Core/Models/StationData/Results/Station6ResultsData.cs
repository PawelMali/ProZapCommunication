using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

public class Station6ResultsData : IStationProcessData
{
    public string Motor_SN { get; set; }
    public int Motor_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; }
    public int Reference_Number { get; set; } = 0;

    public double Initial_static_unbalance { get; set; }
    public double Final_static_unbalance { get; set; }
    public double Runout { get; set; }
    public double Initial_moment_unbalance { get; set; }
    public double Final_moment_unbalance { get; set; }
    public double Chamber { get; set; }
    public double Initial_static_unbalance_angle { get; set; }
    public double Final_static_unbalance_angle { get; set; }
    public double Rpm { get; set; }
    public double Current { get; set; }
}
