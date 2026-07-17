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
    public short Reference_Number = 0;

    public short Fill1 = 0;

    public short Grommet_Left_Bottom_Presence_Cam_1_Status = 0;
    public short Grommet_Left_Bottom_Presence_Cam_1_Counter = 0;
    public short Grommet_Right_Bottom_Presence_Cam_2_Status = 0;
    public short Grommet_Right_Bottom_Presence_Cam_2_Counter = 0;

    public short Radiator_Left_Bottom_Grommet_Cam_1_Status = 0;
    public short Radiator_Left_Bottom_Grommet_Cam_1_Counter = 0;
    public short Radiator_Right_Bottom_Grommet_Cam_2_Status = 0;
    public short Radiator_Right_Bottom_Grommet_Cam_2_Counter = 0;

    public short Stonegrid_Bottom_Left_Bracket_Cam_3_Status = 0;
    public short Stonegrid_Bottom_Left_Bracket_Cam_3_Counter = 0;
    public short Stonegrid_Top_Left_Slider_Cam_4_Status = 0;
    public short Stonegrid_Top_Left_Slider_Cam_4_Counter = 0;
    public short Stonegrid_Bottom_Right_Bracket_Cam_5_Status = 0;
    public short Stonegrid_Bottom_Right_Bracket_Cam_5_Counter = 0;
    public short Stonegrid_Top_Right_Slider_Cam_6_Status = 0;
    public short Stonegrid_Top_Right_Slider_Cam_6_Counter = 0;

    public short Airduct_Left_Middle_Slider_Cam_4_Status = 0;
    public short Airduct_Left_Middle_Slider_Cam_4_Counter = 0;
    public short Airduct_Right_Middle_Slider_Cam_6_Status = 0;
    public short Airduct_Right_Middle_Slider_Cam_6_Counter = 0;
    public short Airduct_Left_Top_Bracket_Cam_7_Status = 0;
    public short Airduct_Left_Top_Bracket_Cam_7_Counter = 0;
    public short Airduct_Right_Top_Bracket_Cam_8_Status = 0;
    public short Airduct_Right_Top_Bracket_Cam_8_Counter = 0;
    public short Airduct_Left_Top_Balcony_Cam_9_Status = 0;
    public short Airduct_Left_Top_Balcony_Cam_9_Counter = 0;
    public short Airduct_Right_Top_Balcony_Cam_10_Status = 0;
    public short Airduct_Right_Top_Balcony_Cam_10_Counter = 0;
    public short Airduct_Middle_Bottom_Ridge_Cam_11_Status = 0;
    public short Airduct_Middle_Bottom_Ridge_Cam_11_Counter = 0;
}
