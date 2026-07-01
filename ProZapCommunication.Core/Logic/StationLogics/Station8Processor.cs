using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;

namespace ProZapCommunication.Core.Logic.StationLogics;

public class Station8Processor : StationProcessorBase
{
    private readonly string _resultsTagName;


    public Station8Processor(int stationId, IDatabaseRepository db, IPlcProvider plc, string stationIdentity, string resultsTagName)
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
            case PlcTaskType.ReworkRadiatorSn: // --- REAKCJA NA TASK 8 ---
                return HandleReworkRadiatorSn();



            default:
                Log.Warning("S{Id} [TASK {TaskCode}]: Received unknown task code", StationId, taskCode);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.InvalidTaskCode;
                return false;
        }
    }

    private bool HandleReworkRadiatorSn()
    {
        Log.Information("S{Id} [TASK 8]: Start rework Radiator_Sn {Sn} procedure.", StationId, CurrentReadState.DMC);

        // Bezpieczne rzutowanie na model posiadający pola DMC i Rework_Status
        if (CurrentReadState is Station8ReadBaseModel reworkRead)
        {
            string currentSn = reworkRead.DMC;
            int currentStatus = reworkRead.Rework_Status;
            int currentCode = 0; // Placeholder for rework code, can be extended if needed

            if (string.IsNullOrEmpty(currentSn))
            {
                Log.Warning("S{Id} [TASK 8]: Missing SN to check", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLEmptyResultsDMC; // Invalid Parameters status
                return false;
            }

            Log.Information("S{Id} [TASK 8]: Get Radiator SN form PLC {Sn}, Status: {Status}",
                StationId, currentSn, currentStatus);

            // Wywołanie zaimplementowanej w Kroku 3 metody transakcyjnej z repozytorium bazy danych
            int isDbSuccess = DbRepository.ProcessRadiatorRework(currentSn, currentStatus, currentCode, StationId, StationIdentity);

            if (isDbSuccess == 1)
            {
                Log.Information("S{Id} [TASK 8]: Rework Radiator SN {Sn} procedure completed successfully.", StationId, currentSn);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.Success; // No Error
                return true;
            }
            else
            {
                Log.Error("S{Id} [TASK 8]: Sql error", StationId);
                CurrentWriteState.TaskStatus = StationId; 
                return false;
            }
        }

        Log.Error("S{Id} [TASK 8]: Critical error - model structure is not of type ReworkHandshakeModelRead.", StationId);
        CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLUnknownError;
        return false;
    }
}
