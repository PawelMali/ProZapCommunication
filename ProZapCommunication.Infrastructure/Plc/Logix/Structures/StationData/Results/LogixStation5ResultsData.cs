using AutomatedSolutions.ASCommStd.SI.S7.Data;
using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;


/////////////////////////////////////////////////////////////////////
//  MFS 1 - STATION 5 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

[StructLayout(LayoutKind.Sequential)]
public class LogixStation5ResultsData
{
    public LogixString Motor_Sn = new LogixString();
    public short Motor_Status = 0;
    public short CycleTime = 0;
    public short Reference_Number = 0;

    public short Fill1 = 0;

    public short Motor_Inspection_Status = 0;
    public short Motor_Inspection_Counter = 0;
    public short Fan_Inspection_Status = 0;
    public short Fan_Inspection_Counter = 0;

    public LogixScrewData[] Screw = InitializeArray<LogixScrewData>(3);


    public static T[] InitializeArray<T>(Int32 length) where T : new()
    {
        T[] array = new T[length];
        for (Int32 i = 0; i < length; i++)
        {
            array[i] = new T();
        }
        return array;
    }
}
