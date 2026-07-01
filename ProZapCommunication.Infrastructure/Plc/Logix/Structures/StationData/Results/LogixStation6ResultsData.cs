using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation6ResultsData
{
    public LogixString Motor_Sn = new LogixString();
    public short Motor_Status = 0;
    public short CycleTime = 0;
}
