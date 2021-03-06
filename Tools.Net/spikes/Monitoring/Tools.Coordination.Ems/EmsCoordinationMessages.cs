﻿using System;

namespace Tools.Coordination.Ems
{
    public enum EmsCoordinationMessages
    {
        None = 0,
        //Regular messages
        ListenerStartedSuccessfuly = 3000,
        WorkingInFailoverMode = 3001,
        MessageDispatchedByStub = 3002,
        MessageDispatchedByEmsWriter = 3003,
        ForceReconnectExecuted = 3004,

        //Error messages
        InvalidConfiguration = 3050,
        ErrorDuringEmsResourceCleanup = 3051,
        ErrorWhenTryingToOpenEmsReaderQueue = 3052,
        InvalidMessageType = 3053,
        CommitCalledOnTheClosedSession = 3054,
        RollbackCalledOnTheClosedSession = 3055,
    }
}