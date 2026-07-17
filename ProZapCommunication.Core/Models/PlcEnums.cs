using System;
using System.Collections.Generic;
using System.Text;

namespace ProZapCommunication.Core.Models;

public enum PlcTaskType : short
{
    None = 0,
    SaveResults = 1,
    VerifySN = 2,
    VerifyRadiator = 3,
    VerifyMfs = 4,
    ReworkSn = 5,
    ReworkMfsSn = 6,
    ReworkMotorSn = 7,
    ReworkRadiatorSn = 8,
    ReworkCheckDiskSpace = 9
}

public enum PlcTaskStatus : short
{
    Empty = 0,
    Success = 1,
    GeneralError = 2,
    MissingProcessData = 4,
    InvalidParameters = 5,
    HardDriveSpaceFailed = 6,


    SQLEmptyResultsData = 50,
    SQLEmptyResultsDMC = 51,
    SQLExecutionError = 52,
    SQLConnectionError = 53,
    SQLUnknownError = 54,
    SQLReworkEmptyStatus = 55,
    SQLReworkUnknownStatus = 56,
    SQLReworkEmptyDMC = 57,
    SQLReworkDMCNBotFound = 58,

    PartAlreadyDoneOK = 60,
    PartAlreadyDoneNOK = 61,
    PartAlreadyDoneSCRAP = 62,
    PartAlreadyDoneUNKNOWN = 63,

    PreviousPartNotFound = 64,
    PreviousPartNOK = 65,
    PreviousPartNotReworked = 66,

    HardDriveSpaceStart = 1000,
    HardDriveSpaceEnd = 1999,



    ReworkSt1 = 3100,
    ReworkSt2 = 3200,
    ReworkSt3 = 3300,
    ReworkSt4 = 3400,
    ReworkSt5 = 3500,
    ReworkSt6 = 3600,
    ReworkSt7 = 3700,




    InvalidTaskCode = 9999


}