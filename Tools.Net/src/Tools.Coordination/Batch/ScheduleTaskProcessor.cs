using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using Tools.Coordination.Scheduling;
using Tools.Core.Context;
using Tools.Processes.Core;

namespace Tools.Coordination.Batch
{
    /// <summary>
    /// Summary description for QueueWorkItemsProducer.
    /// </summary>
    public abstract class ScheduleTaskProcessor : ThreadedProcess
    {
        #region Fields

        private readonly ContextIdentifier _contextIdentifier =
            new ContextIdentifier();
        private int iterationsCounter;

        #endregion

        #region Properties

        protected ContextIdentifier ContextIdentifier
        {
            get { return _contextIdentifier; }
        }

        protected virtual Schedule Schedule
        {
            get;
            set;
        }
        public int MaxCountOfIterations { get; set; }

        public int IterationsCounter { get { return iterationsCounter; } }

        #endregion Properties

        protected override void OnStopped()
        {
            // TODO: Think about placement of base Process class,
            // it can be prefferable to have it lower as it gets in
            // the architecture, on the other side it can provide default logging;
            // can represent the need for delegates use then. Or logging can be located in the utility (SD)
            Log.TraceData(Log.Source,TraceEventType.Stop,
                                 ScheduleTaskProcessorMessage.Stopped,
                                 new ContextualLogEntry
                                     {
                                         Message =
                                             string.Format
                                             (
                                             "{0} process is stopped. Stopped event is about to be raised.",
                                             Name
                                             ),
                                         ContextIdentifier = _contextIdentifier
                                     });

            base.OnStopped();
        }

        #region Methods

        /// <summary>
        /// This method is for child classes to provide more startup info if required
        /// </summary>
        protected virtual string GetStartupInfo()
        {
            return String.Empty;
        }

        protected override void StartInternal()
        {
            try
            {
                Trace.CorrelationManager.ActivityId = _contextIdentifier.ContextGuid;

                Log.Source.TraceTransfer(0, Name, _contextIdentifier.ContextGuid);

                Log.TraceData(Log.Source,TraceEventType.Start,
                                     ScheduleTaskProcessorMessage.Started,
                                     new ContextualLogEntry
                                         {
                                             Message =
                                                 string.Format
                                                 (
                                                 "'{0}': Started. {1}",
                                                 Name, GetStartupInfo()
                                                 ),
                                             ContextIdentifier = _contextIdentifier
                                         });

                //TODO: (SD) This to be subject to configure
                Schedule.SetForImmidiateRun();

                while (true)
                {
                    #region instrumentation

                    TimeSpan waitTime = Schedule.TimeDiff2Run;

                    DateTime nextScheduledTime = DateTime.UtcNow.Add(waitTime);

                    Log.TraceData(Log.Source,TraceEventType.Verbose,
                                         ScheduleTaskProcessorMessage.SuspendingTheProcessUntilTheNextRun,
                                         new ContextualLogEntry
                                             {
                                                 Message =
                                                     string.Format
                                                     (
                                                     "'{0}': Suspending the process for ({1}ms) until the iteration run at {2}",
                                                     Name,
                                                     waitTime.TotalMilliseconds,
                                                     nextScheduledTime.ToString("dd-MMM-yy HH:mm:ss (fff)")
                                                     ),
                                                 ContextIdentifier = _contextIdentifier
                                             });

                    #endregion instrumentation

                    if (MaxCountOfIterations != 0 && iterationsCounter++ == MaxCountOfIterations)
                    {
                        iterationsCounter = 0;
                        Log.TraceData(Log.Source, TraceEventType.Stop, ScheduleTaskProcessorMessage.Stopped,
                            String.Format("Scheduled task is stopped as maximum number of iterations is reached. Max number of iterations is {0} and can be setup as a property of a {1}:{2} class in configuration", MaxCountOfIterations, this.GetType().FullName, this.Name));
                        break;
                    }

                    SelfSuspend(waitTime);

                    if (ExecutionState != ProcessExecutionState.Running)
                    {
                        break;
                    }

                    #region ExecuteSheduleTask

                    try
                    {
                        Log.TraceData(Log.Source,TraceEventType.Verbose,
                             ScheduleTaskProcessorMessage.ScheduledTaskIsAboutToBeExecuted,
                             new ContextualLogEntry
                             {
                                 Message =
                                     String.Format("{0}: starting scheduled task execution (iteration #{1})", Name, iterationsCounter),
                                 ContextIdentifier = _contextIdentifier
                             });
                        ExecuteSheduleTask();
                    }
                    catch (Exception ex)
                    {
                        Log.TraceData(Log.Source,TraceEventType.Error,
                                             ScheduleTaskProcessorMessage.ErrorWhileExecutingScheduledTask,
                                             new ContextualLogEntry
                                                 {
                                                     Message =
                                                         "Error during " + Name + " scheduled task execution." + ex,
                                                     ContextIdentifier = _contextIdentifier
                                                 });
                    }
                    #endregion ExecuteSheduleTask

                    // (SD) In any case setup the next iteration, it is 
                    // subject for consideration how/if to implement an extension for 
                    // for failure resilience.
                    SetNextRunTime();
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.TraceData(Log.Source,TraceEventType.Verbose,
                                     ScheduleTaskProcessorMessage.ThreadInterrupted,
                                     new ContextualLogEntry
                                         {
                                             Message =
                                                 string.Format
                                                 (
                                                 "'{0}': Thread Interrupted.",
                                                 Name
                                                 ),
                                             ContextIdentifier = _contextIdentifier
                                         });
            }
            catch (ThreadAbortException)
            {
                Log.TraceData(Log.Source,TraceEventType.Error,
                                     ScheduleTaskProcessorMessage.AbortRequested,
                                     new ContextualLogEntry
                                         {
                                             Message =
                                                 string.Format
                                                 (
                                                 "'{0}': Abort requested.",
                                                 Name
                                                 ),
                                             ContextIdentifier = _contextIdentifier
                                         });
            }
            catch (Exception ex)
            {
                Log.TraceData(Log.Source,TraceEventType.Error,
                                     ScheduleTaskProcessorMessage.ErrorWhileExecutingScheduledTask,
                                     new ContextualLogEntry
                                         {
                                             Message =
                                                 string.Format
                                                 (
                                                 "'{0}': Error occured while processing the scheduled task." +
                                                 " Error desc.: {1}.",
                                                 Name,
                                                 ex
                                                 ),
                                             ContextIdentifier = _contextIdentifier
                                         });
            }

            Log.TraceData(Log.Source,TraceEventType.Stop,
                                 ScheduleTaskProcessorMessage.FinishingNormally,
                                 new ContextualLogEntry
                                     {
                                         Message =
                                             string.Format
                                             (
                                             "'{0}': Finishing normally",
                                             Name
                                             ),
                                         ContextIdentifier = _contextIdentifier
                                     });
        }

        protected virtual void SetNextRunTime()
        {
            Schedule.SetNextRunTime();
        }


        protected abstract void ExecuteSheduleTask();

        #endregion
    }
}