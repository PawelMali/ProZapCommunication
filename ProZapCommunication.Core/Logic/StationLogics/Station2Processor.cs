using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;

namespace ProZapCommunication.Core.Logic.StationLogics;

public class Station2Processor : StationProcessorBase
{
    private readonly string _resultsTagName;


    public Station2Processor(int stationId, IDatabaseRepository db, IPlcProvider plc, string stationIdentity, string resultsTagName)
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

            case PlcTaskType.VerifyMfs: // --- REAKCJA NA TASK 4 ---
                return HandleVerifySNMfsTask();

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
            Station2ResultsData results = Plc.ReadStation2ResultsData(_resultsTagName);

            // Zapis do bazy
            CurrentWriteState.TaskStatus = DbRepository.SaveProductionStation2Data(StationIdentity, StationId, results);

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
            int currentResult = DbRepository.CheckSnCurrentStationStatus(StationIdentity, StationId, CurrentReadState.DMC);
            int previousResult = DbRepository.CheckSnPreviousStationStatus(StationIdentity, 1, CurrentReadState.DMC);

            if( currentResult != 1)
            {
                CurrentWriteState.TaskStatus = currentResult;
                return false;
            }

            if (previousResult != 1)
            {
                CurrentWriteState.TaskStatus = previousResult;
                return false;
            }

            if(currentResult == 1 && previousResult == 1)
            {
                CurrentWriteState.TaskStatus = 1;
                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 2]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }

    private bool HandleVerifySNMfsTask()
    {
        Log.Information("S{Id} [TASK 3]: Verifying SN code for MFS: {Dmc}", StationId, _resultsTagName, CurrentReadState.DMC);

        try
        {
            int currentResult = DbRepository.CheckMfsSnCurrentStationStatus(StationIdentity, StationId, CurrentReadState.DMC);
            int previousResult = DbRepository.CheckMfsSnPreviousStationStatus(StationIdentity, StationId, CurrentReadState.DMC);

            if (currentResult != 1)
            {
                CurrentWriteState.TaskStatus = currentResult;
                return false;
            }

            if (previousResult != 1)
            {
                CurrentWriteState.TaskStatus = previousResult;
                return false;
            }

            if (currentResult == 1 && previousResult == 1)
            {
                CurrentWriteState.TaskStatus = 1;
                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 3]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }
}
