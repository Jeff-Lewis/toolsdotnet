﻿using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;

namespace Tools.Commands.Implementation
{
    internal static class Log
    {
        private static readonly TraceSource traceSource =
            new TraceSource((typeof(Log).Assembly.GetName().Name));
        private static readonly TraceSource dbTraceSource =
    new TraceSource((typeof(Log).Assembly.GetName().Name + ".DB"));

        internal static TraceSource Source
        {
            get { return traceSource; }
        }
        internal static TraceSource DBSource
        {
            get { return dbTraceSource; }
        }

        internal static void TraceData(TraceSource source, TraceEventType eventType,
                                       Enum eventId, object data)
        {
            try
            {
                source.TraceData(eventType, Convert.ToInt32(eventId), data);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString()); // will get into standard output then
                // this is the lowest fallback possible (SD)
            }

        }
        internal static void TraceData(TraceSource traceSource, TraceEventType traceEventType, int p, string data)
        {
            traceSource.TraceData(traceEventType, p, data);
        }

    }
}