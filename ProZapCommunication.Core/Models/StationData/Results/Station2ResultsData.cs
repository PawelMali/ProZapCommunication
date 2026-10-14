using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

/////////////////////////////////////////////////////////////////////
//  CRFM 2 - STATION 2 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

public class Station2ResultsData : IStationProcessData
{
    public string SN { get; set; }
    public int SN_Status { get; set; } = 0;
    public string MFS_SN { get; set; }
    public int MFS_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; }
    public int Reference_Number { get; set; } = 0;

    public int MFS_Left_Bottom_Balcony_Cam_1_Status { get; set; } = 0;
    public int MFS_Left_Bottom_Balcony_Cam_1_Counter { get; set; } = 0;
    public int MFS_Left_Middle_Guide_Cam_2_Status { get; set; } = 0;
    public int MFS_Left_Middle_Guide_Cam_2_Counter { get; set; } = 0;
    public int MFS_Left_Top_Bracket_Cam_3_Status { get; set; } = 0;
    public int MFS_Left_Top_Bracket_Cam_3_Counter { get; set; } = 0;
    public int MFS_Middle_Top_Ridge_Cam_4_Status { get; set; } = 0;
    public int MFS_Middle_Top_Ridge_Cam_4_Counter { get; set; } = 0;
    public int MFS_Right_Top_Bracket_Cam_5_Status { get; set; } = 0;
    public int MFS_Right_Top_Bracket_Cam_5_Counter { get; set; } = 0;
    public int MFS_Right_Middle_Guide_Cam_6_Status { get; set; } = 0;
    public int MFS_Right_Middle_Guide_Cam_6_Counter { get; set; } = 0;
    public int MFS_Right_Bottom_Balcony_Cam_7_Status { get; set; } = 0;
    public int MFS_Right_Bottom_Balcony_Cam_7_Counter { get; set; } = 0;

    public int Radiator_Left_Top_Grommet_Cam_8_Status { get; set; } = 0;
    public int Radiator_Left_Top_Grommet_Cam_8_Counter { get; set; } = 0;
    public int Radiator_Right_Top_Grommet_Cam_9_Status { get; set; } = 0;
    public int Radiator_Right_Top_Grommet_Cam_9_Counter { get; set; } = 0;

    public int Shipping_Caps_Disassembly_Status { get; set; } = 0;
    public int Shipping_Caps_Disassembly_Counter { get; set; } = 0;

    public string Grommet_Batch_Nr { get; set; }
}
