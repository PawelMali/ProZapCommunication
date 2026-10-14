using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

/////////////////////////////////////////////////////////////////////
//  MFS 1 - STATION 5 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

public class Station5ResultsData : IStationProcessData
{
    public string Motor_SN { get; set; } = String.Empty;
    public int Motor_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; } = DateTime.MinValue;
    public int CycleTime { get; set; } = 0;
    public int Reference_Number { get; set; } = 0;

    public int Motor_Inspection_Status { get; set; } = 0;
    public int Motor_Inspection_Counter { get; set; } = 0;
    public int Fan_Inspection_Status { get; set; } = 0;
    public int Fan_Inspection_Counter { get; set; } = 0;

    public int Screw1_Status { get; set; } = 0;
    public int Screw1_Counter { get; set; } = 0;
    public int Screw1_Program_Number { get; set; } = 0;
    public float Screw1_Angle { get; set; } = 0;
    public float Screw1_Torque { get; set; } = 0;

    public int Screw2_Status { get; set; } = 0;
    public int Screw2_Counter { get; set; } = 0;
    public int Screw2_Program_Number { get; set; } = 0;
    public float Screw2_Angle { get; set; } = 0;
    public float Screw2_Torque { get; set; } = 0;

    public int Screw3_Status { get; set; } = 0;
    public int Screw3_Counter { get; set; } = 0;
    public int Screw3_Program_Number { get; set; } = 0;
    public float Screw3_Angle { get; set; } = 0;
    public float Screw3_Torque { get; set; } = 0;

    public string Screw_Batch_Nr { get; set; } = String.Empty;
    public string Fan_Batch_Nr { get; set; } = String.Empty;
}
