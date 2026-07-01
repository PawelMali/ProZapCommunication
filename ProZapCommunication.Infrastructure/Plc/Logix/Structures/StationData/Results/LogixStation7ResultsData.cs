using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation7ResultsData
{
    public LogixString Motor_Sn = new LogixString();
    public short Motor_Status = 0;
    public LogixString Mfs_Sn = new LogixString();
    public short Mfs_Status = 0;
    public short CycleTime = 0;
}
