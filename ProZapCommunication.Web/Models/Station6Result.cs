namespace ProZapCommunication.Web.Models;

public class Station6Result : StationResultMfsBase
{




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
