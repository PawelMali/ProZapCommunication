namespace ProZapCommunication.Web.Models;

public class Station5Result : StationResultMfsBase
{
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

    public string Screw_Batch_Nr { get; set; } = string.Empty;
    public string Fan_Batch_Nr { get; set; } = string.Empty;


    // ====================================================================
    // WŁAŚCIWOŚCI OBLICZENIOWE DLA MUDDATAGRID (Ignorowane przez Dapper)
    // ====================================================================

    public string MotorStatusText => Motor_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string MotorInspectionStatusText => Motor_Inspection_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string FanInspectionStatusText => Fan_Inspection_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Screw1StatusText => Screw1_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Screw2StatusText => Screw2_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Screw3StatusText => Screw3_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
}
