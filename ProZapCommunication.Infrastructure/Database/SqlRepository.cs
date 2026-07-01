using Dapper;
using Microsoft.Data.SqlClient;
using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Results;
using Serilog;
using System.Data;

namespace ProZapCommunication.Infrastructure.Database;

public class SqlRepository : IDatabaseRepository
{
    private readonly string _connectionString;

    public SqlRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public int SaveProductionStation1Data(string stationIdentity, int stationId, Station1ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station1Results
        (StationId, SN, SN_Status, Radiator_SN, Radiator_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @SN, @SN_Status, @Radiator_SN, @Radiator_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    SN = data.SN,
                    SN_Status = data.SN_Status,
                    Radiator_SN = data.Radiator_SN,
                    Radiator_Status = data.Radiator_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int SaveProductionStation2Data(string stationIdentity, int stationId, Station2ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station2Results
        (StationId, SN, SN_Status, MFS_SN, MFS_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @SN, @SN_Status, @MFS_SN, @MFS_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    SN = data.SN,
                    SN_Status = data.SN_Status,
                    MFS_SN = data.MFS_SN,
                    MFS_Status = data.MFS_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int SaveProductionStation3Data(string stationIdentity, int stationId, Station3ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station3Results
        (StationId, SN, SN_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @SN, @SN_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    SN = data.SN,
                    SN_Status = data.SN_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int SaveProductionStation4Data(string stationIdentity, int stationId, Station4ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station4Results
        (StationId, SN, SN_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @SN, @SN_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    SN = data.SN,
                    SN_Status = data.SN_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int SaveProductionStation5Data(string stationIdentity, int stationId, Station5ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.Motor_SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station5Results
        (StationId, Motor_SN, Motor_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @Motor_SN, @Motor_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    Motor_SN = data.Motor_SN,
                    Motor_Status = data.Motor_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.Motor_SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int SaveProductionStation6Data(string stationIdentity, int stationId, Station6ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.Motor_SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station6Results
        (StationId, Motor_SN, Motor_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @Motor_SN, @Motor_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    Motor_SN = data.Motor_SN,
                    Motor_Status = data.Motor_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.Motor_SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int SaveProductionStation7Data(string stationIdentity, int stationId, Station7ResultsData data)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);


        // Walidacja
        if (data == null)
        {
            stationLog.Warning("S{Id} [SQL]: no data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsData;
        }
        if (string.IsNullOrWhiteSpace(data.Motor_SN))
        {
            stationLog.Warning("S{Id} [SQL]: missing DMC in data to save", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        const string sql = @"
        INSERT INTO Station7Results
        (StationId, Motor_SN, Motor_Status, Mfs_SN, Mfs_Status, CycleTime, Timestamp)
        VALUES
        (@StationId, @Motor_SN, @Motor_Status, @Mfs_SN, @Mfs_Status, @CycleTime, @Timestamp);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    Motor_SN = data.Motor_SN,
                    Motor_Status = data.Motor_Status,
                    Mfs_SN = data.Mfs_SN,
                    Mfs_Status = data.Mfs_Status,
                    CycleTime = data.CycleTime,
                    Timestamp = DateTime.Now
                });
            }

            stationLog.Debug("S{Id} [SQL]: Save data - DMC {Dmc}",
                stationId, data.Motor_SN);

            return (int)PlcTaskStatus.Success;
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when saving", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when saving", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }








    public int CheckSnCurrentStationStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station{stationId}Results";

        string sql = $@"
        SELECT TOP 1 SN_Status
        FROM {tableName}
        WHERE SN = @SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { SN = sn });

                // Brak wpisu -> SUCCESS
                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no previous entry for SN {SN} -> OK",
                        stationId, sn);
                    return (int)PlcTaskStatus.Success;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} already processed -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneOK;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} already processed -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneNOK;

                    case 301:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 1 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt1;

                    case 302:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 2 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt2;

                    case 303:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 3 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt3;

                    case 304:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 4 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt4;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: SN {SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int CheckSnPreviousStationStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station{stationId}Results";

        string sql = $@"
        SELECT TOP 1 SN_Status
        FROM {tableName}
        WHERE SN = @SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { SN = sn });

                // Brak wpisu -> SUCCESS
                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no entry for SN {SN} on previous station -> NOK",
                        stationId, sn);
                    return (int)PlcTaskStatus.PreviousPartNotFound;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} on previous station OK -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.Success;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} on previous station -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNOK;

                    case 301:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 1 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 302:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 2 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 303:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 3 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 304:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 4 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: SN {SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }

    public int CheckMotorSnCurrentStationStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing Motor SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station{stationId}Results";

        string sql = $@"
        SELECT TOP 1 Motor_Status
        FROM {tableName}
        WHERE Motor_SN = @Motor_SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { Motor_SN = sn });

                // Brak wpisu -> SUCCESS
                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no previous entry for Motor SN {Motor_SN} -> OK",
                        stationId, sn);
                    return (int)PlcTaskStatus.Success;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} already processed -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneOK;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} already processed -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneNOK;

                    case 305:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station Mfs 1 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt5;

                    case 306:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station Balancing -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt6;

                    case 307:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station Mfs 3 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt7;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: Motor SN {Motor_SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking Motor SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking Motor SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int CheckMotorSnPreviousStationStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing Motor_SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station{stationId}Results";

        string sql = $@"
        SELECT TOP 1 Motor_Status
        FROM {tableName}
        WHERE Motor_SN = @Motor_SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { Motor_SN = sn });

                // Brak wpisu -> SUCCESS
                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no entry for Motor SN {Motor_SN} on previous station -> NOK",
                        stationId, sn);
                    return (int)PlcTaskStatus.PreviousPartNotFound;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} on previous station OK -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.Success;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} on previous station -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNOK;

                    case 305:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station 5 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 306:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station 6 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 307:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station 7 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: Motor SN {Motor_SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking Motor SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking Motor SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }

    public int CheckMfsSnCurrentStationStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing Mfs SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station{stationId}Results";

        string sql = $@"
        SELECT TOP 1 Mfs_Status
        FROM {tableName}
        WHERE Mfs_SN = @Mfs_SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { Mfs_SN = sn });

                // Brak wpisu -> SUCCESS
                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no previous entry for Mfs SN {Mfs_SN} -> OK",
                        stationId, sn);
                    return (int)PlcTaskStatus.Success;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} already processed -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneOK;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} already processed -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneNOK;

                    case 301:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 1 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt1;

                    case 302:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 2 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt2;

                    case 303:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 3 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt3;

                    case 304:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 4 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt4;

                    case 305:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station Mfs 1 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt5;

                    case 306:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station Balancing -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt6;

                    case 307:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station Mfs 3 -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt7;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: Mfs SN {Mfs_SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking Motor SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking Motor SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }
    public int CheckMfsSnPreviousStationStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing Mfs SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station7Results";

        string sql = $@"
        SELECT TOP 1 Mfs_Status
        FROM {tableName}
        WHERE Mfs_SN = @Mfs_SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { Mfs_SN = sn });

                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no previous entry for Mfs SN {Mfs_SN} -> NOK",
                        stationId, sn);
                    return (int)PlcTaskStatus.PreviousPartNotFound;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} on previous station OK -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.Success;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} on previous station -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNOK;

                    case 305:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 5 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 306:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 6 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 307:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 7 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: Mfs SN {Mfs_SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking Mfs SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking Mfs SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }

    public int CheckRadiatorSnStatus(string stationIdentity, int stationId, string sn)
    {
        var stationLog = Log.ForContext("PlcIdentity", stationIdentity);

        if (string.IsNullOrWhiteSpace(sn))
        {
            stationLog.Warning("S{Id} [SQL]: Missing Radiator SN to check", stationId);
            return (int)PlcTaskStatus.SQLEmptyResultsDMC;
        }

        string tableName = $"Station1Results";

        string sql = $@"
        SELECT TOP 1 Radiator_Status
        FROM {tableName}
        WHERE Radiator_SN = @Radiator_SN
        ORDER BY Timestamp DESC;";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var status = connection.QueryFirstOrDefault<int?>(sql, new { Radiator_SN = sn });

                // Brak wpisu -> SUCCESS
                if (status == null)
                {
                    stationLog.Debug("S{Id} [SQL]: no previous entry for Radiator SN {Radiator_SN} -> OK",
                        stationId, sn);
                    return (int)PlcTaskStatus.Success;
                }

                switch (status)
                {
                    case 1:
                        stationLog.Debug("S{Id} [SQL]: Radiator SN {Radiator_SN} already processed -> OK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneOK;

                    case 2:
                        stationLog.Debug("S{Id} [SQL]: Radiator SN {Radiator_SN} already processed -> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneNOK;

                    case 301:
                        stationLog.Debug("S{Id} [SQL]: Radiator SN {Radiator_SN} -> REWORK",
                            stationId, sn);
                        return (int)PlcTaskStatus.ReworkSt1;

                    case 4:
                        stationLog.Debug("S{Id} [SQL]: Radiator SN {Radiator_SN} already processed -> SCRAP",
                            stationId, sn);
                        return (int)PlcTaskStatus.PartAlreadyDoneSCRAP;

                    default:
                        stationLog.Warning("S{Id} [SQL]: Radiator SN {Radiator_SN} has unknown status {Status}",
                            stationId, sn, status);
                        return (int)PlcTaskStatus.PartAlreadyDoneUNKNOWN;
                }
            }
        }
        catch (SqlException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: SQL error when checking Radiator SN", stationId);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
        catch (InvalidOperationException ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: connection error with DB", stationId);
            return (int)PlcTaskStatus.SQLConnectionError;
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL]: unknown error when checking Radiator SN", stationId);
            return (int)PlcTaskStatus.SQLUnknownError;
        }
    }


    public int ProcessRadiatorRework(string radiatorSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity)
    {
        var stationLog = Log.ForContext("StationIdentity", stationIdentity.Replace(".", "_"));

        // 1. Zapytanie szukające najnowszego ID logu ze Stacji 1 dla danego SN radiatora
        const string sqlFindLatest = @"
                SELECT TOP 1 Id 
                FROM Station1Results 
                WHERE StationId = 1 AND Radiator_SN = @RadiatorSn 
                ORDER BY Timestamp DESC;";

        // 2. Zapytanie aktualizujące znaleziony rekord ze Stacji 1
        const string sqlUpdateProduction = @"
                UPDATE Station1Results 
                SET Radiator_Status = @ReworkStatus, 
                    SN_Status = @ReworkStatus 
                WHERE Id = @LogId;";

        // 3. Zapytanie wstawiające rekord do tabeli reworku
        const string sqlInsertRework = @"
                INSERT INTO StationRework (StationId, Timestamp, SN, SN_Status, Radiator_SN, Radiator_Status, Mfs_SN, mfs_Status, ReworkCode)
                VALUES (@StationId, @Timestamp, @SN, @SN_Status, @Radiator_SN, @Radiator_Status, @Mfs_SN, @mfs_Status, @ReworkCode);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Otwieramy transakcję – jeśli cokolwiek pójdzie nie tak, baza wycofa zmiany (Rollback)
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    // KROK 1: Szukamy wpisu ze Stacji 1
                    long? latestLogId = connection.QueryFirstOrDefault<long?>(sqlFindLatest, new { RadiatorSn = radiatorSn }, transaction);

                    if (latestLogId == null)
                    {
                        stationLog.Warning("S{Id} [SQL]: Not found Radiator_SN: {Sn} on Station 1. Rework not possible", stationIdentity, radiatorSn);
                        return (int)PlcTaskStatus.SQLEmptyResultsData;
                    }
                    else
                    {
                        // KROK 2: Aktualizacja logu produkcyjnego Stacji 1
                        connection.Execute(sqlUpdateProduction, new { ReworkStatus = reworkStatus, LogId = latestLogId.Value }, transaction);
                        stationLog.Information("S{Id} [SQL]: Updated log ID {LogId} with status {Status}.", stationIdentity, latestLogId.Value, reworkStatus);
                    }

                    // KROK 3: Wstawienie rekordu do tabeli StationRework
                    // Uwaga: Ponieważ z PLC odczytujesz głównie Radiator_SN, pola SN oraz Mfs_SN inicjalizujemy tymczasowo pustym ciągiem lub wartością domyślną
                    connection.Execute(sqlInsertRework, new
                    {
                        StationId = currentStationId,
                        Timestamp = DateTime.Now,
                        SN = radiatorSn, // lub inna zmienna jeśli PLC ją wystawia
                        SN_Status = reworkStatus,
                        Radiator_SN = radiatorSn,
                        Radiator_Status = reworkStatus,
                        Mfs_SN = string.Empty,
                        mfs_Status = 0,
                        ReworkCode = reworkCode
                    }, transaction);

                    // Zatwierdzamy transakcję w bazie danych
                    transaction.Commit();
                    stationLog.Information("S{Id} [SQL]: Successfully completed rework operation for SN: {Sn}", stationIdentity, radiatorSn);
                    return (int)PlcTaskStatus.Success;
                }
            }
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL] Critical error during rework transaction for SN: {Sn}", stationIdentity, radiatorSn);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
    }

}

  