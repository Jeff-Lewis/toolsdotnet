﻿using System;
using System.Collections.Generic;

using System.Text;
using System.ServiceModel;

namespace Tools.Common.Authorisation
{
    [ServiceContract(Namespace = "http://Tools.Common/2008/02",
        Name = "ITokenProvider",
        SessionMode = SessionMode.Allowed)]
    public interface ITokenProvider
    {
        [OperationContract(IsTerminating = false, IsOneWay = false, AsyncPattern = false, Action = "IssueToken")]
        string IssueToken(string tokenSource);
    }
}
