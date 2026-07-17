using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Write;

[StructLayout(LayoutKind.Sequential)]
public class LogixStationWriteBase
{
    public Int16 Life;
    public UInt16 Task_Counter;
    public Int16 Task_Recived_From_PC;
    public Int16 Task_Confirm_From_PC;
    public UInt16 Task_Succes_Counter;
    public Int16 Task_Status;
    public Int32 Camera_Found;
    public Int32 Camera_Moved;
    public Int16 AdditionalInt1;
    public Int16 AdditionalInt2;
    public Int16 AdditionalInt3;
    public Int16 AdditionalInt4;
    public Int16 AdditionalInt5;
    public Int16 AdditionalInt6;
    public Int16 AdditionalInt7;

}
