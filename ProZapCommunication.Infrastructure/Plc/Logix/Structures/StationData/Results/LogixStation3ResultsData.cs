using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation3ResultsData
{
    public LogixString DMC = new LogixString();
    public short ResultCode = 0;
    public short CycleTime = 0;
}
