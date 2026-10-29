namespace ProZapCommunication.Web.Models;

public class Station8Result : StationResultBase
{
    public string Radiator_SN { get; set; } = string.Empty;
    public int Radiator_Status { get; set; } = 0;
    public string Mfs_SN { get; set; } = string.Empty;
    public int Mfs_Status { get; set; } = 0;
    public string Motor_SN { get; set; } = string.Empty; 
    public int Motor_Status { get; set; } = 0;

    public int ReworkCode { get; set; } = 0;
    public string Operator_ID { get; set; } = string.Empty;

    // ====================================================================
    // WŁAŚCIWOŚCI OBLICZENIOWE DLA MUDDATAGRID (Ignorowane przez Dapper)
    // ====================================================================

    public string SNStatusText => SN_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string RadiatorStatusText => Radiator_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string MotorStatusText => Motor_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string MfsStatusText => Mfs_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };
}
