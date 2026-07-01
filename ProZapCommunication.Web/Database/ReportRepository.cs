using Dapper;
using Microsoft.Data.SqlClient;
using ProZapCommunication.Web.Models;
using System.Text;

namespace ProZapCommunication.Web.Database;

public class ReportRepository
{
    private readonly string _connectionString;

    // Wstrzykujemy konfigurację, aby pobrać ConnectionString z appsettings.json
    public ReportRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Brak connection stringa!");
    }

    public async Task<ProductionProcessReport?> GetTraceabilityReportAsync(string sn)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var report = new ProductionProcessReport { SN = sn };

        // ====================================================================
        // ETAP 1: Pobranie historii z Linii Głównej (Stacje 1-4) w JEDNYM zapytaniu
        // ====================================================================
        string mainLineSql = @"
                SELECT * FROM Station1Results WHERE SN = @sn ORDER BY Timestamp DESC;
                SELECT * FROM Station2Results WHERE SN = @sn ORDER BY Timestamp DESC;
                SELECT * FROM Station3Results WHERE SN = @sn ORDER BY Timestamp DESC;
                SELECT * FROM Station4Results WHERE SN = @sn ORDER BY Timestamp DESC;
            ";

        using (var multi = await connection.QueryMultipleAsync(mainLineSql, new { sn }))
        {
            report.Station1History = (await multi.ReadAsync<Station1Result>()).ToList();
            report.Station2History = (await multi.ReadAsync<Station2Result>()).ToList();
            report.Station3History = (await multi.ReadAsync<Station3Result>()).ToList();
            report.Station4History = (await multi.ReadAsync<Station4Result>()).ToList();
        }

        // Zliczamy próby z linii głównej
        report.TotalAttempts += report.Station1History.Count + report.Station2History.Count +
                                report.Station3History.Count + report.Station4History.Count;

        // ====================================================================
        // ETAP 2: Szukamy punktu styku (Mfs_SN na Stacji 2)
        // ====================================================================
        // Zakładamy, że interesuje nas ostatni (najświeższy) udany wpis ze stacji 2
        var successfulStation2 = report.Station2History.LastOrDefault(); 

        if (successfulStation2 != null && !string.IsNullOrEmpty(successfulStation2.Mfs_SN))
        {
            report.Mfs_SN = successfulStation2.Mfs_SN;

            // ====================================================================
            // ETAP 3: Uderzamy do Stacji 7, aby znaleźć Motor_SN
            // ====================================================================
            string st7Sql = "SELECT * FROM Station7Results WHERE Mfs_SN = @mfsSn ORDER BY Timestamp ASC";
            report.Station7History = (await connection.QueryAsync<Station7Result>(st7Sql, new { mfsSn = report.Mfs_SN })).ToList();
            report.TotalAttempts += report.Station7History.Count;

            var successfulStation7 = report.Station7History.LastOrDefault();

            if (successfulStation7 != null && !string.IsNullOrEmpty(successfulStation7.Motor_SN))
            {
                report.Motor_SN = successfulStation7.Motor_SN;

                // ====================================================================
                // ETAP 4: Pobranie reszty historii sub-zespołu (Stacje 5-6)
                // ====================================================================
                string subLineSql = @"
                        SELECT * FROM Station5Results WHERE Motor_SN = @motorSn ORDER BY Timestamp ASC;
                        SELECT * FROM Station6Results WHERE Motor_SN = @motorSn ORDER BY Timestamp ASC;
                    ";

                using (var multiSub = await connection.QueryMultipleAsync(subLineSql, new { motorSn = report.Motor_SN }))
                {
                    report.Station5History = (await multiSub.ReadAsync<Station5Result>()).ToList();
                    report.Station6History = (await multiSub.ReadAsync<Station6Result>()).ToList();
                }

                report.TotalAttempts += report.Station5History.Count + report.Station6History.Count;
            }
        }

        // Opcjonalnie: Tutaj można dodać prostą logikę wyciągającą Min(Timestamp) jako StartTime 
        // i Max(Timestamp) jako EndTime ze wszystkich list.

        return report;
    }

    // Główna metoda generyczna zarządzająca wyszukiwaniem po różnych typach
    public async Task<ProductionProcessReport?> GetReportByAnyIdentifierAsync(string idType, string value)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        string? mainSn = null;

        switch (idType.ToUpper())
        {
            case "SN":
                mainSn = value;
                break;

            case "RADIATOR_SN":
                // Szukamy głównego SN na Stacji 1 na podstawie Radiator_SN
                mainSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 SN FROM Station1Results WHERE Radiator_SN = @value ORDER BY Timestamp DESC",
                    new { value });
                break;

            case "MFS_SN":
                // Mfs_SN pojawia się na Stacji 2 oraz Stacji 7. 
                // Najbezpieczniej szukać na Stacji 2, bo tam jest bezpośrednio powiązany z głównym SN.
                mainSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 SN FROM Station2Results WHERE Mfs_SN = @value ORDER BY Timestamp DESC",
                    new { value });
                break;

            case "MOTOR_SN":
                // Silnik (Motor_SN) idzie najpierw przez stacje 5-7. Na stacji 7 dostaje Mfs_SN.
                // KROK A: Znajdź Mfs_SN na podstawie Motor_SN na Stacji 7
                var mfsSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 Mfs_SN FROM Station7Results WHERE Motor_SN = @value ORDER BY Timestamp DESC",
                    new { value });

                if (!string.IsNullOrEmpty(mfsSn))
                {
                    // KROK B: Mając Mfs_SN, znajdź główny SN na Stacji 2
                    mainSn = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT TOP 1 SN FROM Station2Results WHERE Mfs_SN = @mfsSn ORDER BY Timestamp DESC",
                        new { mfsSn });
                }
                break;
        }

        if (string.IsNullOrEmpty(mainSn))
        {
            return null; // Nie znaleziono powiązania z głównym produktem
        }

        // Wywołujemy naszą istniejącą, kompletną metodę pobierania całego drzewa danych
        return await GetTraceabilityReportAsync(mainSn);
    }

    public async Task<IEnumerable<T>> GetStationDataAsync<T>(int stationId, DateTime fromDate, DateTime toDate, int? status = null) where T : class
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Dynamicznie określamy nazwę tabeli oraz kolumnę statusu (Station 5-7 mają 'Status', 1-4 mają 'SN_Status')
        string tableName = $"Station{stationId}Results";
        string statusColumn = stationId >= 5 ? "Status" : "SN_Status";

        var sqlBuilder = new StringBuilder($"SELECT * FROM {tableName} WHERE Timestamp BETWEEN @fromDate AND @toDate");

        if (status.HasValue)
        {
            sqlBuilder.Append($" AND {statusColumn} = @status");
        }

        sqlBuilder.Append(" ORDER BY Timestamp DESC");

        // Dapper automatycznie zmapuje wynik na generyczny typ stacji <T>
        return await connection.QueryAsync<T>(sqlBuilder.ToString(), new { fromDate, toDate, status });
    }
}
