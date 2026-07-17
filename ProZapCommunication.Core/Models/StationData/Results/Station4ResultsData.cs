using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

/////////////////////////////////////////////////////////////////////
//  CRFM 4 - STATION 4 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

public class Station4ResultsData : IStationProcessData
{
    public string SN { get; set; }
    public int SN_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; }
    public int CycleTime { get; set; } = 0;
    public int Reference_Number { get; set; } = 0;

    public int Shipping_Caps_Assembly_Status { get; set; } = 0;
    public int Shipping_Caps_Assembly_Counter { get; set; } = 0;

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
}
