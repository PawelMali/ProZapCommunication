using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

/////////////////////////////////////////////////////////////////////
//  CRFM 1 - STATION 1 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

public class Station1ResultsData : IStationProcessData
{
    public string SN { get; set; } 
    public int SN_Status { get; set; } = 0;
    public string Radiator_SN { get; set; }
    public int Radiator_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; }
    public int Reference_Number { get; set; } = 0;

    public int Grommet_Left_Bottom_Presence_Cam_1_Status { get; set; } = 0;
    public int Grommet_Left_Bottom_Presence_Cam_1_Counter { get; set; } = 0;
    public int Grommet_Right_Bottom_Presence_Cam_2_Status { get; set; } = 0;
    public int Grommet_Right_Bottom_Presence_Cam_2_Counter { get; set; } = 0;

    public int Radiator_Left_Bottom_Grommet_Cam_1_Status { get; set; } = 0;
    public int Radiator_Left_Bottom_Grommet_Cam_1_Counter { get; set; } = 0;
    public int Radiator_Right_Bottom_Grommet_Cam_2_Status { get; set; } = 0;
    public int Radiator_Right_Bottom_Grommet_Cam_2_Counter { get; set; } = 0;

    public int Stonegrid_Bottom_Left_Bracket_Cam_3_Status { get; set; } = 0;
    public int Stonegrid_Bottom_Left_Bracket_Cam_3_Counter { get; set; } = 0;
    public int Stonegrid_Top_Left_Slider_Cam_4_Status { get; set; } = 0;
    public int Stonegrid_Top_Left_Slider_Cam_4_Counter { get; set; } = 0;
    public int Stonegrid_Bottom_Right_Bracket_Cam_5_Status { get; set; } = 0;
    public int Stonegrid_Bottom_Right_Bracket_Cam_5_Counter { get; set; } = 0;
    public int Stonegrid_Top_Right_Slider_Cam_6_Status { get; set; } = 0;
    public int Stonegrid_Top_Right_Slider_Cam_6_Counter { get; set; } = 0;

    public int Airduct_Left_Middle_Slider_Cam_4_Status { get; set; } = 0;
    public int Airduct_Left_Middle_Slider_Cam_4_Counter { get; set; } = 0;
    public int Airduct_Right_Middle_Slider_Cam_6_Status { get; set; } = 0;
    public int Airduct_Right_Middle_Slider_Cam_6_Counter { get; set; } = 0;
    public int Airduct_Left_Top_Bracket_Cam_7_Status { get; set; } = 0;
    public int Airduct_Left_Top_Bracket_Cam_7_Counter { get; set; } = 0;
    public int Airduct_Right_Top_Bracket_Cam_8_Status { get; set; } = 0;
    public int Airduct_Right_Top_Bracket_Cam_8_Counter { get; set; } = 0;
    public int Airduct_Left_Top_Balcony_Cam_9_Status { get; set; } = 0;
    public int Airduct_Left_Top_Balcony_Cam_9_Counter { get; set; } = 0;
    public int Airduct_Right_Top_Balcony_Cam_10_Status { get; set; } = 0;
    public int Airduct_Right_Top_Balcony_Cam_10_Counter { get; set; } = 0;
    public int Airduct_Middle_Bottom_Ridge_Cam_11_Status { get; set; } = 0;
    public int Airduct_Middle_Bottom_Ridge_Cam_11_Counter { get; set; } = 0;

    public string Grommet_Batch_Nr { get; set; }
    public string Stonegrid_Batch_Nr { get; set; }
    public string Airduct_Batch_Nr { get; set; }

}
