using Dapper;
using Microsoft.Data.SqlClient;
using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Results;
using Serilog;
using System.Data;
using System.Net.NetworkInformation;

namespace ProZapCommunication.Infrastructure.Database;

public class SqlRepository : IDatabaseRepository
{
    private readonly string _connectionString;
    private readonly int _maxReworkAllowed; // Pole przechowujące limit z appconfig

    public SqlRepository(string connectionString, int maxReworkAllowed)
    {
        _connectionString = connectionString;
        _maxReworkAllowed = maxReworkAllowed;
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
                        (
                            StationId,
                            SN,
                            SN_Status,
                            Radiator_SN,
                            Radiator_Status,
                            CycleTime,
                            Timestamp,
                            Reference_Number,

                            Grommet_Left_Bottom_Presence_Cam_1_Status,
                            Grommet_Left_Bottom_Presence_Cam_1_Counter,
                            Grommet_Right_Bottom_Presence_Cam_2_Status,
                            Grommet_Right_Bottom_Presence_Cam_2_Counter,

                            Radiator_Left_Bottom_Grommet_Cam_1_Status,
                            Radiator_Left_Bottom_Grommet_Cam_1_Counter,
                            Radiator_Right_Bottom_Grommet_Cam_2_Status,
                            Radiator_Right_Bottom_Grommet_Cam_2_Counter,

                            Stonegrid_Bottom_Left_Bracket_Cam_3_Status,
                            Stonegrid_Bottom_Left_Bracket_Cam_3_Counter,
                            Stonegrid_Top_Left_Slider_Cam_4_Status,
                            Stonegrid_Top_Left_Slider_Cam_4_Counter,
                            Stonegrid_Bottom_Right_Bracket_Cam_5_Status,
                            Stonegrid_Bottom_Right_Bracket_Cam_5_Counter,
                            Stonegrid_Top_Right_Slider_Cam_6_Status,
                            Stonegrid_Top_Right_Slider_Cam_6_Counter,

                            Airduct_Left_Middle_Slider_Cam_4_Status,
                            Airduct_Left_Middle_Slider_Cam_4_Counter,
                            Airduct_Right_Middle_Slider_Cam_6_Status,
                            Airduct_Right_Middle_Slider_Cam_6_Counter,
                            Airduct_Left_Top_Bracket_Cam_7_Status,
                            Airduct_Left_Top_Bracket_Cam_7_Counter,
                            Airduct_Right_Top_Bracket_Cam_8_Status,
                            Airduct_Right_Top_Bracket_Cam_8_Counter,
                            Airduct_Left_Top_Balcony_Cam_9_Status,
                            Airduct_Left_Top_Balcony_Cam_9_Counter,
                            Airduct_Right_Top_Balcony_Cam_10_Status,
                            Airduct_Right_Top_Balcony_Cam_10_Counter,
                            Airduct_Middle_Bottom_Ridge_Cam_11_Status,
                            Airduct_Middle_Bottom_Ridge_Cam_11_Counter
                        )
                        VALUES
                        (
                            @StationId,
                            @SN,
                            @SN_Status,
                            @Radiator_SN,
                            @Radiator_Status,
                            @CycleTime,
                            @Timestamp,
                            @Reference_Number,

                            @Grommet_Left_Bottom_Presence_Cam_1_Status,
                            @Grommet_Left_Bottom_Presence_Cam_1_Counter,
                            @Grommet_Right_Bottom_Presence_Cam_2_Status,
                            @Grommet_Right_Bottom_Presence_Cam_2_Counter,

                            @Radiator_Left_Bottom_Grommet_Cam_1_Status,
                            @Radiator_Left_Bottom_Grommet_Cam_1_Counter,
                            @Radiator_Right_Bottom_Grommet_Cam_2_Status,
                            @Radiator_Right_Bottom_Grommet_Cam_2_Counter,

                            @Stonegrid_Bottom_Left_Bracket_Cam_3_Status,
                            @Stonegrid_Bottom_Left_Bracket_Cam_3_Counter,
                            @Stonegrid_Top_Left_Slider_Cam_4_Status,
                            @Stonegrid_Top_Left_Slider_Cam_4_Counter,
                            @Stonegrid_Bottom_Right_Bracket_Cam_5_Status,
                            @Stonegrid_Bottom_Right_Bracket_Cam_5_Counter,
                            @Stonegrid_Top_Right_Slider_Cam_6_Status,
                            @Stonegrid_Top_Right_Slider_Cam_6_Counter,

                            @Airduct_Left_Middle_Slider_Cam_4_Status,
                            @Airduct_Left_Middle_Slider_Cam_4_Counter,
                            @Airduct_Right_Middle_Slider_Cam_6_Status,
                            @Airduct_Right_Middle_Slider_Cam_6_Counter,
                            @Airduct_Left_Top_Bracket_Cam_7_Status,
                            @Airduct_Left_Top_Bracket_Cam_7_Counter,
                            @Airduct_Right_Top_Bracket_Cam_8_Status,
                            @Airduct_Right_Top_Bracket_Cam_8_Counter,
                            @Airduct_Left_Top_Balcony_Cam_9_Status,
                            @Airduct_Left_Top_Balcony_Cam_9_Counter,
                            @Airduct_Right_Top_Balcony_Cam_10_Status,
                            @Airduct_Right_Top_Balcony_Cam_10_Counter,
                            @Airduct_Middle_Bottom_Ridge_Cam_11_Status,
                            @Airduct_Middle_Bottom_Ridge_Cam_11_Counter
                        );";


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
                    Timestamp = DateTime.Now,
                    Reference_Number = data.Reference_Number,

                    Grommet_Left_Bottom_Presence_Cam_1_Status = data.Grommet_Left_Bottom_Presence_Cam_1_Status,
                    Grommet_Left_Bottom_Presence_Cam_1_Counter = data.Grommet_Left_Bottom_Presence_Cam_1_Counter,
                    Grommet_Right_Bottom_Presence_Cam_2_Status = data.Grommet_Right_Bottom_Presence_Cam_2_Status,
                    Grommet_Right_Bottom_Presence_Cam_2_Counter = data.Grommet_Right_Bottom_Presence_Cam_2_Counter,

                    Radiator_Left_Bottom_Grommet_Cam_1_Status = data.Radiator_Left_Bottom_Grommet_Cam_1_Status,
                    Radiator_Left_Bottom_Grommet_Cam_1_Counter = data.Radiator_Left_Bottom_Grommet_Cam_1_Counter,
                    Radiator_Right_Bottom_Grommet_Cam_2_Status = data.Radiator_Right_Bottom_Grommet_Cam_2_Status,
                    Radiator_Right_Bottom_Grommet_Cam_2_Counter = data.Radiator_Right_Bottom_Grommet_Cam_2_Counter,

                    Stonegrid_Bottom_Left_Bracket_Cam_3_Status = data.Stonegrid_Bottom_Left_Bracket_Cam_3_Status,
                    Stonegrid_Bottom_Left_Bracket_Cam_3_Counter = data.Stonegrid_Bottom_Left_Bracket_Cam_3_Counter,
                    Stonegrid_Top_Left_Slider_Cam_4_Status = data.Stonegrid_Top_Left_Slider_Cam_4_Status,
                    Stonegrid_Top_Left_Slider_Cam_4_Counter = data.Stonegrid_Top_Left_Slider_Cam_4_Counter,
                    Stonegrid_Bottom_Right_Bracket_Cam_5_Status = data.Stonegrid_Bottom_Right_Bracket_Cam_5_Status,
                    Stonegrid_Bottom_Right_Bracket_Cam_5_Counter = data.Stonegrid_Bottom_Right_Bracket_Cam_5_Counter,
                    Stonegrid_Top_Right_Slider_Cam_6_Status = data.Stonegrid_Top_Right_Slider_Cam_6_Status,
                    Stonegrid_Top_Right_Slider_Cam_6_Counter = data.Stonegrid_Top_Right_Slider_Cam_6_Counter,

                    Airduct_Left_Middle_Slider_Cam_4_Status = data.Airduct_Left_Middle_Slider_Cam_4_Status,
                    Airduct_Left_Middle_Slider_Cam_4_Counter = data.Airduct_Left_Middle_Slider_Cam_4_Counter,
                    Airduct_Right_Middle_Slider_Cam_6_Status = data.Airduct_Right_Middle_Slider_Cam_6_Status,
                    Airduct_Right_Middle_Slider_Cam_6_Counter = data.Airduct_Right_Middle_Slider_Cam_6_Counter,
                    Airduct_Left_Top_Bracket_Cam_7_Status = data.Airduct_Left_Top_Bracket_Cam_7_Status,
                    Airduct_Left_Top_Bracket_Cam_7_Counter = data.Airduct_Left_Top_Bracket_Cam_7_Counter,
                    Airduct_Right_Top_Bracket_Cam_8_Status = data.Airduct_Right_Top_Bracket_Cam_8_Status,
                    Airduct_Right_Top_Bracket_Cam_8_Counter = data.Airduct_Right_Top_Bracket_Cam_8_Counter,
                    Airduct_Left_Top_Balcony_Cam_9_Status = data.Airduct_Left_Top_Balcony_Cam_9_Status,
                    Airduct_Left_Top_Balcony_Cam_9_Counter = data.Airduct_Left_Top_Balcony_Cam_9_Counter,
                    Airduct_Right_Top_Balcony_Cam_10_Status = data.Airduct_Right_Top_Balcony_Cam_10_Status,
                    Airduct_Right_Top_Balcony_Cam_10_Counter = data.Airduct_Right_Top_Balcony_Cam_10_Counter,
                    Airduct_Middle_Bottom_Ridge_Cam_11_Status = data.Airduct_Middle_Bottom_Ridge_Cam_11_Status,
                    Airduct_Middle_Bottom_Ridge_Cam_11_Counter = data.Airduct_Middle_Bottom_Ridge_Cam_11_Counter
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
                        (
                            StationId,
                            SN,
                            SN_Status,
                            MFS_SN,
                            MFS_Status,
                            CycleTime,
                            Timestamp,
                            Reference_Number,

                            MFS_Left_Bottom_Balcony_Cam_1_Status,
                            MFS_Left_Bottom_Balcony_Cam_1_Counter,
                            MFS_Left_Middle_Guide_Cam_2_Status,
                            MFS_Left_Middle_Guide_Cam_2_Counter,
                            MFS_Left_Top_Bracket_Cam_3_Status,
                            MFS_Left_Top_Bracket_Cam_3_Counter,
                            MFS_Middle_Top_Ridge_Cam_4_Status,
                            MFS_Middle_Top_Ridge_Cam_4_Counter,
                            MFS_Right_Top_Bracket_Cam_5_Status,
                            MFS_Right_Top_Bracket_Cam_5_Counter,
                            MFS_Right_Middle_Guide_Cam_6_Status,
                            MFS_Right_Middle_Guide_Cam_6_Counter,
                            MFS_Right_Bottom_Balcony_Cam_7_Status,
                            MFS_Right_Bottom_Balcony_Cam_7_Counter,

                            Radiator_Left_Top_Grommet_Cam_8_Status,
                            Radiator_Left_Top_Grommet_Cam_8_Counter,
                            Radiator_Right_Top_Grommet_Cam_9_Status,
                            Radiator_Right_Top_Grommet_Cam_9_Counter,

                            Shipping_Caps_Disassembly_Status,
                            Shipping_Caps_Disassembly_Counter
                        )
                        VALUES
                        (
                            @StationId,
                            @SN,
                            @SN_Status,
                            @MFS_SN,
                            @MFS_Status,
                            @CycleTime,
                            @Timestamp,
                            @Reference_Number,

                            @MFS_Left_Bottom_Balcony_Cam_1_Status,
                            @MFS_Left_Bottom_Balcony_Cam_1_Counter,
                            @MFS_Left_Middle_Guide_Cam_2_Status,
                            @MFS_Left_Middle_Guide_Cam_2_Counter,
                            @MFS_Left_Top_Bracket_Cam_3_Status,
                            @MFS_Left_Top_Bracket_Cam_3_Counter,
                            @MFS_Middle_Top_Ridge_Cam_4_Status,
                            @MFS_Middle_Top_Ridge_Cam_4_Counter,
                            @MFS_Right_Top_Bracket_Cam_5_Status,
                            @MFS_Right_Top_Bracket_Cam_5_Counter,
                            @MFS_Right_Middle_Guide_Cam_6_Status,
                            @MFS_Right_Middle_Guide_Cam_6_Counter,
                            @MFS_Right_Bottom_Balcony_Cam_7_Status,
                            @MFS_Right_Bottom_Balcony_Cam_7_Counter,

                            @Radiator_Left_Top_Grommet_Cam_8_Status,
                            @Radiator_Left_Top_Grommet_Cam_8_Counter,
                            @Radiator_Right_Top_Grommet_Cam_9_Status,
                            @Radiator_Right_Top_Grommet_Cam_9_Counter,

                            @Shipping_Caps_Disassembly_Status,
                            @Shipping_Caps_Disassembly_Counter
                        );";

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
                    Timestamp = DateTime.Now,
                    Reference_Number = data.Reference_Number,

                    MFS_Left_Bottom_Balcony_Cam_1_Status = data.MFS_Left_Bottom_Balcony_Cam_1_Status,
                    MFS_Left_Bottom_Balcony_Cam_1_Counter = data.MFS_Left_Bottom_Balcony_Cam_1_Counter,
                    MFS_Left_Middle_Guide_Cam_2_Status = data.MFS_Left_Middle_Guide_Cam_2_Status,
                    MFS_Left_Middle_Guide_Cam_2_Counter = data.MFS_Left_Middle_Guide_Cam_2_Counter,
                    MFS_Left_Top_Bracket_Cam_3_Status = data.MFS_Left_Top_Bracket_Cam_3_Status,
                    MFS_Left_Top_Bracket_Cam_3_Counter = data.MFS_Left_Top_Bracket_Cam_3_Counter,
                    MFS_Middle_Top_Ridge_Cam_4_Status = data.MFS_Middle_Top_Ridge_Cam_4_Status,
                    MFS_Middle_Top_Ridge_Cam_4_Counter = data.MFS_Middle_Top_Ridge_Cam_4_Counter,
                    MFS_Right_Top_Bracket_Cam_5_Status = data.MFS_Right_Top_Bracket_Cam_5_Status,
                    MFS_Right_Top_Bracket_Cam_5_Counter = data.MFS_Right_Top_Bracket_Cam_5_Counter,
                    MFS_Right_Middle_Guide_Cam_6_Status = data.MFS_Right_Middle_Guide_Cam_6_Status,
                    MFS_Right_Middle_Guide_Cam_6_Counter = data.MFS_Right_Middle_Guide_Cam_6_Counter,
                    MFS_Right_Bottom_Balcony_Cam_7_Status = data.MFS_Right_Bottom_Balcony_Cam_7_Status,
                    MFS_Right_Bottom_Balcony_Cam_7_Counter = data.MFS_Right_Bottom_Balcony_Cam_7_Counter,

                    Radiator_Left_Top_Grommet_Cam_8_Status = data.Radiator_Left_Top_Grommet_Cam_8_Status,
                    Radiator_Left_Top_Grommet_Cam_8_Counter = data.Radiator_Left_Top_Grommet_Cam_8_Counter,
                    Radiator_Right_Top_Grommet_Cam_9_Status = data.Radiator_Right_Top_Grommet_Cam_9_Status,
                    Radiator_Right_Top_Grommet_Cam_9_Counter = data.Radiator_Right_Top_Grommet_Cam_9_Counter,

                    Shipping_Caps_Disassembly_Status = data.Shipping_Caps_Disassembly_Status,
                    Shipping_Caps_Disassembly_Counter = data.Shipping_Caps_Disassembly_Counter

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
                        (
                            StationId,
                            SN,
                            SN_Status,
                            CycleTime,
                            Timestamp,
                            Reference_Number,

                            LeakTest_Status,
                            LeakTest_Counter,
                            LeakTest_Method,
                            LeakTest_Program_Number,
                            LeakTest_Pressure,
                            LeakTest_Pressure_Drop,
                            LeakTest_Leak_Rate,
                            LeakTest_Temperature
                        )
                        VALUES
                        (
                            @StationId,
                            @SN,
                            @SN_Status,
                            @CycleTime,
                            @Timestamp,
                            @Reference_Number,

                            @LeakTest_Status,
                            @LeakTest_Counter,
                            @LeakTest_Method,
                            @LeakTest_Program_Number,
                            @LeakTest_Pressure,
                            @LeakTest_Pressure_Drop,
                            @LeakTest_Leak_Rate,
                            @LeakTest_Temperature
                        );";


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
                    Timestamp = DateTime.Now,

                    Reference_Number = data.Reference_Number,

                    LeakTest_Status = data.LeakTest_Status,
                    LeakTest_Counter = data.LeakTest_Counter,
                    LeakTest_Method = data.LeakTest_Method,
                    LeakTest_Program_Number = data.LeakTest_Program_Number,
                    LeakTest_Pressure = data.LeakTest_Pressure,
                    LeakTest_Pressure_Drop = data.LeakTest_Pressure_Drop,
                    LeakTest_Leak_Rate = data.LeakTest_Leak_Rate,
                    LeakTest_Temperature = data.LeakTest_Temperature

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
                        (
                            StationId,
                            SN,
                            SN_Status,
                            CycleTime,
                            Timestamp,
                            Reference_Number,

                            Shipping_Caps_Assembly_Status,
                            Shipping_Caps_Assembly_Counter,

                            Print_Counter,

                            DMC_Quality_Content_Status,
                            DMC_Quality_Content_Counter,
                            DMC_Quality_Grade_Status,
                            DMC_Quality_Grade_Counter,
                            DMC_Quality_Grade_Result,

                            DMC_OCR_Homologation_Part_Number_Status,
                            DMC_OCR_Homologation_Part_Number_Counter,
                            DMC_OCR_Supplier_Status,
                            DMC_OCR_Supplier_Counter,
                            DMC_OCR_Country_Of_Orgin_Status,
                            DMC_OCR_Country_Of_Orgin_Counter
                        )
                        VALUES
                        (
                            @StationId,
                            @SN,
                            @SN_Status,
                            @CycleTime,
                            @Timestamp,
                            @Reference_Number,

                            @Shipping_Caps_Assembly_Status,
                            @Shipping_Caps_Assembly_Counter,

                            @Print_Counter,

                            @DMC_Quality_Content_Status,
                            @DMC_Quality_Content_Counter,
                            @DMC_Quality_Grade_Status,
                            @DMC_Quality_Grade_Counter,
                            @DMC_Quality_Grade_Result,

                            @DMC_OCR_Homologation_Part_Number_Status,
                            @DMC_OCR_Homologation_Part_Number_Counter,
                            @DMC_OCR_Supplier_Status,
                            @DMC_OCR_Supplier_Counter,
                            @DMC_OCR_Country_Of_Orgin_Status,
                            @DMC_OCR_Country_Of_Orgin_Counter
                        );";

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
                    Timestamp = DateTime.Now,

                    Reference_Number = data.Reference_Number,

                    Shipping_Caps_Assembly_Status = data.Shipping_Caps_Assembly_Status,
                    Shipping_Caps_Assembly_Counter = data.Shipping_Caps_Assembly_Counter,

                    Print_Counter = data.Print_Counter,

                    DMC_Quality_Content_Status = data.DMC_Quality_Content_Status,
                    DMC_Quality_Content_Counter = data.DMC_Quality_Content_Counter,
                    DMC_Quality_Grade_Status = data.DMC_Quality_Grade_Status,
                    DMC_Quality_Grade_Counter = data.DMC_Quality_Grade_Counter,
                    DMC_Quality_Grade_Result = data.DMC_Quality_Grade_Result,

                    DMC_OCR_Homologation_Part_Number_Status = data.DMC_OCR_Homologation_Part_Number_Status,
                    DMC_OCR_Homologation_Part_Number_Counter = data.DMC_OCR_Homologation_Part_Number_Counter,
                    DMC_OCR_Supplier_Status = data.DMC_OCR_Supplier_Status,
                    DMC_OCR_Supplier_Counter = data.DMC_OCR_Supplier_Counter,
                    DMC_OCR_Country_Of_Orgin_Status = data.DMC_OCR_Country_Of_Origin_Status,
                    DMC_OCR_Country_Of_Orgin_Counter = data.DMC_OCR_Country_Of_Origin_Counter

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
                        (
                            StationId,
                            Motor_SN,
                            Motor_Status,
                            Timestamp,
                            CycleTime,
                            Reference_Number,

                            Motor_Inspection_Status,
                            Fan_Inspection_Status,
                            Motor_Inspection_Counter,
                            Fan_Inspection_Counter,

                            Screw1_Status,
                            Screw1_Counter,
                            Screw1_Program_Number,
                            Screw1_Angle,
                            Screw1_Torque,

                            Screw2_Status,
                            Screw2_Counter,
                            Screw2_Program_Number,
                            Screw2_Angle,
                            Screw2_Torque,

                            Screw3_Status,
                            Screw3_Counter,
                            Screw3_Program_Number,
                            Screw3_Angle,
                            Screw3_Torque
                        )
                        VALUES
                        (
                            @StationId,
                            @Motor_SN,
                            @Motor_Status,
                            @Timestamp,
                            @CycleTime,
                            @Reference_Number,

                            @Motor_Inspection_Status,
                            @Fan_Inspection_Status,
                            @Motor_Inspection_Counter,
                            @Fan_Inspection_Counter,

                            @Screw1_Status,
                            @Screw1_Counter,
                            @Screw1_Program_Number,
                            @Screw1_Angle,
                            @Screw1_Torque,

                            @Screw2_Status,
                            @Screw2_Counter,
                            @Screw2_Program_Number,
                            @Screw2_Angle,
                            @Screw2_Torque,

                            @Screw3_Status,
                            @Screw3_Counter,
                            @Screw3_Program_Number,
                            @Screw3_Angle,
                            @Screw3_Torque
                        );";



        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {
                    StationId = stationId,
                    Motor_SN = data.Motor_SN,
                    Motor_Status = data.Motor_Status,
                    Timestamp = DateTime.Now,
                    CycleTime = data.CycleTime,
                    Reference_Number = data.Reference_Number,

                    Motor_Inspection_Status = data.Motor_Inspection_Status,
                    Motor_Inspection_Counter = data.Motor_Inspection_Counter,
                    Fan_Inspection_Status = data.Fan_Inspection_Status,
                    Fan_Inspection_Counter = data.Fan_Inspection_Counter,

                    Screw1_Status = data.Screw1_Status,
                    Screw1_Counter = data.Screw1_Counter,
                    Screw1_Program_Number = data.Screw1_Program_Number,
                    Screw1_Angle = data.Screw1_Angle,
                    Screw1_Torque = data.Screw1_Torque,

                    Screw2_Status = data.Screw2_Status,
                    Screw2_Counter = data.Screw2_Counter,
                    Screw2_Program_Number = data.Screw2_Program_Number,
                    Screw2_Angle = data.Screw2_Angle,
                    Screw2_Torque = data.Screw2_Torque,

                    Screw3_Status = data.Screw3_Status,
                    Screw3_Counter = data.Screw3_Counter,
                    Screw3_Program_Number = data.Screw3_Program_Number,
                    Screw3_Angle = data.Screw3_Angle,
                    Screw3_Torque = data.Screw3_Torque

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
                        (
                            StationId,
                            Motor_SN,
                            Motor_Status,
                            Timestamp,
                            CycleTime,
                            Reference_Number,

                            Initial_static_unbalance,
                            Final_static_unbalance,
                            Runout,
                            Initial_moment_unbalance,
                            Final_moment_unbalance,
                            Chamber,
                            Initial_static_unbalance_angle,
                            Final_static_unbalance_angle,
                            Rpm,
                            [Current]
                        )
                        VALUES
                        (
                            @StationId,
                            @Motor_SN,
                            @Motor_Status,
                            @Timestamp,
                            @CycleTime,
                            @Reference_Number,

                            @Initial_static_unbalance,
                            @Final_static_unbalance,
                            @Runout,
                            @Initial_moment_unbalance,
                            @Final_moment_unbalance,
                            @Chamber,
                            @Initial_static_unbalance_angle,
                            @Final_static_unbalance_angle,
                            @Rpm,
                            @Current
                        );";


        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new
                {

                    StationId = stationId,

                    Motor_SN = data.Motor_SN,
                    Motor_Status = data.Motor_Status,
                    Timestamp = DateTime.Now,
                    CycleTime = data.CycleTime,
                    Reference_Number = data.Reference_Number,

                    Initial_static_unbalance = data.Initial_static_unbalance,
                    Final_static_unbalance = data.Final_static_unbalance,
                    Runout = data.Runout,
                    Initial_moment_unbalance = data.Initial_moment_unbalance,
                    Final_moment_unbalance = data.Final_moment_unbalance,
                    Chamber = data.Chamber,
                    Initial_static_unbalance_angle = data.Initial_static_unbalance_angle,
                    Final_static_unbalance_angle = data.Final_static_unbalance_angle,
                    Rpm = data.Rpm,
                    Current = data.Current

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
                        (
                            StationId,

                            Motor_SN,
                            Motor_Status,
                            Mfs_SN,
                            Mfs_Status,

                            Timestamp,
                            CycleTime,
                            Reference_Number,

                            Shraud_Status,
                            Shraud_Counter,
                            Cable_Status,
                            Cable_Counter,
                            Plug_Status,
                            Plug_Counter,
                            Shipping_Cap_Status,
                            Shipping_Cap_Counter,

                            Print_Counter,

                            DMC_Quality_Content_Status,
                            DMC_Quality_Content_Counter,
                            DMC_Quality_Grade_Status,
                            DMC_Quality_Grade_Counter,
                            DMC_Quality_Grade_Result,

                            DMC_OCR_Homologation_Part_Number_Status,
                            DMC_OCR_Homologation_Part_Number_Counter,
                            DMC_OCR_Supplier_Status,
                            DMC_OCR_Supplier_Counter,
                            DMC_OCR_Country_Of_Origin_Status,
                            DMC_OCR_Country_Of_Origin_Counter,

                            Screw1_Status,
                            Screw1_Counter,
                            Screw1_Program_Number,
                            Screw1_Angle,
                            Screw1_Torque,

                            Screw2_Status,
                            Screw2_Counter,
                            Screw2_Program_Number,
                            Screw2_Angle,
                            Screw2_Torque,

                            Screw3_Status,
                            Screw3_Counter,
                            Screw3_Program_Number,
                            Screw3_Angle,
                            Screw3_Torque
                        )
                        VALUES
                        (
                            @StationId,
                            @Motor_SN,
                            @Motor_Status,
                            @Mfs_SN,
                            @Mfs_Status,
                            @Timestamp,
                            @CycleTime,
                            @Reference_Number,

                            @Shraud_Status,
                            @Shraud_Counter,
                            @Cable_Status,
                            @Cable_Counter,
                            @Plug_Status,
                            @Plug_Counter,
                            @Shipping_Cap_Status,
                            @Shipping_Cap_Counter,

                            @Print_Counter,

                            @DMC_Quality_Content_Status,
                            @DMC_Quality_Content_Counter,
                            @DMC_Quality_Grade_Status,
                            @DMC_Quality_Grade_Counter,
                            @DMC_Quality_Grade_Result,

                            @DMC_OCR_Homologation_Part_Number_Status,
                            @DMC_OCR_Homologation_Part_Number_Counter,
                            @DMC_OCR_Supplier_Status,
                            @DMC_OCR_Supplier_Counter,
                            @DMC_OCR_Country_Of_Origin_Status,
                            @DMC_OCR_Country_Of_Origin_Counter,

                            @Screw1_Status,
                            @Screw1_Counter,
                            @Screw1_Program_Number,
                            @Screw1_Angle,
                            @Screw1_Torque,

                            @Screw2_Status,
                            @Screw2_Counter,
                            @Screw2_Program_Number,
                            @Screw2_Angle,
                            @Screw2_Torque,

                            @Screw3_Status,
                            @Screw3_Counter,
                            @Screw3_Program_Number,
                            @Screw3_Angle,
                            @Screw3_Torque
                        );";



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
                    Timestamp = DateTime.Now,
                    CycleTime = data.CycleTime,
                    Reference_Number = data.Reference_Number,

                    Shraud_Status = data.Shraud_Status,
                    Shraud_Counter = data.Shraud_Counter,
                    Cable_Status = data.Cable_Status,
                    Cable_Counter = data.Cable_Counter,
                    Plug_Status = data.Plug_Status,
                    Plug_Counter = data.Plug_Counter,
                    Shipping_Cap_Status = data.Shipping_Cap_Status,
                    Shipping_Cap_Counter = data.Shipping_Cap_Counter,

                    Print_Counter = data.Print_Counter,

                    DMC_Quality_Content_Status = data.DMC_Quality_Content_Status,
                    DMC_Quality_Content_Counter = data.DMC_Quality_Content_Counter,
                    DMC_Quality_Grade_Status = data.DMC_Quality_Grade_Status,
                    DMC_Quality_Grade_Counter = data.DMC_Quality_Grade_Counter,
                    DMC_Quality_Grade_Result = data.DMC_Quality_Grade_Result,

                    DMC_OCR_Homologation_Part_Number_Status = data.DMC_OCR_Homologation_Part_Number_Status,
                    DMC_OCR_Homologation_Part_Number_Counter = data.DMC_OCR_Homologation_Part_Number_Counter,
                    DMC_OCR_Supplier_Status = data.DMC_OCR_Supplier_Status,
                    DMC_OCR_Supplier_Counter = data.DMC_OCR_Supplier_Counter,
                    DMC_OCR_Country_Of_Origin_Status = data.DMC_OCR_Country_Of_Origin_Status,
                    DMC_OCR_Country_Of_Origin_Counter = data.DMC_OCR_Country_Of_Origin_Counter,

                    Screw1_Status = data.Screw1_Status,
                    Screw1_Counter = data.Screw1_Counter,
                    Screw1_Program_Number = data.Screw1_Program_Number,
                    Screw1_Angle = data.Screw1_Angle,
                    Screw1_Torque = data.Screw1_Torque,

                    Screw2_Status = data.Screw2_Status,
                    Screw2_Counter = data.Screw2_Counter,
                    Screw2_Program_Number = data.Screw2_Program_Number,
                    Screw2_Angle = data.Screw2_Angle,
                    Screw2_Torque = data.Screw2_Torque,

                    Screw3_Status = data.Screw3_Status,
                    Screw3_Counter = data.Screw3_Counter,
                    Screw3_Program_Number = data.Screw3_Program_Number,
                    Screw3_Angle = data.Screw3_Angle,
                    Screw3_Torque = data.Screw3_Torque

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

                    case >= 3100 and <= 3199:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 1 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3200 and <= 3299:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 2 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3300 and <= 3399:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 3 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3400 and <= 3499:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 4 -> REWORK",
                            stationId, sn);
                        return (int)status;

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

                    case >= 3100 and <= 3199:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 1 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3200 and <= 3299:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 2 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3300 and <= 3399:
                        stationLog.Debug("S{Id} [SQL]: SN {SN} Station 3 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3400 and <= 3499:
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

                    case >= 3500 and <= 3599:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station Mfs 1 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3600 and <= 3699:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station Balancing -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3700 and <= 3799:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station Mfs 3 -> REWORK",
                            stationId, sn);
                        return (int)status;

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

                    case >= 3500 and <= 3599:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station 5 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3600 and <= 3699:
                        stationLog.Debug("S{Id} [SQL]: Motor SN {Motor_SN} Station 6 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3700 and <= 3799:
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

                    case >= 3100 and <= 3199:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 1 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3200 and <= 3299:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 2 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3300 and <= 3399:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 3 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3400 and <= 3499:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 4 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3500 and <= 3599:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station Mfs 1 -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3600 and <= 3699:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station Balancing -> REWORK",
                            stationId, sn);
                        return (int)status;

                    case >= 3700 and <= 3799:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station Mfs 3 -> REWORK",
                            stationId, sn);
                        return (int)status;

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
        SELECT TOP 1 Motor_Status
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

                    case >= 3500 and <= 3599:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 5 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3600 and <= 3699:
                        stationLog.Debug("S{Id} [SQL]: Mfs SN {Mfs_SN} Station 6 not reworked-> NOK",
                            stationId, sn);
                        return (int)PlcTaskStatus.PreviousPartNotReworked;

                    case >= 3700 and <= 3799:
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

                    case >= 3100 and <= 3199:
                        stationLog.Debug("S{Id} [SQL]: Radiator SN {Radiator_SN} -> REWORK",
                            stationId, sn);
                        return (int)status;

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
                SELECT TOP 1 Id, SN 
                FROM Station1Results 
                WHERE StationId = 1 AND Radiator_SN = @RadiatorSn 
                ORDER BY Timestamp DESC;";

        // 2. Zapytanie aktualizujące znaleziony rekord ze Stacji 1
        const string sqlUpdateProduction = @"
                UPDATE Station1Results 
                SET Radiator_Status = @ReworkStatus, 
                    SN_Status = @ReworkCode 
                WHERE Id = @LogId;";

        // 3. Zapytanie wstawiające rekord do tabeli reworku
        const string sqlInsertRework = @"
                INSERT INTO Station8Results (StationId, Timestamp, SN, SN_Status, Radiator_SN, Radiator_Status, Mfs_SN, mfs_Status, ReworkCode)
                VALUES (@StationId, @Timestamp, @SN, @SN_Status, @Radiator_SN, @Radiator_Status, @Mfs_SN, @mfs_Status, @ReworkCode);";

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Otwieramy transakcję – jeśli cokolwiek pójdzie nie tak, baza wycofa zmiany (Rollback)
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    // WALIDACJA LIMITU REWORKÓW
                    if (IsReworkLimitExceeded(connection, radiatorSn, transaction))
                    {
                        stationLog.Warning("S{Id} [SQL]: Radiator SN: {Sn} exceeded max allowed reworks limit ({Limit}).", currentStationId, radiatorSn, _maxReworkAllowed);
                        return (int)PlcTaskStatus.SQLReworkMaxAllowed;
                    }

                    // KROK 1: Szukamy wpisu ze Stacji 1
                    var latestRecord = connection.QueryFirstOrDefault(sqlFindLatest, new { RadiatorSn = radiatorSn }, transaction);

                    if (latestRecord == null)
                    {
                        stationLog.Warning("S{Id} [SQL]: Not found Radiator_SN: {Sn} on Station 1. Rework not possible", currentStationId, radiatorSn);
                        return (int)PlcTaskStatus.SQLEmptyResultsData;
                    }
                    else
                    {
                        // KROK 2: Aktualizacja logu produkcyjnego Stacji 1
                        connection.Execute(sqlUpdateProduction, new { ReworkStatus = reworkCode, LogId = latestRecord.Id }, transaction);
                        stationLog.Information("S{Id} [SQL]: Updated log ID {LogId} with status {Status}.", stationIdentity, latestRecord.Id, reworkCode);
                    }

                    // KROK 3: Wstawienie rekordu do tabeli Station8Results
                    // Uwaga: Ponieważ z PLC odczytujesz głównie Radiator_SN, pola SN oraz Mfs_SN inicjalizujemy tymczasowo pustym ciągiem lub wartością domyślną
                    connection.Execute(sqlInsertRework, new
                    {
                        StationId = currentStationId,
                        Timestamp = DateTime.Now,
                        SN = latestRecord.SN,
                        SN_Status = 3100,
                        Radiator_SN = radiatorSn,
                        Radiator_Status = reworkCode,
                        Mfs_SN = string.Empty,
                        mfs_Status = 0,
                        ReworkCode = reworkCode
                    }, transaction);

                    // Zatwierdzamy transakcję w bazie danych
                    transaction.Commit();
                    stationLog.Information("S{Id} [SQL]: Successfully completed rework operation for Radiator SN: {Sn}", stationIdentity, radiatorSn);
                    return (int)PlcTaskStatus.Success;
                }
            }
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL] Critical error during rework transaction for Radiator SN: {Sn}", stationIdentity, radiatorSn);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
    }

    public int ProcessMotorRework(string motorSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity)
    {
        var stationLog = Log.ForContext("StationIdentity", stationIdentity.Replace(".", "_"));

        bool updateStatus = false;

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    // WALIDACJA LIMITU REWORKÓW
                    if (IsReworkLimitExceeded(connection, motorSn, transaction))
                    {
                        stationLog.Warning("S{Id} [SQL]: Motor SN: {Sn} exceeded max allowed reworks limit ({Limit}).", currentStationId, motorSn, _maxReworkAllowed);
                        return (int)PlcTaskStatus.SQLReworkMaxAllowed;
                    }

                    // Flagi określające, które stacje wymagają aktualizacji na podstawie parametrów biznesowych
                    bool updateSt5 = false;
                    bool updateSt6 = false;
                    bool updateSt7 = false;

                    // ANALIZA STATUSU REWORKU
                    if (reworkCode == 2 || reworkCode == 4 || (reworkCode >= 3500 && reworkCode <= 3599))
                    {
                        updateSt5 = true;
                        updateSt6 = true;
                        updateSt7 = true;
                    }
                    else if (reworkCode >= 3600 && reworkCode <= 3699)
                    {
                        updateSt6 = true;
                        updateSt7 = true;
                    }
                    else if (reworkCode >= 3700 && reworkCode <= 3799)
                    {
                        updateSt7 = true;
                    }
                    else// if (!updateSt5 && !updateSt6 && !updateSt7)
                    {
                        stationLog.Warning("S{Id} [SQL]: Received unsupported rework status: {Status}. Update of result tables skipped.", stationIdentity, reworkCode);
                        return (int)PlcTaskStatus.SQLReworkUnknownStatus;
                    }

                    // AKTUALIZACJA STACJI 5
                    if (updateSt5)
                    {
                        var status = UpdateLatestMotorResult(connection, transaction, "Station5Results", motorSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }

                    // AKTUALIZACJA STACJI 6
                    if (updateSt6)
                    {
                        var status = UpdateLatestMotorResult(connection, transaction, "Station6Results", motorSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }

                    // AKTUALIZACJA STACJI 7
                    if (updateSt7)
                    {
                        var status = UpdateLatestMotorResult(connection, transaction, "Station7Results", motorSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }

                    if (!updateStatus)
                    {
                        stationLog.Error("S{Id} [SQL] Not found Motor SN: {Sn} on any station", stationIdentity, motorSn);
                        return (int)PlcTaskStatus.SQLReworkDMCNBotFound;
                    }

                    // KROK OSTATNI: Wstawienie wpisu do tabeli historycznej Station8Results
                    const string sqlInsertRework = @"
                            INSERT INTO Station8Results (StationId, Timestamp, SN, SN_Status, Radiator_SN, Radiator_Status, Mfs_SN, mfs_Status, ReworkCode)
                            VALUES (@StationId, @Timestamp, @SN, @SN_Status, @Radiator_SN, @Radiator_Status, @Mfs_SN, @mfs_Status, @ReworkCode);";

                    connection.Execute(sqlInsertRework, new
                    {
                        StationId = currentStationId,
                        Timestamp = DateTime.Now,
                        SN = motorSn, // W tym przypadku głównym SN jest silnik
                        SN_Status = reworkCode,
                        Radiator_SN = string.Empty,
                        Radiator_Status = 0,
                        Mfs_SN = string.Empty,
                        Mfs_Status = 0,
                        Motor_SN = motorSn, // W tym przypadku głównym SN jest silnik
                        Motor_Status = reworkCode,
                        ReworkCode = reworkCode
                    }, transaction);

                    // Zatwierdzamy całą transakcję
                    transaction.Commit();
                    stationLog.Information("S{Id} [SQL]: Successfully completed rework operation for Motor SN: {Sn}", motorSn, reworkCode);
                    return (int)PlcTaskStatus.Success;
                }
            }
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL] Critical error during rework transaction for Motor SN: {Sn}", stationIdentity, motorSn);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
    }

    public int ProcessMfsRework(string mfsSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity)
    {
        var stationLog = Log.ForContext("StationIdentity", stationIdentity.Replace(".", "_"));

        var latestRecord = default(dynamic);

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    // WALIDACJA LIMITU REWORKÓW
                    if (IsReworkLimitExceeded(connection, mfsSn, transaction))
                    {
                        stationLog.Warning("S{Id} [SQL]: MFS SN: {Sn} exceeded max allowed reworks limit ({Limit}).", currentStationId, mfsSn, _maxReworkAllowed);
                        return (int)PlcTaskStatus.SQLReworkMaxAllowed;
                    }

                    // Warunek biznesowy: 2, 4 lub zakres od 3200 do 3299
                    if (reworkCode == 2 || reworkCode == 4 || (reworkCode >= 3200 && reworkCode <= 3299))
                    {
                        const string sqlFindLatest = @"
                                SELECT TOP 1 Id, SN  
                                FROM Station2Results 
                                WHERE Mfs_SN = @MfsSn 
                                ORDER BY Timestamp DESC;";

                        const string sqlUpdate = @"
                                UPDATE Station2Results 
                                SET Mfs_Status = @Status,
                                    SN_Status = @Status    
                                WHERE Id = @Id;";

                        latestRecord = connection.QueryFirstOrDefault(sqlFindLatest, new { MfsSn = mfsSn }, transaction);

                        if (latestRecord == null)
                        {
                            stationLog.Warning("S{Id} [SQL]: Not found Mfs_SN: {Sn} on Station 2. Rework not possible", currentStationId, mfsSn);
                            return (int)PlcTaskStatus.SQLEmptyResultsData;
                        }
                        else
                        {
                            connection.Execute(sqlUpdate, new { Status = reworkCode, Id = latestRecord.Id }, transaction);
                            stationLog.Information("S{Id} [SQL]: Updated log ID {LogId} with status {Status}.", stationIdentity, latestRecord.Id, reworkCode);
                        }
                    }
                    else
                    {
                        stationLog.Warning("S{Id} [SQL]: Received unsupported rework status: {Status}. Update of result tables skipped.", stationIdentity, reworkCode);
                        return (int)PlcTaskStatus.SQLReworkUnknownStatus;
                    }

                    // KROK OSTATNI: Wstawienie wpisu do tabeli historycznej Station8Results
                    const string sqlInsertRework = @"
                            INSERT INTO Station8Results (StationId, Timestamp, SN, SN_Status, Radiator_SN, Radiator_Status, Mfs_SN, mfs_Status, ReworkCode)
                            VALUES (@StationId, @Timestamp, @SN, @SN_Status, @Radiator_SN, @Radiator_Status, @Mfs_SN, @mfs_Status, @ReworkCode);";

                    connection.Execute(sqlInsertRework, new
                    {
                        StationId = currentStationId,
                        Timestamp = DateTime.Now,
                        SN = latestRecord.SN,
                        SN_Status = reworkCode,
                        Radiator_SN = string.Empty,
                        Radiator_Status = 0,
                        Mfs_SN = mfsSn,
                        mfs_Status = reworkCode,
                        ReworkCode = reworkCode
                    }, transaction);

                    transaction.Commit();
                    stationLog.Information("S{Id} [SQL]: Successfully completed rework operation for MFS SN: {Sn}", stationIdentity, mfsSn);
                    return (int)PlcTaskStatus.Success;
                }
            }
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL] Critical error during rework transaction for MFS SN: {Sn}", stationIdentity, mfsSn);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
    }

    public int ProcessMainSnRework(string mainSn, int reworkStatus, int reworkCode, int currentStationId, string stationIdentity)
    {
        var stationLog = Log.ForContext("StationIdentity", stationIdentity.Replace(".", "_"));

        bool updateStatus = false;

        try
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    // WALIDACJA LIMITU REWORKÓW
                    if (IsReworkLimitExceeded(connection, mainSn, transaction))
                    {
                        stationLog.Warning("S{Id} [SQL]: Main SN: {Sn} exceeded max allowed reworks limit ({Limit}).", currentStationId, mainSn, _maxReworkAllowed);
                        return (int)PlcTaskStatus.SQLReworkMaxAllowed;
                    }

                    bool updateSt1 = false;
                    bool updateSt2 = false;
                    bool updateSt3 = false;
                    bool updateSt4 = false;

                    // ANALIZA STATUSU REWORKU GŁÓWNEGO SN
                    if (reworkCode == 2 || reworkCode == 4 || (reworkCode >= 3100 && reworkCode <= 3199))
                    {
                        updateSt1 = true;
                        updateSt2 = true;
                        updateSt3 = true;
                        updateSt4 = true;
                    }
                    else if (reworkCode >= 3200 && reworkCode <= 3299)
                    {
                        updateSt2 = true;
                        updateSt3 = true;
                        updateSt4 = true;
                    }
                    else if (reworkCode >= 3300 && reworkCode <= 3399)
                    {
                        updateSt3 = true;
                        updateSt4 = true;
                    }
                    else if (reworkCode >= 3400 && reworkCode <= 3499)
                    {
                        updateSt4 = true;
                    }
                    else// if (!updateSt5 && !updateSt6 && !updateSt7)
                    {
                        stationLog.Warning("S{Id} [SQL]: Received unsupported rework status: {Status}. Update of result tables skipped.", stationIdentity, reworkCode);
                        return (int)PlcTaskStatus.SQLReworkUnknownStatus;
                    }

                    // AKTUALIZACJA POSZCZEGÓLNYCH STACJI (Wyszukiwanie po kolumnie SN, aktualizacja SN_Status)
                    if (updateSt1)
                    {
                        var status = UpdateLatestMainSnResult(connection, transaction, "Station1Results", mainSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }
                    if (updateSt2)
                    {
                        var status = UpdateLatestMainSnResult(connection, transaction, "Station2Results", mainSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }
                    if (updateSt3)
                    {
                        var status = UpdateLatestMainSnResult(connection, transaction, "Station3Results", mainSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }
                    if (updateSt4)
                    {
                        var status = UpdateLatestMainSnResult(connection, transaction, "Station4Results", mainSn, reworkCode, stationLog);
                        if (status)
                            updateStatus = true;
                    }

                    if (!updateStatus)
                    {
                        stationLog.Error("S{Id} [SQL] Not found Main_SN: {Sn} on any station", stationIdentity, mainSn);
                        return (int)PlcTaskStatus.SQLReworkDMCNBotFound;
                    }

                    // KROK OSTATNI: Zapis historyczny do tabeli Station8Results
                    const string sqlInsertRework = @"
                            INSERT INTO Station8Results (StationId, Timestamp, SN, SN_Status, Radiator_SN, Radiator_Status, Mfs_SN, mfs_Status, ReworkCode)
                            VALUES (@StationId, @Timestamp, @SN, @SN_Status, @Radiator_SN, @Radiator_Status, @Mfs_SN, @mfs_Status, @ReworkCode);";

                    connection.Execute(sqlInsertRework, new
                    {
                        StationId = currentStationId,
                        Timestamp = DateTime.Now,
                        SN = mainSn,
                        SN_Status = reworkCode,
                        Radiator_SN = string.Empty,
                        Radiator_Status = 0,
                        Mfs_SN = string.Empty,
                        mfs_Status = 0,
                        ReworkCode = reworkCode
                    }, transaction);

                    transaction.Commit();
                    stationLog.Information("S{Id} [SQL]: Successfully completed rework operation for Main SN: {Sn}", currentStationId, mainSn);
                    return (int)PlcTaskStatus.Success;
                }
            }
        }
        catch (Exception ex)
        {
            stationLog.Error(ex, "S{Id} [SQL] Critical error during rework transaction for Main SN: {Sn}", stationIdentity, mainSn);
            return (int)PlcTaskStatus.SQLExecutionError;
        }
    }


    private bool UpdateLatestMainSnResult(IDbConnection conn, IDbTransaction trans, string tableName, string mainSn, int status, ILogger logger)
    {
        string sqlFindLatest = $@"
                SELECT TOP 1 Id 
                FROM {tableName} 
                WHERE SN = @MainSn 
                ORDER BY Timestamp DESC;";

        string sqlUpdate = $@"
                UPDATE {tableName} 
                SET SN_Status = @Status 
                WHERE Id = @Id;";

        long? latestId = conn.QueryFirstOrDefault<long?>(sqlFindLatest, new { MainSn = mainSn }, trans);

        if (latestId != null)
        {
            conn.Execute(sqlUpdate, new { Status = status, Id = latestId.Value }, trans);
            logger.Debug("S{Id} [SQL]: Updated latest record ID {Id} in table {Table} with status {Status}.", latestId.Value, tableName, status);
            return true;
        }
        else
        {
            logger.Warning("S{Id} [SQL]: No entry found for SN: {Sn} in table {Table}.", mainSn, tableName);
            return false;
        }
    }

    private bool UpdateLatestMotorResult(IDbConnection conn, IDbTransaction trans, string tableName, string motorSn, int ReworkCode, ILogger logger)
    {
        // Dynamicznie wstrzykujemy nazwę tabeli. Bezpieczne, bo nazwa pochodzi w 100% z naszego zaufanego kodu (sztywne stringi powyżej)
        string sqlFindLatest = $@"
                SELECT TOP 1 Id 
                FROM {tableName} 
                WHERE Motor_SN = @MotorSn 
                ORDER BY Timestamp DESC;";


        string setClause = "Motor_Status = @Status";

        if (tableName == "Station7Results")
            setClause += ", Mfs_Status = @Status";

        string sqlUpdate = $@"
                UPDATE {tableName}
                SET {setClause}
                WHERE Id = @Id;";


        long? latestId = conn.QueryFirstOrDefault<long?>(sqlFindLatest, new { MotorSn = motorSn }, trans);

        if (latestId != null)
        {
            conn.Execute(sqlUpdate, new { Status = ReworkCode, Id = latestId.Value }, trans);
            logger.Debug("S{Id} [SQL]: Updated latest record ID {Id} in table {Table} with status {Status}.", latestId.Value, tableName, ReworkCode);
            return true;
        }
        else
        {
            logger.Warning("S{Id} [SQL]: No entry found for Motor_SN: {Sn} in table {Table}.", motorSn, tableName);
            return false;
        }
    }

    private bool IsReworkLimitExceeded(IDbConnection connection, string sn, IDbTransaction transaction = null)
    {
        const string sqlCount = @"
        SELECT COUNT(1) 
        FROM Station8Results 
        WHERE SN = @SN OR Radiator_SN = @SN OR Mfs_SN = @SN;";

        int currentReworkCount = connection.ExecuteScalar<int>(sqlCount, new { SN = sn }, transaction);

        return currentReworkCount >= _maxReworkAllowed;
    }

}

