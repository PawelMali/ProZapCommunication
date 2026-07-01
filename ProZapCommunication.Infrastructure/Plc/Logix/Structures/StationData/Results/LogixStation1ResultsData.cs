using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation1ResultsData
{
    public LogixString SN = new LogixString();
    public short SN_Status = 0;
    public LogixString Radiator_SN = new LogixString();
    public short Radiator_Status = 0;
    public short CycleTime = 0;
}
