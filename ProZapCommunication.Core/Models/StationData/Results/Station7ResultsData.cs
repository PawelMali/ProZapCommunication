using ProZapCommunication.Core.Interfaces;

namespace ProZapCommunication.Core.Models.StationData.Results;

/////////////////////////////////////////////////////////////////////
//  MFS 3 - STATION 7 - RESULTS DATA STRUCTURE
////////////////////////////////////////////////////////////////////

public class Station7ResultsData : IStationProcessData
{
    public string Motor_SN { get; set; } = String.Empty;
    public int Motor_Status { get; set; } = 0;
    public string Mfs_SN { get; set; } = String.Empty;
    public int Mfs_Status { get; set; } = 0;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int CycleTime { get; set; } = 0;
    public int Reference_Number { get; set; } = 0;

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

    public string Screw_Batch_Nr { get; set; } = String.Empty;
    public string Cap_Batch_Nr { get; set; } = String.Empty;
    public string Shraud_Batch_Nr { get; set; } = String.Empty;
}
