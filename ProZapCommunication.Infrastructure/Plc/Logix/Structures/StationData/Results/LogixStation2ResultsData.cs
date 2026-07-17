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
    public short Reference_Number = 0;

    public short Fill1 = 0;

    public short MFS_Left_Bottom_Balcony_Cam_1_Status = 0;
    public short MFS_Left_Bottom_Balcony_Cam_1_Counter = 0;
    public short MFS_Left_Middle_Guide_Cam_2_Status = 0;
    public short MFS_Left_Middle_Guide_Cam_2_Counter = 0;
    public short MFS_Left_Top_Bracket_Cam_3_Status = 0;
    public short MFS_Left_Top_Bracket_Cam_3_Counter = 0;
    public short MFS_Middle_Top_Ridge_Cam_4_Status = 0;
    public short MFS_Middle_Top_Ridge_Cam_4_Counter = 0;
    public short MFS_Right_Top_Bracket_Cam_5_Status = 0;
    public short MFS_Right_Top_Bracket_Cam_5_Counter = 0;
    public short MFS_Right_Middle_Guide_Cam_6_Status = 0;
    public short MFS_Right_Middle_Guide_Cam_6_Counter = 0;
    public short MFS_Right_Bottom_Balcony_Cam_7_Status = 0;
    public short MFS_Right_Bottom_Balcony_Cam_7_Counter = 0;

    public short Radiator_Left_Top_Grommet_Cam_8_Status = 0;
    public short Radiator_Left_Top_Grommet_Cam_8_Counter = 0;
    public short Radiator_Right_Top_Grommet_Cam_9_Status = 0;
    public short Radiator_Right_Top_Grommet_Cam_9_Counter = 0;

    public short Shipping_Caps_Disassembly_Status = 0;
    public short Shipping_Caps_Disassembly_Counter = 0;


}
