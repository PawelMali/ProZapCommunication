namespace ProZapCommunication.Web.Models;

public class Station4Result : StationResultBase
{
    public int? Shipping_Caps_Assembly_Status { get; set; } = 0;
    public int? Shipping_Caps_Assembly_Counter { get; set; } = 0;

    public int? Print_Counter { get; set; } = 0;

    public int? DMC_Quality_Content_Status { get; set; } = 0;
    public int? DMC_Quality_Content_Counter { get; set; } = 0;
    public int? DMC_Quality_Grade_Status { get; set; } = 0;
    public int? DMC_Quality_Grade_Counter { get; set; } = 0;
    public string? DMC_Quality_Grade_Result { get; set; } = string.Empty;

    public int? DMC_OCR_Homologation_Part_Number_Status { get; set; } = 0;
    public int? DMC_OCR_Homologation_Part_Number_Counter { get; set; } = 0;
    public int? DMC_OCR_Supplier_Status { get; set; } = 0;
    public int? DMC_OCR_Supplier_Counter { get; set; } = 0;
    public int? DMC_OCR_Country_Of_Origin_Status { get; set; } = 0;
    public int? DMC_OCR_Country_Of_Origin_Counter { get; set; } = 0;


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

    public string Shipping_Caps_AssemblyText => Shipping_Caps_Assembly_Status switch
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
}
