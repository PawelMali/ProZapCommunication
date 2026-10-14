using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;

namespace ProZapCommunication.Core.Logic.StationLogics;

public class Station7Processor : StationProcessorBase
{
    private readonly string _resultsTagName;


    public Station7Processor(int stationId, IDatabaseRepository db, IPlcProvider plc, string stationIdentity, string resultsTagName)
                : base(stationId, db, plc, stationIdentity, new Station7WriteBaseModel()) // Przekazujemy inteligentny interfejs PLC do bazy
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
            Station7ResultsData results = Plc.ReadStation7ResultsData(_resultsTagName);

            // Zapis do bazy
            CurrentWriteState.TaskStatus = DbRepository.SaveProductionStation7Data(StationIdentity, StationId, results);

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
            int currentResult = DbRepository.CheckMotorSnCurrentStationStatus(StationIdentity, StationId, CurrentReadState.DMC);
            int previousResult = DbRepository.CheckMotorSnPreviousStationStatus(StationIdentity, 5, CurrentReadState.DMC);
            int previousResult2 = DbRepository.CheckMotorSnPreviousStationStatus(StationIdentity, 6, CurrentReadState.DMC);

            if (CurrentWriteState is Station7WriteBaseModel s7WriteBase)
            {
                s7WriteBase.BalancingStatus = previousResult2;
            }


            bool isCurrentStationOk = currentResult == 1 || (currentResult >= 3500 && currentResult <= 3699);
            bool isPreviousStationOk = previousResult == 1;

            if (!isCurrentStationOk)
            {
                CurrentWriteState.TaskStatus = currentResult;
                return false;
            }

            if (!isPreviousStationOk)
            {
                CurrentWriteState.TaskStatus = previousResult;
                return false;
            }

            CurrentWriteState.TaskStatus =
                currentResult >= 3500 && currentResult <= 3699
                    ? currentResult
                    : 1;

            return true;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 2]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }

    public void WriteBaseMenagment()
    {
        if (CurrentWriteState is Station7WriteBaseModel s7WriteBase)
        {
            if (CurrentWriteState.TaskConfirmToPLC == 0 && CurrentReadState.TaskSendToPc == 0)
            {
                s7WriteBase.BalancingStatus = 0;
            }
        }
    }
}
