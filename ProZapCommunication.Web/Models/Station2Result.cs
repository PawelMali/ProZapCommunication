namespace ProZapCommunication.Web.Models;

public class Station2Result : StationResultBase
{
    public string MFS_SN { get; set; } = string.Empty;
    public int MFS_Status { get; set; }
    public int? MFS_Left_Bottom_Balcony_Cam_1_Status { get; set; } = 0;
    public int? MFS_Left_Bottom_Balcony_Cam_1_Counter { get; set; } = 0;
    public int? MFS_Left_Middle_Guide_Cam_2_Status { get; set; } = 0;
    public int? MFS_Left_Middle_Guide_Cam_2_Counter { get; set; } = 0;
    public int? MFS_Left_Top_Bracket_Cam_3_Status { get; set; } = 0;
    public int? MFS_Left_Top_Bracket_Cam_3_Counter { get; set; } = 0;
    public int? MFS_Middle_Top_Ridge_Cam_4_Status { get; set; } = 0;
    public int? MFS_Middle_Top_Ridge_Cam_4_Counter { get; set; } = 0;
    public int? MFS_Right_Top_Bracket_Cam_5_Status { get; set; } = 0;
    public int? MFS_Right_Top_Bracket_Cam_5_Counter { get; set; } = 0;
    public int? MFS_Right_Middle_Guide_Cam_6_Status { get; set; } = 0;
    public int? MFS_Right_Middle_Guide_Cam_6_Counter { get; set; } = 0;
    public int? MFS_Right_Bottom_Balcony_Cam_7_Status { get; set; } = 0;
    public int? MFS_Right_Bottom_Balcony_Cam_7_Counter { get; set; } = 0;

    public int? Radiator_Left_Top_Grommet_Cam_8_Status { get; set; } = 0;
    public int? Radiator_Left_Top_Grommet_Cam_8_Counter { get; set; } = 0;
    public int? Radiator_Right_Top_Grommet_Cam_9_Status { get; set; } = 0;
    public int? Radiator_Right_Top_Grommet_Cam_9_Counter { get; set; } = 0;

    public int? Shipping_Caps_Disassembly_Status { get; set; } = 0;
    public int? Shipping_Caps_Disassembly_Counter { get; set; } = 0;


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

    public string MFSStatusText => MFS_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string MFS_Left_Bottom_Balcony_Cam_1Text => MFS_Left_Bottom_Balcony_Cam_1_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string MFS_Left_Middle_Guide_Cam_2Text => MFS_Left_Middle_Guide_Cam_2_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string MFS_Left_Top_Bracket_Cam_3Text => MFS_Left_Top_Bracket_Cam_3_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string MFS_Middle_Top_Ridge_Cam_4Text => MFS_Middle_Top_Ridge_Cam_4_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string MFS_Right_Top_Bracket_Cam_5Text => MFS_Right_Top_Bracket_Cam_5_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string MFS_Right_Middle_Guide_Cam_6Text => MFS_Right_Middle_Guide_Cam_6_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string MFS_Right_Bottom_Balcony_Cam_7Text => MFS_Right_Bottom_Balcony_Cam_7_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Radiator_Left_Top_Grommet_Cam_8Text => Radiator_Left_Top_Grommet_Cam_8_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Radiator_Right_Top_Grommet_Cam_9Text => Radiator_Right_Top_Grommet_Cam_9_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Shipping_Caps_DisassemblyText => Shipping_Caps_Disassembly_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

}
