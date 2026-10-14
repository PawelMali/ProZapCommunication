using System.Runtime.InteropServices;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;


/////////////////////////////////////////////////////////////////////
//  MFS 3 - STATION 7 - RESULTS DATA STRUCTURE
/////////////////////////////////////////////////////////////////////

[StructLayout(LayoutKind.Sequential)]
public class LogixStation7ResultsData
{
    public LogixString Motor_Sn = new LogixString();
    public short Motor_Status = 0;
    public LogixString Mfs_Sn = new LogixString();
    public short Mfs_Status = 0;
    public short CycleTime = 0;
    public short Reference_Number = 0;

    public short Fill1 = 0;

    public short Shraud_Status = 0;
    public short Shraud_Counter = 0;
    public short Cable_Status = 0;
    public short Cable_Counter = 0;
    public short Plug_Status = 0;
    public short Plug_Counter = 0;
    public short Shipping_Cap_Status = 0;
    public short Shipping_Cap_Counter = 0;

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

    public LogixScrewData[] Screw = InitializeArray<LogixScrewData>(3);

    public LogixString Screw_Batch_Nr = new LogixString();
    public LogixString Cap_Batch_Nr = new LogixString();
    public LogixString Shraud_Batch_Nr = new LogixString();



    public static T[] InitializeArray<T>(Int32 length) where T : new()
    {
        T[] array = new T[length];
        for (Int32 i = 0; i < length; i++)
        {
            array[i] = new T();
        }
        return array;
    }

}
