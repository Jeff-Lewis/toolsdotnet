﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Common.Authorisation
{
    public interface ITokenVolatileDataProvider
    {
        string GetVolatileData();
    }
}
