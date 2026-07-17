using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

/////////////////////////////////////////////////////////////////////
//  CRFM 3 - STATION 3 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

public class Station3ResultsData : IStationProcessData
{
    public string SN { get; set; }
    public int SN_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; } = 0;
    public int Reference_Number { get; set; } = 0;

    public int LeakTest_Status { get; set; } = 0;
    public int LeakTest_Counter { get; set; } = 0;
    public short LeakTest_Method { get; set; } = 0;
    public int LeakTest_Program_Number { get; set; } = 0;
    public long LeakTest_Pressure { get; set; } = 0;
    public long LeakTest_Pressure_Drop { get; set; } = 0;
    public long LeakTest_Leak_Rate { get; set; } = 0;
    public int LeakTest_Temperature { get; set; } = 0;

}
