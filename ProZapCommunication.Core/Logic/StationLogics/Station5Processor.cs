using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;

namespace ProZapCommunication.Core.Logic.StationLogics;

public class Station5Processor : StationProcessorBase
{
    private readonly string _resultsTagName;


    public Station5Processor(int stationId, IDatabaseRepository db, IPlcProvider plc, string stationIdentity, string resultsTagName)
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
            Station5ResultsData results = Plc.ReadStation5ResultsData(_resultsTagName);

            // Zapis do bazy
            CurrentWriteState.TaskStatus = DbRepository.SaveProductionStation5Data(StationIdentity, StationId, results);

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
        Log.Information("S{Id} [TASK 2]: Verifying SN code: {Dmc}", StationId, _resultsTagName, CurrentReadState.DMC);

        try
        {
            CurrentWriteState.TaskStatus = DbRepository.CheckMotorSnCurrentStationStatus(StationIdentity, StationId, CurrentReadState.DMC);

            return CurrentWriteState.TaskStatus == 1;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 2]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }
}
