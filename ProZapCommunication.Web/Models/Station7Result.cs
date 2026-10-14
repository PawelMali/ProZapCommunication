namespace ProZapCommunication.Web.Models;

public class Station7Result : StationResultMfsBase
{
    public string Mfs_SN { get; set; } = string.Empty;
    public int Mfs_Status { get; set; }

    public int Shraud_Status { get; set; } = 0;
    public int Shraud_Counter { get; set; } = 0;
    public int Cable_Status { get; set; } = 0;
    public int Cable_Counter { get; set; } = 0;
    public int Plug_Status { get; set; } = 0;
    public int Plug_Counter { get; set; } = 0;
    public int Shipping_Cap_Status { get; set; } = 0;
    public int Shipping_Cap_Counter { get; set; } = 0;

    public int Print_Counter { get; set; } = 0;

    public int DMC_Quality_Content_Status { get; set; } = 0;
    public int DMC_Quality_Content_Counter { get; set; } = 0;
    public int DMC_Quality_Grade_Status { get; set; } = 0;
    public int DMC_Quality_Grade_Counter { get; set; } = 0;
    public string DMC_Quality_Grade_Result { get; set; } = string.Empty;

    public int DMC_OCR_Homologation_Part_Number_Status { get; set; } = 0;
    public int DMC_OCR_Homologation_Part_Number_Counter { get; set; } = 0;
    public int DMC_OCR_Supplier_Status { get; set; } = 0;
    public int DMC_OCR_Supplier_Counter { get; set; } = 0;
    public int DMC_OCR_Country_Of_Origin_Status { get; set; } = 0;
    public int DMC_OCR_Country_Of_Origin_Counter { get; set; } = 0;

    public int Screw1_Status { get; set; } = 0;
    public int Screw1_Counter { get; set; } = 0;
    public int Screw1_Program_Number { get; set; } = 0;
    public float Screw1_Angle { get; set; } = 0;
    public float Screw1_Torque { get; set; } = 0;

    public int Screw2_Status { get; set; } = 0;
    public int Screw2_Counter { get; set; } = 0;
    public int Screw2_Program_Number { get; set; } = 0;
    public float Screw2_Angle { get; set; } = 0;
    public float Screw2_Torque { get; set; } = 0;

    public int Screw3_Status { get; set; } = 0;
    public int Screw3_Counter { get; set; } = 0;
    public int Screw3_Program_Number { get; set; } = 0;
    public float Screw3_Angle { get; set; } = 0;
    public float Screw3_Torque { get; set; } = 0;

    public string Screw_Batch_Nr { get; set; } = string.Empty;
    public string Cap_Batch_Nr { get; set; } = string.Empty;
    public string Shraud_Batch_Nr { get; set; } = string.Empty;


    // ====================================================================
    // WŁAŚCIWOŚCI OBLICZENIOWE DLA MUDDATAGRID (Ignorowane przez Dapper)
    // ====================================================================

    public string MotorStatusText => Motor_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string MfsStatusText => Mfs_Status switch
    {
        1 => "OK",
        2 => "NOK",
        >= 3000 and < 4000 => "REWORK",
        4 => "SCRAP",
        _ => "UNKNOWN"
    };

    public string ShraudStatusText => Shraud_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string CableStatusText => Cable_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string PlugStatusText => Plug_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string ShippingCapStatusText => Shipping_Cap_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string DMC_Quality_ContentText => DMC_Quality_Content_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string DMC_Quality_GradeText => DMC_Quality_Grade_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string DMC_OCR_Homologation_Part_NumberText => DMC_OCR_Homologation_Part_Number_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string DMC_OCR_SupplierText => DMC_OCR_Supplier_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string DMC_OCR_Country_Of_OriginText => DMC_OCR_Country_Of_Origin_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Screw1StatusText => Screw1_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Screw2StatusText => Screw2_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };

    public string Screw3StatusText => Screw3_Status switch
    {
        1 => "OK",
        2 => "NOK",
        3 => "MANUAL OK",
        4 => "DISABLED",
        _ => "UNKNOWN"
    };
}
