using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation2ResultsData
{
    public LogixString SN = new LogixString();
    public short SN_Status = 0;
    public LogixString MFS_SN = new LogixString();
    public short MFS_Status = 0;
    public short CycleTime = 0;
}
