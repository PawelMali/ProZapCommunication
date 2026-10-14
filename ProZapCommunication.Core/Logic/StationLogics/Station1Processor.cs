using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;
using Serilog;
using System.Reflection.Metadata.Ecma335;

namespace ProZapCommunication.Core.Logic.StationLogics;

public class Station1Processor : StationProcessorBase
{
    private readonly string _resultsTagName;


    public Station1Processor(int stationId, IDatabaseRepository db, IPlcProvider plc, string stationIdentity, string resultsTagName)
                : base(stationId, db, plc, stationIdentity, new StationWriteBaseModel()) // Przekazujemy inteligentny interfejs PLC do bazy
    {
        _resultsTagName = resultsTagName;
    }

    protected override bool ExecuteTask(int taskCode)
    {
        // Rzutujemy surowy int z PLC na nasz bezpieczny Enum z Core
        PlcTaskType currentTask = (PlcTaskType)taskCode;

        switch (currentTask)
        {
            case PlcTaskType.SaveResults: // --- REAKCJA NA TASK 1 ---
                return HandleSaveProductionResultsTask();

            case PlcTaskType.VerifySN: // --- REAKCJA NA TASK 2 ---
                return HandleVerifySNTask();

            case PlcTaskType.VerifyRadiator: // --- REAKCJA NA TASK 3 ---
                return HandleVerifySNRadiatorTask();

            default:
                Log.Warning("S{Id} [TASK {TaskCode}]: Received unknown task code", StationId, taskCode);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.InvalidTaskCode;
                return false;
        }
    }


    private bool HandleSaveProductionResultsTask()
    {
        Log.Information("S{Id} [TASK 1]: Saving results '{Tag}' from PLC...", StationId, _resultsTagName);

        try
        {
            Station1ResultsData results = Plc.ReadStation1ResultsData(_resultsTagName);

            // Zapis do bazy
            CurrentWriteState.TaskStatus = DbRepository.SaveProductionStation1Data(StationIdentity, StationId, results);

            return CurrentWriteState.TaskStatus == 1; 
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 1]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }

    private bool HandleVerifySNTask()
    {
        Log.Information("S{Id} [TASK 2]: Verifying SN code: {Dmc}", StationId, CurrentReadState.DMC);

        try
        {
            CurrentWriteState.TaskStatus = DbRepository.CheckSnCurrentStationStatus(StationIdentity, StationId, CurrentReadState.DMC);

            return CurrentWriteState.TaskStatus is 1 or >= 3100 and <= 3799;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 2]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }

    private bool HandleVerifySNRadiatorTask()
    {
        Log.Information("S{Id} [TASK 3]: Verifying SN code for Radiator: {Dmc}", StationId, CurrentReadState.DMC);

        try
        {
            CurrentWriteState.TaskStatus = DbRepository.CheckRadiatorSnStatus(StationIdentity, StationId, CurrentReadState.DMC);

            return CurrentWriteState.TaskStatus is 1 or >= 3100 and <= 3799;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 3]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }
}
