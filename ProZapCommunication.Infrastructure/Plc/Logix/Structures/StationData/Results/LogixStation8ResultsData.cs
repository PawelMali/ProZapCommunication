using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation8ResultsData
{
    public LogixString Reserve1_Sn = new LogixString();
    public short Reserve1_Status = 0;
    public LogixString Reserve2_Sn = new LogixString();
    public short Reserve2_Status = 0;
    public short CycleTime = 0;
}
