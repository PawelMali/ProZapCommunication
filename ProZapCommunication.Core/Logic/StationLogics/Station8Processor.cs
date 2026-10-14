using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;
using System.Security.Cryptography.X509Certificates;

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
            case PlcTaskType.ReworkSn: // --- REAKCJA NA TASK 5 ---
                return HandleReworkMainSn();

            case PlcTaskType.ReworkMfsSn: // --- REAKCJA NA TASK 6 ---
                return HandleReworkMfsSn();

            case PlcTaskType.ReworkMotorSn: // --- REAKCJA NA TASK 7 ---
                return HandleReworkMotorSn();

            case PlcTaskType.ReworkRadiatorSn: // --- REAKCJA NA TASK 8 ---
                return HandleReworkRadiatorSn();

            case PlcTaskType.ReworkCheckDiskSpace: // --- REAKCJA NA TASK 9 ---
                return HandleCheckDiskSpace();

            case PlcTaskType.ReworkCheckRadiator: // --- REAKCJA NA TASK 10 ---
                return HandleCheckRadiator();




            default:
                Log.Warning("S{Id} [TASK {TaskCode}]: Received unknown task code", StationId, taskCode);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.InvalidTaskCode;
                return false;
        }
    }

    private bool HandleReworkMainSn()
    {
        Log.Information("S{Id} [TASK 5]: Start rework MAIN_SN {Sn} procedure.", StationId);

        if (CurrentReadState is Station8ReadBaseModel reworkRead)
        {
            string currentSn = reworkRead.DMC;
            int currentCode = reworkRead.Rework_Code;

            int currentStatus = (currentCode >= 3000 && currentCode <= 3999)
                ? 300 + (currentCode - 3000) / 100
                : currentCode;

            if (string.IsNullOrEmpty(currentSn))
            {
                Log.Warning("S{Id} [TASK 5]: Missing MAIN_SN to check", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyDMC;
                return false;
            }

            if (currentStatus <= 0)
            {
                Log.Warning("S{Id} [TASK 5]: Missing Status rework to change", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyStatus;
                return false;
            }

            Log.Information("S{Id} [TASK 5]: Get MAIN_SN form PLC {Sn}, Status: {Status}, Code: {Code}",
                StationId, currentSn, currentStatus, currentCode);

            // Wywołanie metody transakcyjnej z repozytorium SQL
            var isDbSuccess = DbRepository.ProcessMainSnRework(currentSn, currentStatus, currentCode, StationId, StationIdentity);

            if (isDbSuccess == 1)
            {
                Log.Information("S{Id} [TASK 5]: Rework MAIN_SN {Sn} procedure completed successfully.", StationId, currentSn);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.Success; // No Error
                return true;
            }
            else
            {
                Log.Error("S{Id} [TASK 5]: Sql error {ErrorCode}", StationId, isDbSuccess);
                CurrentWriteState.TaskStatus = isDbSuccess;
                return false;
            }
        }

        Log.Error("S{Id} [TASK 5]: Critical error - model structure is not of type Station8ReadBaseModel.", StationId);
        CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLUnknownError;
        return false;
    }

    private bool HandleReworkMfsSn()
    {
        Log.Information("S{Id} [TASK 6]: Start rework MFS_SN {Sn} procedure.", StationId);

        if (CurrentReadState is Station8ReadBaseModel reworkRead)
        {
            string currentSn = reworkRead.DMC;
            int currentCode = reworkRead.Rework_Code;

            int currentStatus = (currentCode >= 3000 && currentCode <= 3999)
                ? 300 + (currentCode - 3000) / 100
                : currentCode;

            if (string.IsNullOrEmpty(currentSn))
            {
                Log.Warning("S{Id} [TASK 6]: Missing MFS_SN to check", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyDMC;
                return false;
            }

            if (currentStatus <= 0)
            {
                Log.Warning("S{Id} [TASK 6]: Missing Status rework to change", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyStatus;
                return false;
            }

            Log.Information("S{Id} [TASK 6]: Get MFS_SN form PLC {Sn}, Status: {Status}, Code: {Code}",
                StationId, currentSn, currentStatus, currentCode);

            // Wywołanie metody z interfejsu repozytorium SQL
            var isDbSuccess = DbRepository.ProcessMfsRework(currentSn, currentStatus, currentCode, StationId, StationIdentity);
            if (isDbSuccess == 1)
            {
                Log.Information("S{Id} [TASK 6]: Rework MFS_SN {Sn} procedure completed successfully.", StationId, currentSn);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.Success; // No Error
                return true;
            }
            else
            {
                Log.Error("S{Id} [TASK 6]: Sql error {ErrorCode}", StationId, isDbSuccess);
                CurrentWriteState.TaskStatus = isDbSuccess;
                return false;
            }
        }

        Log.Error("S{Id} [TASK 6]: Critical error - model structure is not of type Station8ReadBaseModel.", StationId);
        CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLUnknownError;
        return false;
    }

    private bool HandleReworkMotorSn()
    {
        Log.Information("S{Id} [TASK 7]: Start rework Motor_Sn {Sn} procedure.", StationId, CurrentReadState.DMC);

        if (CurrentReadState is Station8ReadBaseModel reworkRead)
        {
            string currentSn = reworkRead.DMC;
            int currentCode = reworkRead.Rework_Code; 

            int currentStatus = (currentCode >= 3000 && currentCode <= 3999)
                ? 300 + (currentCode - 3000) / 100
                : currentCode;

            if (string.IsNullOrEmpty(currentSn))
            {
                Log.Warning("S{Id} [TASK 7]: Missing Motor SN to check", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyDMC; 
                return false;
            }

            if (currentStatus <= 0)
            {
                Log.Warning("S{Id} [TASK 7]: Missing Status rework to change", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyStatus; 
                return false;
            }

            Log.Information("S{Id} [TASK 7]: Get Motor SN form PLC {Sn}, Status: {Status}, Code: {Code}",
                StationId, currentSn, currentStatus, currentCode);

            // Wywołanie naszej nowej metody z interfejsu repozytorium SQL
            var isDbSuccess = DbRepository.ProcessMotorRework(currentSn, currentStatus, currentCode, StationId, StationIdentity);

            if (isDbSuccess == 1)
            {
                Log.Information("S{Id} [TASK 7]: Rework Motor SN {Sn} procedure completed successfully.", StationId, currentSn);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.Success; // No Error
                return true;
            }
            else
            {
                Log.Error("S{Id} [TASK 7]: Sql error {ErrorCode}", StationId, isDbSuccess);
                CurrentWriteState.TaskStatus = isDbSuccess;
                return false;
            }
        }

        Log.Error("S{Id} [TASK 7]: Critical error - model structure is not of type Station8ReadBaseModel.", StationId);
        CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLUnknownError;
        return false;
    }

    private bool HandleReworkRadiatorSn()
    {
        Log.Information("S{Id} [TASK 8]: Start rework Radiator_Sn {Sn} procedure.", StationId, CurrentReadState.DMC);

        // Bezpieczne rzutowanie na model posiadający pola DMC i Rework_Status
        if (CurrentReadState is Station8ReadBaseModel reworkRead)
        {
            string currentSn = reworkRead.DMC;
            int currentCode = reworkRead.Rework_Code; // Placeholder for rework code, can be extended if needed

            int currentStatus = (currentCode >= 3000 && currentCode <= 3999)
                ? 300 + (currentCode - 3000) / 100
                : currentCode;




            if (string.IsNullOrEmpty(currentSn))
            {
                Log.Warning("S{Id} [TASK 8]: Missing Radiator SN to check", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyDMC; // Invalid Parameters status
                return false;
            }

            if (currentStatus <= 0)
            {
                Log.Warning("S{Id} [TASK 8]: Missing Status rework to change", StationId);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLReworkEmptyStatus; // Invalid Parameters status
                return false;
            }

            Log.Information("S{Id} [TASK 8]: Get Radiator SN form PLC {Sn}, Status: {Status}, Code: {Code}",
                StationId, currentSn, currentStatus, currentCode);

            // Wywołanie zaimplementowanej w Kroku 3 metody transakcyjnej z repozytorium bazy dany
            int isDbSuccess = DbRepository.ProcessRadiatorRework(currentSn, currentStatus, currentCode, StationId, StationIdentity);

            if (isDbSuccess == 1)
            {
                Log.Information("S{Id} [TASK 8]: Rework Radiator SN {Sn} procedure completed successfully.", StationId, currentSn);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.Success; // No Error
                return true;
            }
            else
            {
                Log.Error("S{Id} [TASK 8]: Sql error {ErrorCode}", StationId, isDbSuccess);
                CurrentWriteState.TaskStatus = isDbSuccess; 
                return false;
            }
        }

        Log.Error("S{Id} [TASK 8]: Critical error - model structure is not of type Station8ReadBaseModel.", StationId);
        CurrentWriteState.TaskStatus = (short)PlcTaskStatus.SQLUnknownError;
        return false;
    }

    private bool HandleCheckDiskSpace()
    {
        Log.Information("S{Id} [TASK 9]: Checking disk space...", StationId);

        // Jeśli ścieżka nie jest skonfigurowana, domyślnie sprawdzamy dysk aplikacji (np. C:\)
        string pathToAnalyze = string.IsNullOrEmpty(_visionDestPath) ? AppDomain.CurrentDomain.BaseDirectory : _visionDestPath;

        try
        {
            // Wyciągamy informację o dysku (np. "D:\Archiwum" -> drive name: "D:\")
            string driveRoot = Path.GetPathRoot(pathToAnalyze);
            DriveInfo drive = new DriveInfo(driveRoot);

            if (!drive.IsReady)
            {
                Log.Error("S{Id} [TASK 9]: Hard drive {Drive} is not ready or disconnected!", StationId, driveRoot);
                CurrentWriteState.TaskStatus = (short)PlcTaskStatus.HardDriveSpaceFailed; // Status błędu (np. błąd sprzętowy)
                return false;
            }

            long totalSpace = drive.TotalSize;
            long freeSpace = drive.AvailableFreeSpace;
            long usedSpace = totalSpace - freeSpace;

            // Obliczamy procentowe zajęcie dysku
            // Używamy rzutowania na double, aby uniknąć zaokrągleń całkowitoliczbowych do zera
            double usedPercent = ((double)usedSpace / totalSpace) * 100;
            short finalPercent = (short)(1000 + Math.Round(usedPercent * 10));

            // Zapisujemy wynik bezpośrednio do struktury WRITE – pętla 50ms natychmiast wyśle to do PLC
            CurrentWriteState.TaskStatus = finalPercent;

            Log.Information("S{Id} [TASK 9]: Analyzed disk {Drive} -> Used: {Percent}% (Free: {Free} GB / Total: {Total} GB), send to PLC {plc}",
                StationId,
                driveRoot,
                usedPercent,
                Math.Round((double)freeSpace / 1024 / 1024 / 1024, 1),
                Math.Round((double)totalSpace / 1024 / 1024 / 1024, 1),
                finalPercent);

            // Monitorowanie alarmowe w logach C#
            if (usedPercent >= 90)
            {
                Log.Fatal("S{Id} [TASK 9]: Disk usage exceeded 90%! Immediate action required.", StationId);
            }
            else if (usedPercent >= 80)
            {
                Log.Warning("S{Id} [TASK 9]: Disk usage exceeded 80%.", StationId);
            }


            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 9]: Unexpected error while checking disk space.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.HardDriveSpaceFailed;;
            return false;
        }
    }

    private bool HandleCheckRadiator()
    {
        Log.Information("S{Id} [TASK 10]: Verifying Radiator SN code: {Dmc}", StationId, CurrentReadState.DMC);

        int lastStation = 0;

        try
        {
           // CurrentWriteState.TaskStatus = DbRepository.CheckSnCurrentStationStatus(out lastStation CurrentReadState.DMC);

            return CurrentWriteState.TaskStatus is 1 or >= 3100 and <= 3799;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "S{Id} [TASK 10]: Error.", StationId);
            CurrentWriteState.TaskStatus = (short)PlcTaskStatus.GeneralError;
            return false;
        }
    }
}
