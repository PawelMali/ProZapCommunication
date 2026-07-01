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
}

public enum PlcTaskStatus : short
{
    Empty = 0,
    Success = 1,
    GeneralError = 2,
    MissingProcessData = 4,
    InvalidParameters = 5,


    SQLEmptyResultsData = 50,
    SQLEmptyResultsDMC = 51,
    SQLExecutionError = 52,
    SQLConnectionError = 53,
    SQLUnknownError = 54,

    PartAlreadyDoneOK = 60,
    PartAlreadyDoneNOK = 61,
    PartAlreadyDoneSCRAP = 62,
    PartAlreadyDoneUNKNOWN = 63,

    PreviousPartNotFound = 64,
    PreviousPartNOK = 65,
    PreviousPartNotReworked = 66,


    ReworkSt1 = 301,
    ReworkSt2 = 302,
    ReworkSt3 = 303,
    ReworkSt4 = 304,
    ReworkSt5 = 305,
    ReworkSt6 = 306,
    ReworkSt7 = 307,




    InvalidTaskCode = 999


}