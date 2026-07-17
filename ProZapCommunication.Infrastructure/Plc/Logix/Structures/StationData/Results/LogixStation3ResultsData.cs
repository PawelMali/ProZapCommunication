using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation3ResultsData
{
    public LogixString DMC = new LogixString();
    public short ResultCode = 0;
    public short CycleTime = 0;
    public short Reference_Number = 0;

    public short Fill1 = 0;

    public short LeakTest_Status = 0;
    public short LeakTest_Counter = 0;
    public byte LeakTest_Method = 0;
    public short LeakTest_Program_Number = 0;
    public long LeakTest_Pressure = 0;
    public long LeakTest_Pressure_Drop = 0;
    public long LeakTest_Leak_Rate = 0;
    public int LeakTest_Temperature = 0;
}
