namespace ProZapCommunication.Web.Models;

public class Station3Result : StationResultBase
{
    public int? LeakTest_Status { get; set; } = 0;
    public int? LeakTest_Counter { get; set; } = 0;
    public short? LeakTest_Method { get; set; } = 0;
    public int? LeakTest_Program_Number { get; set; } = 0;
    public long? LeakTest_Pressure { get; set; } = 0;
    public long? LeakTest_Pressure_Drop { get; set; } = 0;
    public long? LeakTest_Leak_Rate { get; set; } = 0;
    public int? LeakTest_Temperature { get; set; } = 0;

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

    public string LeakTest_MethodText => LeakTest_Method switch
    {
        1 => "Pressure difference",
        2 => "Leakrate",
        null => "UNKNOWN",
        _ => "UNKNOWN"
    };

    public string LeakTestText => LeakTest_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
}
