namespace ProZapCommunication.Web.Models;

public class Station6Result : StationResultMfsBase
{
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
}
