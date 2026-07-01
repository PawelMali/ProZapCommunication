using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Read;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Write;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Mappers;

public static class LogixMapperExtensions
{
    // Rzutowanie struktury READ na model Core
    public static StationReadBaseModel ToCoreModel(this LogixStationReadBase plcData)
    {
        if (plcData == null) return null;

        return new StationReadBaseModel
        {
            Life = plcData.Life,
            TaskCounter = plcData.Task_Counter,
            TaskSendToPc = plcData.Task_Send_To_PC,
            TaskConfirmToPc = plcData.Task_Confirm_To_PC,
            TaskSuccessCounter = plcData.Task_Succes_Counter,
            TaskStatus = plcData.Task_Status,
            DMC = plcData.DMC.ToString()
        };
    }

    // Rzutowanie modelu Core na strukturę WRITE (przygotowanie do wysyłki do PLC)
    public static LogixStationWriteBase ToPlcStructure(this StationWriteBaseModel coreModel)
    {
        if (coreModel == null) return null;

        return new LogixStationWriteBase
        {
            Life = (short)coreModel.Life,
            Task_Counter = (ushort)coreModel.TaskCounter,
            // Tutaj mapujemy parametry sterujące wysyłane z PC:
            Task_Recived_From_PC = (short)coreModel.TaskSendToPLC,
            Task_Confirm_From_PC = (short)coreModel.TaskConfirmToPLC,
            Task_Succes_Counter = (ushort)coreModel.TaskSuccessCounter,
            Task_Status = (short)coreModel.TaskStatus
            // Rezerwy pozostają domyślnie 0
        };
    }

    #region Stacja 1

    public static Station1ResultsData ToCoreModel(this LogixStation1ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station1ResultsData
        {
            SN = plcData.SN.ToString(),
            SN_Status = plcData.SN_Status,
            Radiator_SN = plcData.Radiator_SN.ToString(),
            Radiator_Status = plcData.Radiator_Status,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 2
    public static Station2ResultsData ToCoreModel(this LogixStation2ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station2ResultsData
        {
            SN = plcData.SN.ToString(),
            SN_Status = plcData.SN_Status,
            MFS_SN = plcData.MFS_SN.ToString(),
            MFS_Status = plcData.MFS_Status,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 3
    public static Station3ResultsData ToCoreModel(this LogixStation3ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station3ResultsData
        {
            SN = plcData.DMC.ToString(),
            SN_Status = plcData.ResultCode,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 4
    public static Station4ResultsData ToCoreModel(this LogixStation4ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station4ResultsData
        {
            SN = plcData.DMC.ToString(),
            SN_Status = plcData.ResultCode,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 5 Mfs1
    public static Station5ResultsData ToCoreModel(this LogixStation5ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station5ResultsData
        {
            Motor_SN = plcData.Motor_Sn.ToString(),
            Motor_Status = plcData.Motor_Status,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 6 Balancing
    public static Station6ResultsData ToCoreModel(this LogixStation6ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station6ResultsData
        {
            Motor_SN = plcData.Motor_Sn.ToString(),
            Motor_Status = plcData.Motor_Status,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 7 Mfs3

    public static LogixStationWriteBase ToPlcStructure(this Station7WriteBaseModel coreModel)
    {
        if (coreModel == null) return null;

        return new LogixStationWriteBase
        {
            Life = (short)coreModel.Life,
            Task_Counter = (ushort)coreModel.TaskCounter,
            // Tutaj mapujemy parametry sterujące wysyłane z PC:
            Task_Recived_From_PC = (short)coreModel.TaskSendToPLC,
            Task_Confirm_From_PC = (short)coreModel.TaskConfirmToPLC,
            Task_Succes_Counter = (ushort)coreModel.TaskSuccessCounter,
            Task_Status = (short)coreModel.TaskStatus,
            AdditionalInt1 = (short)coreModel.BalancingStatus


            // Rezerwy pozostają domyślnie 0
        };
    }

    public static Station7ResultsData ToCoreModel(this LogixStation7ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station7ResultsData
        {
            Motor_SN = plcData.Motor_Sn.ToString(),
            Motor_Status = plcData.Motor_Status,
            Mfs_SN = plcData.Mfs_Sn.ToString(),
            Mfs_Status = plcData.Mfs_Status,
            CycleTime = plcData.CycleTime
        };
    }

    #endregion

    #region Stacja 8 Rework

    public static Station8ReadBaseModel ToCoreModel8(this LogixStationReadBase plcData)
    {
        if (plcData == null) return null;

        return new Station8ReadBaseModel
        {
            Life = plcData.Life,
            TaskCounter = plcData.Task_Counter,
            TaskSendToPc = plcData.Task_Send_To_PC,
            TaskConfirmToPc = plcData.Task_Confirm_To_PC,
            TaskSuccessCounter = plcData.Task_Succes_Counter,
            TaskStatus = plcData.Task_Status,
            DMC = plcData.DMC.ToString(),
            Rework_Status = plcData.Reserve1
        };
    }

    public static Station8ResultsData ToCoreModel(this LogixStation8ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station8ResultsData
        {
            Reserve1_SN = plcData.Reserve1_Sn.ToString(),
            Reserve1_Status = plcData.Reserve1_Status,
            Reserve2_SN = plcData.Reserve2_Sn.ToString(),
            Reserve2_Status = plcData.Reserve2_Status,
        };
    }

    #endregion
}
