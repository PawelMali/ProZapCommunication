using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;


/////////////////////////////////////////////////////////////////////
//  MFS 2 - STATION 6 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

[StructLayout(LayoutKind.Sequential)]
public class LogixStation6ResultsData
{
    public LogixString Motor_Sn = new LogixString();
    public short Motor_Status = 0;
    public short CycleTime = 0;
    public short Reference_Number = 0;

    public float Initial_static_unbalance = 0;
    public float Final_static_unbalance = 0;
    public float Runout = 0;
    public float Initial_moment_unbalance = 0;
    public float Final_moment_unbalance = 0;
    public float Chamber = 0;
    public float Initial_static_unbalance_angle = 0;
    public float Final_static_unbalance_angle = 0;
    public float Rpm = 0;
    public float Current = 0;

}
