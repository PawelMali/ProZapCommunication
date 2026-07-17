using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;

[StructLayout(LayoutKind.Sequential)]
public class LogixStation4ResultsData
{
    public LogixString DMC = new LogixString();
    public short ResultCode = 0;
    public short CycleTime = 0;
    public short Reference_Number = 0;

    public short Fill1 = 0;

    public short Shipping_Caps_Assembly_Status = 0;
    public short Shipping_Caps_Assembly_Counter = 0;

    public short Print_Counter = 0;

    public short Fill2 = 0;

    public short DMC_Quality_Content_Status = 0;
    public short DMC_Quality_Content_Counter = 0;
    public short DMC_Quality_Grade_Status = 0;
    public short DMC_Quality_Grade_Counter = 0;
    public byte DMC_Quality_Grade_Result = 0;

    public short Fill3 = 0;

    public short DMC_OCR_Homologation_Part_Number_Status = 0;
    public short DMC_OCR_Homologation_Part_Number_Counter = 0;
    public short DMC_OCR_Supplier_Status = 0;
    public short DMC_OCR_Supplier_Counter = 0;
    public short DMC_OCR_Country_Of_Origin_Status = 0;
    public short DMC_OCR_Country_Of_Origin_Counter = 0;
}
