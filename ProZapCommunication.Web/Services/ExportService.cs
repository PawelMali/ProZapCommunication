using ClosedXML.Excel;
using ProZapCommunication.Web.Models;
using System.Security.Cryptography.Xml;
using System.Text;

namespace ProZapCommunication.Web.Services;

public class ExportService
{
    // GENEROWANIE PLIKU EXCEL TRACEABILITY (.xlsx)

    public byte[] ExportToExcel(ProductionProcessReport report)
    {
        using var workbook = new XLWorkbook();

        // Summary
        // CreateSummarySheet(workbook, report);

        // Dane procesowe
        AddWorksheetFromData(
            workbook,
            "CRFM 1",
            report.Station1History);

        AddWorksheetFromData(
            workbook,
            "CRFM 2",
            report.Station2History);

        AddWorksheetFromData(
            workbook,
            "CRFM 3",
            report.Station3History);

        AddWorksheetFromData(
            workbook,
            "CRFM 4",
            report.Station4History);

        AddWorksheetFromData(
            workbook,
            "MSF 1",
            report.Station5History);

        AddWorksheetFromData(
            workbook,
            "MSF 2",
            report.Station6History);

        AddWorksheetFromData(
            workbook,
            "MSF 3",
            report.Station7History);

        AddWorksheetFromData(
            workbook,
            "Rework",
            report.Station8History);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    // GENEROWANIE PLIKU EXCEL DLA STACJI (.xlsx)
    public byte[] ExportFilteredStationToExcel<T>(IEnumerable<T> filteredData, string stationName) where T : class
    {
        using var workbook = new XLWorkbook();

        AddWorksheetFromData(
            workbook,
            stationName,
            filteredData);

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    // GENEROWANIE PLIKU CSV DLA STACJI (.csv)
    public byte[] ExportFilteredStationToCsv<T>(IEnumerable<T> filteredData)
        where T : class
    {
        var csvBuilder = new StringBuilder();
        var columns = GetColumnsForType<T>();

        // Nagłówki
        csvBuilder.AppendLine(
            string.Join(";", columns.Select(x =>
                x.Header.Replace("\r\n", " "))));


        // Dane
        foreach (var item in filteredData)
        {
            var values = columns.Select(column =>
            {
                var value = column.ValueGetter(item);

                return value switch
                {
                    DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                    null => string.Empty,
                    _ => value.ToString()?.Replace(";", ",") ?? string.Empty
                };
            });

            csvBuilder.AppendLine(string.Join(";", values));
        }

        return Encoding.UTF8.GetBytes(csvBuilder.ToString());
    }

    // GENEROWANIE ARKUSZA EXCEL Z DANYCH
    private void AddWorksheetFromData<T>(XLWorkbook workbook, string sheetName, IEnumerable<T> data) where T : class
    {
        if (!data.Any())
            return;

        var ws = workbook.Worksheets.Add(sheetName);

        var columns = GetColumnsForType<T>();

        // Nagłówki
        for (int i = 0; i < columns.Length; i++)
        {
            ws.Cell(1, i + 1).Value = columns[i].Header;
        }

        // Dane
        int row = 2;

        foreach (var item in data)
        {
            for (int col = 0; col < columns.Length; col++)
            {
                SetCellValue(ws.Cell(row, col + 1), columns[col].ValueGetter(item));
            }

            row++;
        }

        var range = ws.Range(1, 1, row - 1, columns.Length);
        var table = range.CreateTable();
        table.Theme = XLTableTheme.TableStyleMedium2;

        ws.Columns().AdjustToContents();
    }

    // USTAWIANIE WARTOŚCI KOMÓRKI Z ODPOWIEDNIM FORMATOWANIEM
    private void SetCellValue(IXLCell cell, object? value)
    {
        if (value is DateTime dt)
        {
            cell.Value = dt;
            cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
        }
        else if (value is int || value is long || value is short)
        {
            cell.Value = Convert.ToInt64(value);
            cell.Style.NumberFormat.Format = "#,##0";
        }
        else if (value is decimal || value is double || value is float)
        {
            cell.Value = Convert.ToDecimal(value);
            cell.Style.NumberFormat.Format = "#,##0.00";
        }
        else
        {
            cell.Value = value?.ToString() ?? string.Empty;
        }
    }

    // DEFINICJA KOLUMN DLA KAŻDEJ STACJI
    public record ExportColumn<T>(
        string Header,
        Func<T, object?> ValueGetter);


    // POBIERANIE KOLUMN DLA OKREŚLONEGO TYPU
    private ExportColumn<T>[] GetColumnsForType<T>()
    {
        if (typeof(T) == typeof(Station1Result))
            return (ExportColumn<T>[])(object)Station1Columns;

        if (typeof(T) == typeof(Station2Result))
            return (ExportColumn<T>[])(object)Station2Columns;

        if (typeof(T) == typeof(Station3Result))
            return (ExportColumn<T>[])(object)Station3Columns;

        if (typeof(T) == typeof(Station4Result))
            return (ExportColumn<T>[])(object)Station4Columns;

        if (typeof(T) == typeof(Station5Result))
            return (ExportColumn<T>[])(object)Station5Columns;

        if (typeof(T) == typeof(Station6Result))
            return (ExportColumn<T>[])(object)Station6Columns;

        if (typeof(T) == typeof(Station7Result))
            return (ExportColumn<T>[])(object)Station7Columns;

        if (typeof(T) == typeof(Station8Result))
            return (ExportColumn<T>[])(object)Station8Columns;

        throw new NotSupportedException(
            $"Brak konfiguracji dla {typeof(T).Name}");
    }

    private static readonly ExportColumn<Station1Result>[] Station1Columns =
    {
        new("Timestamp", x => x.Timestamp),
        new("SN", x => x.SN),
        new("SN Status", x => x.SNStatusText),
        new("SN Status Value", x => x.SN_Status),
        new("Radiator SN", x => x.Radiator_SN),
        new("Radiator Status", x => x.RadiatorStatusText),
        new("Radiator Status Value", x => x.Radiator_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),
        new("Grommet Batch", x => x.Grommet_Batch_Nr),
        new("Stonegrid Batch", x => x.Stonegrid_Batch_Nr),
        new("Airduct Batch", x => x.Airduct_Batch_Nr),

        new("Grommet Left\r\nBottom Presence\r\nCAM1 Status", x => x.Grommet_Left_Bottom_Presence_Cam_1Text),
        new("Grommet Left\r\nBottom Presence\r\nCAM1 Counter", x => x.Grommet_Left_Bottom_Presence_Cam_1_Counter),
        new("Grommet Right\r\nBottom Presence\r\nCAM2 Status", x => x.Grommet_Right_Bottom_Presence_Cam_2Text),
        new("Grommet Right\r\nBottom Presence\r\nCAM2 Counter", x => x.Grommet_Right_Bottom_Presence_Cam_2_Counter),
        new("Radiator Left\r\nBottom Grommet\r\nCAM1 Status", x => x.Radiator_Left_Bottom_Grommet_Cam_1Text),
        new("Radiator Left\r\nBottom Grommet\r\nCAM1 Counter", x => x.Radiator_Left_Bottom_Grommet_Cam_1_Counter),
        new("Radiator Right\r\nBottom Grommet\r\nCAM2 Status", x => x.Radiator_Right_Bottom_Grommet_Cam_2Text),
        new("Radiator Right\r\nBottom Grommet\r\nCAM2 Counter", x => x.Radiator_Right_Bottom_Grommet_Cam_2_Counter),
        new("Stonegrid Bottom\r\nLeft Bracket\r\nCAM3 Status", x => x.Stonegrid_Bottom_Left_Bracket_Cam_3Text),
        new("Stonegrid Bottom\r\nLeft Bracket\r\nCAM3 Counter", x => x.Stonegrid_Bottom_Left_Bracket_Cam_3_Counter),
        new("Stonegrid Top\r\nLeft Slider\r\nCAM4 Status", x => x.Stonegrid_Top_Left_Slider_Cam_4Text),
        new("Stonegrid Top\r\nLeft Slider\r\nCAM4 Counter", x => x.Stonegrid_Top_Left_Slider_Cam_4_Counter),
        new("Stonegrid Bottom\r\nRight Bracket\r\nCAM5 Status", x => x.Stonegrid_Bottom_Right_Bracket_Cam_5Text),
        new("Stonegrid Bottom\r\nRight Bracket\r\nCAM5 Counter", x => x.Stonegrid_Bottom_Right_Bracket_Cam_5_Counter),
        new("Stonegrid Top\r\nRight Slider\r\nCAM6 Status", x => x.Stonegrid_Top_Right_Slider_Cam_6Text),
        new("Stonegrid Top\r\nRight Slider\r\nCAM6 Counter", x => x.Stonegrid_Top_Right_Slider_Cam_6_Counter),
        new("Airduct Left\r\nMiddle Slider\r\nCAM4 Status", x => x.Airduct_Left_Middle_Slider_Cam_4Text),
        new("Airduct Left\r\nMiddle Slider\r\nCAM4 Counter", x => x.Airduct_Left_Middle_Slider_Cam_4_Counter),
        new("Airduct Right\r\nMiddle Slider\r\nCAM6 Status", x => x.Airduct_Right_Middle_Slider_Cam_6Text),
        new("Airduct Right\r\nMiddle Slider\r\nCAM6 Counter", x => x.Airduct_Right_Middle_Slider_Cam_6_Counter),
        new("Airduct Left\r\nTop Bracket\r\nCAM7 Status", x => x.Airduct_Left_Top_Bracket_Cam_7Text),
        new("Airduct Left\r\nTop Bracket\r\nCAM7 Counter", x => x.Airduct_Left_Top_Bracket_Cam_7_Counter),
        new("Airduct Right\r\nTop Bracket\r\nCAM8 Status", x => x.Airduct_Right_Top_Bracket_Cam_8Text),
        new("Airduct Right\r\nTop Bracket\r\nCAM8 Counter", x => x.Airduct_Right_Top_Bracket_Cam_8_Counter),
        new("Airduct Left\r\nTop Bracket\r\nCAM9 Status", x => x.Airduct_Left_Top_Balcony_Cam_9Text),
        new("Airduct Left\r\nTop Balcony\r\nCAM9 Counter", x => x.Airduct_Left_Top_Balcony_Cam_9_Counter),
        new("Airduct Right\r\nTop Balcony\r\nCAM10 Status", x => x.Airduct_Right_Top_Balcony_Cam_10Text),
        new("Airduct Right\r\nTop Balcony\r\nCAM10 Counter", x => x.Airduct_Right_Top_Balcony_Cam_10_Counter),
        new("Airduct Middle\r\nBottom Ridge\r\nCAM11 Status", x => x.Airduct_Middle_Bottom_Ridge_Cam_11Text),
        new("Airduct Middle\r\nBottom Ridge\r\nCAM11 Counter", x => x.Airduct_Middle_Bottom_Ridge_Cam_11_Counter)
    };

    private static readonly ExportColumn<Station2Result>[] Station2Columns =
    {
        new("Timestamp", x => x.Timestamp),
        new("SN", x => x.SN),
        new("SN Status", x => x.SNStatusText),
        new("SN Status Value", x => x.SN_Status),
        new("MFS SN", x => x.MFS_SN),
        new("MFS Status", x => x.MFSStatusText),
        new("MFS Status Value", x => x.MFS_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),
        new("Grommet Batch", x => x.Grommet_Batch_Nr),

        new("MFS Left\r\nBottom Balcony\r\nCAM1 Status", x => x.MFS_Left_Bottom_Balcony_Cam_1Text),
        new("MFS Left\r\nBottom Balcony\r\nCAM1 Counter", x => x.MFS_Left_Bottom_Balcony_Cam_1_Counter),
        new("MFS Left\r\nMiddle Guide\r\nCAM2 Status", x => x.MFS_Left_Middle_Guide_Cam_2Text),
        new("MFS Left\r\nMiddle Guide\r\nCAM2 Counter", x => x.MFS_Left_Middle_Guide_Cam_2_Counter),
        new("MFS Left\r\nTop Bracket\r\nCAM3 Status", x => x.MFS_Left_Top_Bracket_Cam_3Text),
        new("MFS Left\r\nTop Bracket\r\nCAM3 Counter", x => x.MFS_Left_Top_Bracket_Cam_3_Counter),
        new("MFS Middle\r\nTop Ridge\r\nCAM4 Status", x => x.MFS_Middle_Top_Ridge_Cam_4Text),
        new("MFS Middle\r\nTop Ridge\r\nCAM4 Counter", x => x.MFS_Middle_Top_Ridge_Cam_4_Counter),
        new("MFS Right\r\nTop Bracket\r\nCAM5 Status", x => x.MFS_Right_Top_Bracket_Cam_5Text),
        new("MFS Right\r\nTop Bracket\r\nCAM5 Counter", x => x.MFS_Right_Top_Bracket_Cam_5_Counter),
        new("MFS Right\r\nMiddle Guide\r\nCAM6 Status", x => x.MFS_Right_Middle_Guide_Cam_6Text),
        new("MFS Right\r\nMiddle Guide\r\nCAM6 Counter", x => x.MFS_Right_Middle_Guide_Cam_6_Counter),
        new("MFS Right\r\nBottom Balcony\r\nCAM7 Status", x => x.MFS_Right_Bottom_Balcony_Cam_7Text),
        new("MFS Right\r\nBottom Balcony\r\nCAM7 Counter", x => x.MFS_Right_Bottom_Balcony_Cam_7_Counter),
        new("Radiator Left\r\nTop Grommet\r\nCAM8 Status", x => x.Radiator_Left_Top_Grommet_Cam_8Text),
        new("Radiator Left\r\nTop Grommet\r\nCAM8 Counter", x => x.Radiator_Left_Top_Grommet_Cam_8_Counter),
        new("Radiator Right\r\nTop Grommet\r\nCAM9 Status", x => x.Radiator_Right_Top_Grommet_Cam_9Text),
        new("Radiator Right\r\nTop Grommet\r\nCAM9 Counter", x => x.Radiator_Right_Top_Grommet_Cam_9_Counter),
        new("Shipping Caps\r\nDisassembly\r\nStatus", x => x.Shipping_Caps_DisassemblyText),
        new("Shipping Caps\r\nDisassembly\r\nCounter", x => x.Shipping_Caps_Disassembly_Counter)
    };

    private static readonly ExportColumn<Station3Result>[] Station3Columns =
    {
        new("Timestamp", x => x.Timestamp),
        new("SN", x => x.SN),
        new("SN Status", x => x.SNStatusText),
        new("SN Status Value", x => x.SN_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),

        new("Leak Test\r\nStatus", x => x.LeakTestText),
        new("Leak Test\r\nCounter", x => x.LeakTest_Counter),
        new("Method", x => x.LeakTest_MethodText),
        new("Program\r\nNumber", x => x.LeakTest_Program_Number),
        new("Pressure\r\n(mPa)", x => x.LeakTest_Pressure),
        new("Pressure Drop\r\n(mPa)", x => x.LeakTest_Pressure_Drop),
        new("Leak Rate\r\n(μL/min)", x => x.LeakTest_Leak_Rate)
        //new("Temperature\r\n(°C)", x => x.LeakTestTemperatureC)
    };

    private static readonly ExportColumn<Station4Result>[] Station4Columns =
    {
        new("Timestamp", x => x.Timestamp),
        new("SN", x => x.SN),
        new("SN Status", x => x.SNStatusText),
        new("SN Status Value", x => x.SN_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),

        new("Shipping Caps\r\nAssembly Status", x => x.Shipping_Caps_AssemblyText),
        new("Shipping Caps\r\nAssembly Counter", x => x.Shipping_Caps_Assembly_Counter),
        new("Print Counter", x => x.Print_Counter),
        new("DMC Quality Content\r\nCAM2 Status", x => x.DMC_Quality_ContentText),
        new("DMC Quality Content\r\nCAM2 Counter", x => x.DMC_Quality_Content_Counter),
        new("DMC Quality Grade\r\nCAM2 Status", x => x.DMC_Quality_GradeText),
        new("DMC Quality Grade\r\nCAM2 Counter", x => x.DMC_Quality_Grade_Counter),
        new("DMC Quality Grade\r\nCAM2 Result", x => x.DMC_Quality_Grade_Result),
        new("DMC OCR\r\nHomologation\r\nPart Number CAM1 Status", x => x.DMC_OCR_Homologation_Part_NumberText),
        new("DMC OCR\r\nHomologation\r\nPart Number CAM1 Counter", x => x.DMC_OCR_Homologation_Part_Number_Counter),
        new("DMC OCR\r\nSupplier\r\nCAM1 Status", x => x.DMC_OCR_SupplierText),
        new("DMC OCR\r\nSupplier\r\nCAM1 Counter", x => x.DMC_OCR_Supplier_Counter),
        new("DMC OCR\r\nCountry Of Origin\r\nCAM1 Status", x => x.DMC_OCR_Country_Of_OriginText),
        new("DMC OCR\r\nCountry Of Origin\r\nCAM1 Counter", x => x.DMC_OCR_Country_Of_Origin_Counter)
    };

    private static readonly ExportColumn<Station5Result>[] Station5Columns =
{
        new("Timestamp", x => x.Timestamp),
        new("Motor SN", x => x.Motor_SN),
        new("Motor Status", x => x.MotorStatusText),
        new("Motor Status Value", x => x.Motor_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),
        new("Screw Batch", x => x.Screw_Batch_Nr),
        new("Fan Batch", x => x.Fan_Batch_Nr),

        new("Motor Inspection\r\nCAM1 Status", x => x.MotorInspectionStatusText),
        new("Motor Inspection\r\nCAM1 Counter", x => x.Motor_Inspection_Counter),
        new("Fan Inspection\r\nCAM1 Status", x => x.FanInspectionStatusText),
        new("Fan Inspection\r\nCAM1 Counter", x => x.Fan_Inspection_Counter),
        new("Screw 1\r\nStatus", x => x.Screw1StatusText),
        new("Screw 1\r\nCounter", x => x.Screw1_Counter),
        new("Screw 1\r\nProgram Number", x => x.Screw1_Program_Number),
        new("Screw 1\r\nTorque", x => x.Screw1_Torque),
        new("Screw 1\r\nAngle", x => x.Screw1_Angle),
        new("Screw 2\r\nStatus", x => x.Screw2StatusText),
        new("Screw 2\r\nCounter", x => x.Screw2_Counter),
        new("Screw 2\r\nProgram Number", x => x.Screw2_Program_Number),
        new("Screw 2\r\nTorque", x => x.Screw2_Torque),
        new("Screw 2\r\nAngle", x => x.Screw2_Angle),
        new("Screw 3\r\nStatus", x => x.Screw3StatusText),
        new("Screw 3\r\nCounter", x => x.Screw3_Counter),
        new("Screw 3\r\nProgram Number", x => x.Screw3_Program_Number),
        new("Screw 3\r\nTorque", x => x.Screw3_Torque),
        new("Screw 3\r\nAngle", x => x.Screw3_Angle)
    };

    private static readonly ExportColumn<Station6Result>[] Station6Columns =
{
        new("Timestamp", x => x.Timestamp),
        new("Motor SN", x => x.Motor_SN),
        new("Motor Status", x => x.MotorStatusText),
        new("Motor Status Value", x => x.Motor_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),

        new("Initial static unbalance", x => x.Initial_static_unbalance.ToString("N2")),
        new("Final static unbalance", x => x.Final_static_unbalance.ToString("N2")),
        new("Runout", x => x.Runout.ToString("N2")),
        new("Initial moment unbalance", x => x.Initial_moment_unbalance.ToString("N2")),
        new("Final moment unbalance", x => x.Final_moment_unbalance.ToString("N2")),
        new("Chamber", x => x.Chamber.ToString("N2")),
        new("Initial static unbalance angle", x => x.Initial_static_unbalance_angle.ToString("N2")),
        new("Final static unbalance angle", x => x.Final_static_unbalance_angle.ToString("N2")),
        new("Rpm", x => x.Rpm.ToString("N2")),
        new("Current", x => x.Current.ToString("N2")),

    };

    private static readonly ExportColumn<Station7Result>[] Station7Columns =
{
        new("Timestamp", x => x.Timestamp),
        new("Motor SN", x => x.Motor_SN),
        new("MFS SN", x => x.Mfs_SN),
        new("Motor/MFS Status", x => x.MotorStatusText),
        new("Motor/MFS Status Value", x => x.Motor_Status),
        new("Cycle\r\nTime (s)", x => x.CycleTime),
        new("Reference number", x => x.Reference_Number),
        new("Screw Batch", x => x.Screw_Batch_Nr),
        new("Cap Batch", x => x.Cap_Batch_Nr),
        new("Shraud Batch", x => x.Shraud_Batch_Nr),

        new("Shraud Type\r\nCAM1 Status", x => x.ShraudStatusText),
        new("Shraud Type\r\nCAM1 Counter", x => x.Shraud_Counter),
        new("Cable Inspection\r\nCAM1 Status", x => x.CableStatusText),
        new("Cable Inspection\r\nCAM1 Counter", x => x.Cable_Counter),
        new("Plug Inspection\r\nCAM2 Status", x => x.PlugStatusText),
        new("Plug Inspection\r\nCAM2 Counter", x => x.Plug_Counter),
        new("Shipping Cap Inspection\r\nCAM1 Status", x => x.ShippingCapStatusText),
        new("Shipping Cap Inspection\r\nCAM1 Counter", x => x.Shipping_Cap_Counter),

        new("Print Counter", x => x.Print_Counter),
        new("DMC Quality Content\r\nCAM4 Status", x => x.DMC_Quality_ContentText),
        new("DMC Quality Content\r\nCAM4 Counter", x => x.DMC_Quality_Content_Counter),
        new("DMC Quality Grade\r\nCAM4 Status", x => x.DMC_Quality_GradeText),
        new("DMC Quality Grade\r\nCAM4 Counter", x => x.DMC_Quality_Grade_Counter),
        new("DMC Quality Grade\r\nCAM4 Result", x => x.DMC_Quality_Grade_Result),
        new("DMC OCR\r\nHomologation\r\nPart Number CAM3 Status", x => x.DMC_OCR_Homologation_Part_NumberText),
        new("DMC OCR\r\nHomologation\r\nPart Number CAM3 Counter", x => x.DMC_OCR_Homologation_Part_Number_Counter),
        new("DMC OCR\r\nSupplier\r\nCAM3 Status", x => x.DMC_OCR_SupplierText),
        new("DMC OCR\r\nSupplier\r\nCAM3 Counter", x => x.DMC_OCR_Supplier_Counter),
        new("DMC OCR\r\nCountry Of Origin\r\nCAM3 Status", x => x.DMC_OCR_Country_Of_OriginText),
        new("DMC OCR\r\nCountry Of Origin\r\nCAM3 Counter", x => x.DMC_OCR_Country_Of_Origin_Counter),

        new("Screw 1\r\nStatus", x => x.Screw1StatusText),
        new("Screw 1\r\nCounter", x => x.Screw1_Counter),
        new("Screw 1\r\nProgram Number", x => x.Screw1_Program_Number),
        new("Screw 1\r\nTorque", x => x.Screw1_Torque),
        new("Screw 1\r\nAngle", x => x.Screw1_Angle),
        new("Screw 2\r\nStatus", x => x.Screw2StatusText),
        new("Screw 2\r\nCounter", x => x.Screw2_Counter),
        new("Screw 2\r\nProgram Number", x => x.Screw2_Program_Number),
        new("Screw 2\r\nTorque", x => x.Screw2_Torque),
        new("Screw 2\r\nAngle", x => x.Screw2_Angle),
        new("Screw 3\r\nStatus", x => x.Screw3StatusText),
        new("Screw 3\r\nCounter", x => x.Screw3_Counter),
        new("Screw 3\r\nProgram Number", x => x.Screw3_Program_Number),
        new("Screw 3\r\nTorque", x => x.Screw3_Torque),
        new("Screw 3\r\nAngle", x => x.Screw3_Angle)
    };

    private static readonly ExportColumn<Station8Result>[] Station8Columns =
{
        new("Timestamp", x => x.Timestamp),
        new("Operator", x => x.Operator_ID),
        new("SN\\Motor", x => x.SN),
        new("SN\\Motor Status", x => x.SNStatusText),
        new("SN\\Motor Status Value", x => x.SN_Status),

        new("Radiator SN", x => x.Radiator_SN),
        new("Radiator Status", x => x.RadiatorStatusText),
        new("Radiator Status Value", x => x.Radiator_Status),

        new("MFS SN", x => x.Mfs_SN),
        new("MFS Status", x => x.MfsStatusText),
        new("MFS Status Value", x => x.Mfs_Status),

        new("Rework Code", x => x.ReworkCode),

    };


}
