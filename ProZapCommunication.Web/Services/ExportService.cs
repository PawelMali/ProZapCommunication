using ClosedXML.Excel;
using ProZapCommunication.Web.Models;
using System.Text;

namespace ProZapCommunication.Web.Services;

public class ExportService
{
    // 1. GENEROWANIE PLIKU EXCEL (.xlsx)
    public byte[] ExportToExcel(ProductionProcessReport report)
    {
        using var workbook = new XLWorkbook();

        // --- SHEET 1: SUMMARY ---
        var wsSummary = workbook.Worksheets.Add("Summary");
        wsSummary.Cell(1, 1).Value = "Traceability Summary Report";
        wsSummary.Cell(1, 1).Style.Font.Bold = true;
        wsSummary.Cell(1, 1).Style.Font.FontSize = 14;

        wsSummary.Cell(3, 1).Value = "Main SN:";
        wsSummary.Cell(3, 2).Value = report.SN;
        wsSummary.Cell(4, 1).Value = "Linked Mfs SN:";
        wsSummary.Cell(4, 2).Value = report.Mfs_SN ?? "N/A";
        wsSummary.Cell(5, 1).Value = "Linked Motor SN:";
        wsSummary.Cell(5, 2).Value = report.Motor_SN ?? "N/A";
        wsSummary.Cell(6, 1).Value = "Total Operations:";
        wsSummary.Cell(6, 2).Value = report.TotalAttempts;
        wsSummary.Columns().AdjustToContents();

        // --- SHEET 2: STATION 1 ---
        if (report.Station1History.Any())
        {
            var ws = workbook.Worksheets.Add("Station 1");
            string[] headers = { "Timestamp", "Status", "Cycle Time [ms]", "Radiator SN", "Torque", "Angle" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];

            int row = 2;
            foreach (var item in report.Station1History)
            {
                ws.Cell(row, 1).Value = item.Timestamp;
                ws.Cell(row, 2).Value = item.SN_Status == 1 ? "OK" : "NOK";
                ws.Cell(row, 3).Value = item.CycleTime;
                ws.Cell(row, 4).Value = item.Radiator_SN;
                ws.Cell(row, 5).Value = item.Torque;
                ws.Cell(row, 6).Value = item.Angle;
                row++;
            }
            ws.Columns().AdjustToContents();
        }

        // --- SHEET 3: STATION 2 ---
        if (report.Station2History.Any())
        {
            var ws = workbook.Worksheets.Add("Station 2");
            string[] headers = { "Timestamp", "Status", "Cycle Time [ms]", "Mfs SN", "Pressure", "Test Time [s]", "Leak" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];

            int row = 2;
            foreach (var item in report.Station2History)
            {
                ws.Cell(row, 1).Value = item.Timestamp;
                ws.Cell(row, 2).Value = item.SN_Status == 1 ? "OK" : "NOK";
                ws.Cell(row, 3).Value = item.CycleTime;
                ws.Cell(row, 4).Value = item.Mfs_SN;
                ws.Cell(row, 5).Value = item.Preasure.HasValue ? Math.Round(item.Preasure.Value, 2) : "";
                ws.Cell(row, 6).Value = item.TestTime;
                ws.Cell(row, 7).Value = item.Leak;
                row++;
            }
            ws.Columns().AdjustToContents();
        }

        // [W analogiczny sposób możesz dodać arkusze dla Stacji 3, 4, 5, 6, 7]

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // 2. GENEROWANIE PLIKU CSV (.csv)
    public byte[] ExportToCsv(ProductionProcessReport report)
    {
        var csvBuilder = new StringBuilder();

        // Nagłówki dla płaskiego pliku CSV podsumowania operacji
        csvBuilder.AppendLine("Station;Timestamp;Status;CycleTime_ms;Main_SN;Component_SN");

        foreach (var item in report.Station1History)
            csvBuilder.AppendLine($"Station 1;{item.Timestamp:yyyy-MM-dd HH:mm:ss};{(item.SN_Status == 1 ? "OK" : "NOK")};{item.CycleTime};{report.SN};{item.Radiator_SN}");

        foreach (var item in report.Station2History)
            csvBuilder.AppendLine($"Station 2;{item.Timestamp:yyyy-MM-dd HH:mm:ss};{(item.SN_Status == 1 ? "OK" : "NOK")};{item.CycleTime};{report.SN};{item.Mfs_SN}");

        // Zwracamy tablicę bajtów zakodowaną w UTF-8, aby zachować poprawne znaki
        return Encoding.UTF8.GetBytes(csvBuilder.ToString());
    }

    public byte[] ExportFilteredStationToExcel<T>(IEnumerable<T> filteredData, string stationName) where T : class
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(stationName);

        // Definiujemy nagłówki dokładnie w kolejności z Grida
        var headers = GetOrderedHeadersForType<T>();

        // 1. Renderowanie Nagłówków
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // 2. Renderowanie Danych
        int row = 2;
        foreach (var item in filteredData)
        {
            for (int col = 0; col < headers.Length; col++)
            {
                var value = GetPropertyValueByHeader(item, headers[col]);

                // Formatowanie specyficzne dla daty, aby Excel traktował ją jako czas, a nie tekst
                if (value is DateTime dt)
                {
                    ws.Cell(row, col + 1).Value = dt;
                    ws.Cell(row, col + 1).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                }
                else
                {
                    ws.Cell(row, col + 1).Value = value?.ToString() ?? string.Empty;
                }
            }
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // Generowanie CSV dla WYFILTROWANYCH danych jednej, konkretnej stacji
    public byte[] ExportFilteredStationToCsv<T>(IEnumerable<T> filteredData) where T : class
    {
        var csvBuilder = new StringBuilder();
        var headers = GetOrderedHeadersForType<T>();

        // 1. Nagłówki
        csvBuilder.AppendLine(string.Join(";", headers));

        // 2. Dane
        foreach (var item in filteredData)
        {
            var line = headers.Select(header => {
                var val = GetPropertyValueByHeader(item, header);
                if (val is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
                return val?.ToString()?.Replace(";", ",") ?? "";
            });
            csvBuilder.AppendLine(string.Join(";", line));
        }

        return Encoding.UTF8.GetBytes(csvBuilder.ToString());
    }

    private string[] GetOrderedHeadersForType<T>()
    {
        if (typeof(T) == typeof(Station1Result))
            return new[] { "Timestamp", "Serial Number", "Cycle Time (ms)", "Status", "Radiator SN", "Torque", "Angle" };

        if (typeof(T) == typeof(Station2Result))
            return new[] { "Timestamp", "Serial Number", "Cycle Time (ms)", "Status", "Mfs SN", "Pressure", "Leak Rate" };

        // Tutaj w przyszłości dodasz kolejne stacje: 3, 4, 5, 6, 7...
        return typeof(T).GetProperties().Select(p => p.Name).ToArray();
    }

    private object? GetPropertyValueByHeader(object item, string header)
    {
        // Mapowanie czytelnego nagłówka z UI na techniczną nazwę właściwości w klasie C#
        string propName = header switch
        {
            "Timestamp" => "Timestamp",
            "Serial Number" => "SN",
            "Cycle Time (ms)" => "CycleTime",
            "Status" => typeof(StationResultBase).IsAssignableFrom(item.GetType()) ? "SN_Status" : "Status",
            "Radiator SN" => "Radiator_SN",
            "Torque" => "Torque",
            "Angle" => "Angle",
            "Mfs SN" => "Mfs_SN",
            "Pressure" => "Preasure",
            "Leak Rate" => "Leak",
            _ => header // Fallback
        };

        return item.GetType().GetProperty(propName)?.GetValue(item);
    }
}
