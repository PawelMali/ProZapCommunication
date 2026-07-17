namespace ProZapCommunication.Web.Models;

public class Station1Result : StationResultBase
{
    public string Radiator_SN { get; set; } = string.Empty;
    public int Radiator_Status { get; set; }
    public int? Grommet_Left_Bottom_Presence_Cam_1_Status { get; set; } = 0;
    public int? Grommet_Left_Bottom_Presence_Cam_1_Counter { get; set; } = 0;
    public int? Grommet_Right_Bottom_Presence_Cam_2_Status { get; set; } = 0;
    public int? Grommet_Right_Bottom_Presence_Cam_2_Counter { get; set; } = 0;

    public int? Radiator_Left_Bottom_Grommet_Cam_1_Status { get; set; } = 0;
    public int? Radiator_Left_Bottom_Grommet_Cam_1_Counter { get; set; } = 0;
    public int? Radiator_Right_Bottom_Grommet_Cam_2_Status { get; set; } = 0;
    public int? Radiator_Right_Bottom_Grommet_Cam_2_Counter { get; set; } = 0;

    public int? Stonegrid_Bottom_Left_Bracket_Cam_3_Status { get; set; } = 0;
    public int? Stonegrid_Bottom_Left_Bracket_Cam_3_Counter { get; set; } = 0;
    public int? Stonegrid_Top_Left_Slider_Cam_4_Status { get; set; } = 0;
    public int? Stonegrid_Top_Left_Slider_Cam_4_Counter { get; set; } = 0;
    public int? Stonegrid_Bottom_Right_Bracket_Cam_5_Status { get; set; } = 0;
    public int? Stonegrid_Bottom_Right_Bracket_Cam_5_Counter { get; set; } = 0;
    public int? Stonegrid_Top_Right_Slider_Cam_6_Status { get; set; } = 0;
    public int? Stonegrid_Top_Right_Slider_Cam_6_Counter { get; set; } = 0;

    public int? Airduct_Left_Middle_Slider_Cam_4_Status { get; set; } = 0;
    public int? Airduct_Left_Middle_Slider_Cam_4_Counter { get; set; } = 0;
    public int? Airduct_Right_Middle_Slider_Cam_6_Status { get; set; } = 0;
    public int? Airduct_Right_Middle_Slider_Cam_6_Counter { get; set; } = 0;
    public int? Airduct_Left_Top_Bracket_Cam_7_Status { get; set; } = 0;
    public int? Airduct_Left_Top_Bracket_Cam_7_Counter { get; set; } = 0;
    public int? Airduct_Right_Top_Bracket_Cam_8_Status { get; set; } = 0;
    public int? Airduct_Right_Top_Bracket_Cam_8_Counter { get; set; } = 0;
    public int? Airduct_Left_Top_Balcony_Cam_9_Status { get; set; } = 0;
    public int? Airduct_Left_Top_Balcony_Cam_9_Counter { get; set; } = 0;
    public int? Airduct_Right_Top_Balcony_Cam_10_Status { get; set; } = 0;
    public int? Airduct_Right_Top_Balcony_Cam_10_Counter { get; set; } = 0;
    public int? Airduct_Middle_Bottom_Ridge_Cam_11_Status { get; set; } = 0;
    public int? Airduct_Middle_Bottom_Ridge_Cam_11_Counter { get; set; } = 0;

    // ====================================================================
    // WŁAŚCIWOŚCI OBLICZENIOWE DLA MUDDATAGRID (Ignorowane przez Dapper)
    // ====================================================================

    public string SNStatusText => SN_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string RadiatorStatusText => Radiator_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string Grommet_Left_Bottom_Presence_Cam_1Text => Grommet_Left_Bottom_Presence_Cam_1_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Grommet_Right_Bottom_Presence_Cam_2Text => Grommet_Right_Bottom_Presence_Cam_2_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Radiator_Left_Bottom_Grommet_Cam_1Text => Radiator_Left_Bottom_Grommet_Cam_1_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Radiator_Right_Bottom_Grommet_Cam_2Text => Radiator_Right_Bottom_Grommet_Cam_2_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Stonegrid_Bottom_Left_Bracket_Cam_3Text => Stonegrid_Bottom_Left_Bracket_Cam_3_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Stonegrid_Top_Left_Slider_Cam_4Text => Stonegrid_Top_Left_Slider_Cam_4_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Stonegrid_Bottom_Right_Bracket_Cam_5Text => Stonegrid_Bottom_Right_Bracket_Cam_5_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Stonegrid_Top_Right_Slider_Cam_6Text => Stonegrid_Top_Right_Slider_Cam_6_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Left_Middle_Slider_Cam_4Text => Airduct_Left_Middle_Slider_Cam_4_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Right_Middle_Slider_Cam_6Text => Airduct_Right_Middle_Slider_Cam_6_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Left_Top_Bracket_Cam_7Text => Airduct_Left_Top_Bracket_Cam_7_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Right_Top_Bracket_Cam_8Text => Airduct_Right_Top_Bracket_Cam_8_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Left_Top_Balcony_Cam_9Text => Airduct_Left_Top_Balcony_Cam_9_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Right_Top_Balcony_Cam_10Text => Airduct_Right_Top_Balcony_Cam_10_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
    public string Airduct_Middle_Bottom_Ridge_Cam_11Text => Airduct_Middle_Bottom_Ridge_Cam_11_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
}
