using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixScrewData
{
    public bool Status_OK;
    public bool Status_NOK;
    public bool Reserve_02;
    public bool Reserve_03;
    public bool Reserve_04;
    public bool Reserve_05;
    public bool Reserve_06;
    public bool Reserve_07;

    public int Counter;
    public int Program_Number;
    public int Reserve_DINT_2;
    public int Reserve_DINT_3;

    public float Angle;
    public float Torque;
    public float Reserve_REAL_0;
    public float Reserve_REAL_1;
    public float Reserve_REAL_2;
    public float Reserve_REAL_3;

}
