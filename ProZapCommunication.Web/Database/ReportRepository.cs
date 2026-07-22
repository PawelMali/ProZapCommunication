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

        if (successfulStation2 != null && !string.IsNullOrEmpty(successfulStation2.MFS_SN))
        {
            report.Mfs_SN = successfulStation2.MFS_SN;

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

        // ====================================================================
        // ETAP 5: Zbieranie wszystkich unikalnych numerów seryjnych dla sztuki
        // ====================================================================
        // Pobieramy wartości z historii stacji (na wypadek, gdyby sztuka miała przypisane różne komponenty w różnych próbach)
        var radiatorSns = report.Station1History.Select(x => x.Radiator_SN).Where(sn => !string.IsNullOrEmpty(sn)).Distinct().ToList();
        var mfsSns = report.Station2History.Select(x => x.MFS_SN).Where(sn => !string.IsNullOrEmpty(sn)).Distinct().ToList();
        var motorSns = report.Station7History.Select(x => x.Motor_SN).Where(sn => !string.IsNullOrEmpty(sn)).Distinct().ToList();

        // Dodajemy również wartości bezpośrednio z pól raportu dla pewności
        if (!string.IsNullOrEmpty(report.Mfs_SN) && !mfsSns.Contains(report.Mfs_SN)) mfsSns.Add(report.Mfs_SN);
        if (!string.IsNullOrEmpty(report.Motor_SN) && !motorSns.Contains(report.Motor_SN)) motorSns.Add(report.Motor_SN);

        // Zapytanie SQL do tabeli Station8ResultsStation8Results korzystające z operatora IN (bardzo wydajne w SQL Server)
        string reworkSql = @"
            SELECT * FROM Station8Results 
            WHERE SN = @mainSn 
               OR (Radiator_SN IN @radiatorSns AND @hasRadiators = 1)
               OR (Mfs_SN IN @mfsSns AND @hasMfs = 1)
               OR (Motor_SN IN @motorSns AND @hasMotors = 1)
            ORDER BY Timestamp DESC;";

        var reworkParams = new
        {
            mainSn = report.SN,
            radiatorSns = radiatorSns.Any() ? radiatorSns : new List<string> { "" },
            hasRadiators = radiatorSns.Any() ? 1 : 0,
            mfsSns = mfsSns.Any() ? mfsSns : new List<string> { "" },
            hasMfs = mfsSns.Any() ? 1 : 0,
            motorSns = motorSns.Any() ? motorSns : new List<string> { "" },
            hasMotors = motorSns.Any() ? 1 : 0
        };

        report.Station8History = (await connection.QueryAsync<Station8Result>(reworkSql, reworkParams)).ToList();
        report.TotalAttempts += report.Station8History.Count;

        return report;
    }

    // Główna metoda generyczna zarządzająca wyszukiwaniem po różnych typach
    public async Task<ProductionProcessReport?> GetReportByAnyIdentifierAsync(string idType, string value)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        string? mainSn = null;
        string? foundMfsSn = null;
        string? foundMotorSn = null;

        // KROK 1: Analiza punktu startowego wyszukiwania
        switch (idType.ToUpper())
        {
            case "SN":
                mainSn = value;
                break;

            case "RADIATOR_SN":
                mainSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 SN FROM Station1Results WHERE Radiator_SN = @value ORDER BY Timestamp DESC",
                    new { value });
                break;

            case "MFS_SN":
                foundMfsSn = value;
                // Próbujemy znaleźć główny SN
                mainSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 SN FROM Station2Results WHERE Mfs_SN = @value ORDER BY Timestamp DESC",
                    new { value });
                break;

            case "MOTOR_SN":
                foundMotorSn = value;
                // Próbujemy znaleźć Mfs_SN na stacji 7
                foundMfsSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 Mfs_SN FROM Station7Results WHERE Motor_SN = @value ORDER BY Timestamp DESC",
                    new { value });

                if (!string.IsNullOrEmpty(foundMfsSn))
                {
                    // Próbujemy znaleźć główny SN na stacji 2
                    mainSn = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT TOP 1 SN FROM Station2Results WHERE Mfs_SN = @foundMfsSn ORDER BY Timestamp DESC",
                        new { foundMfsSn });
                }
                break;
        }

        // KROK 2: Jeśli znaleźliśmy główny SN – pobieramy tradycyjnie KOMPLET danych (Stacje 1-7)
        if (!string.IsNullOrEmpty(mainSn))
        {
            return await GetTraceabilityReportAsync(mainSn);
        }

        // KROK 3: AWARYJNY SCENARIUSZ (Silnik wyprodukowany, ale jeszcze NIE POŁĄCZONY)
        // Jeśli nie ma głównego SN, ale szukaliśmy po Motor_SN lub znaleźliśmy go na Stacji 7
        if (!string.IsNullOrEmpty(foundMotorSn) || !string.IsNullOrEmpty(foundMfsSn))
        {
            // Jeżeli znaliśmy tylko Mfs_SN, spróbujmy odnaleźć Motor_SN na stacji 7
            if (string.IsNullOrEmpty(foundMotorSn) && !string.IsNullOrEmpty(foundMfsSn))
            {
                foundMotorSn = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT TOP 1 Motor_SN FROM Station7Results WHERE Mfs_SN = @foundMfsSn ORDER BY Timestamp DESC",
                    new { foundMfsSn });
            }

            if (!string.IsNullOrEmpty(foundMotorSn))
            {
                // Tworzymy niekompletny raport (tylko dla linii silnika)
                var partialReport = new ProductionProcessReport
                {
                    SN = $"[Unlinked Component] Motor: {foundMotorSn}",
                    Mfs_SN = foundMfsSn,
                    Motor_SN = foundMotorSn
                };

                // Pobieramy historię wyłącznie dla stacji sub-zespołu 5, 6, 7
                string subLineSql = @"
                SELECT * FROM Station5Results WHERE Motor_SN = @foundMotorSn ORDER BY Timestamp ASC;
                SELECT * FROM Station6Results WHERE Motor_SN = @foundMotorSn ORDER BY Timestamp ASC;
                SELECT * FROM Station7Results WHERE Motor_SN = @foundMotorSn ORDER BY Timestamp ASC;
            ";

                using (var multiSub = await connection.QueryMultipleAsync(subLineSql, new { foundMotorSn }))
                {
                    partialReport.Station5History = (await multiSub.ReadAsync<Station5Result>()).ToList();
                    partialReport.Station6History = (await multiSub.ReadAsync<Station6Result>()).ToList();
                    partialReport.Station7History = (await multiSub.ReadAsync<Station7Result>()).ToList();
                }

                // ====================================================================
                // NOWY ETAP DLA KROKU 3: Przeszukiwanie Rework tylko po Motor_SN i Mfs_SN
                // ====================================================================
                string partialReworkSql = @"
                    SELECT * FROM Station8Results 
                    WHERE Motor_SN = @foundMotorSn 
                       OR (Mfs_SN = @foundMfsSn AND @hasMfs = 1)
                    ORDER BY Timestamp DESC;";

                var partialReworkParams = new
                {
                    foundMotorSn,
                    foundMfsSn = foundMfsSn ?? string.Empty,
                    hasMfs = !string.IsNullOrEmpty(foundMfsSn) ? 1 : 0
                };

                partialReport.Station8History = (await connection.QueryAsync<Station8Result>(partialReworkSql, partialReworkParams)).ToList();

                partialReport.TotalAttempts = partialReport.Station5History.Count +
                                             partialReport.Station6History.Count +
                                             partialReport.Station7History.Count +
                                             partialReport.Station8History.Count;

                return partialReport;
            }
        }

        return null; // Absolutny brak danych w bazie dla tego numeru
    }

    public async Task<IEnumerable<T>> GetStationDataAsync<T>(int stationId, DateTime fromDate, DateTime toDate, int? status = null) where T : class
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Dynamicznie określamy nazwę tabeli oraz kolumnę statusu (Station 5-7 mają 'Status', 1-4 mają 'SN_Status')
        string tableName = $"Station{stationId}Results";
        string statusColumn = "SN_Status";
            
        if (stationId >= 5 && stationId <= 7)
            statusColumn = "Motor_Status";

        var sqlBuilder = new StringBuilder($"SELECT * FROM {tableName} WHERE Timestamp BETWEEN @fromDate AND @toDate");

        if (status.HasValue)
        {
            if(status.Value == 3)
                sqlBuilder.Append($" AND {statusColumn} >=3000 AND {statusColumn} < 4000");
            else
                sqlBuilder.Append($" AND {statusColumn} = @status");
        }

        sqlBuilder.Append(" ORDER BY Timestamp DESC");

        // Dapper automatycznie zmapuje wynik na generyczny typ stacji <T>
        return await connection.QueryAsync<T>(sqlBuilder.ToString(), new { fromDate, toDate, status });
    }
}
