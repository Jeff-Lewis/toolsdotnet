﻿using System;
using Tools.Failover;
using TIBCO.EMS;
using Tools.Core.Asserts;
using System.Configuration;
using System.Diagnostics;
using Tools.Core.Configuration;

namespace Tools.Coordination.Ems
{
    public class EmsReaderQueue : EmsQueueBase, IDisposable 
    {
        private SessionConfiguration sessionConfig;
        public ServerConfiguration ServerConfig { get; private set; }
        private EmsReaderQueueConfiguration queueConfig;
        public EmsReaderQueueConfiguration QueueConfig { get { return queueConfig; } private set { queueConfig = value; } }

        private ConnectionFactory factory;
        private Connection connection;
        private Session session;
        private MessageConsumer consumer;
        private Destination destination;
        /// <summary>
        /// Zero is a special value meaning no timeout
        /// </summary>
        private long readTimeout = 0;

        private bool initialized;
        private IFailureExceptionHandler connectionFailureExceptionHandler =
            new DelayedFailureExceptionHandler(10000, 2, 30000);

        public IFailureExceptionHandler ExceptionHandler
        {
            get { return connectionFailureExceptionHandler; }
            set { connectionFailureExceptionHandler = value; }
        }

        public bool IsInitialized
        {
            get { return initialized; }
            set { initialized = value; }
        }

        public long ReadTimeout { get { return readTimeout; } set { readTimeout = value; } }

        public EmsReaderQueue(SessionConfiguration sessionConfig, ServerConfiguration serverConfig, EmsReaderQueueConfiguration queueConfig, IFailureExceptionHandler connectionFailureExceptionHandler)
        {
            this.sessionConfig = sessionConfig;
            ServerConfig = serverConfig;
            this.queueConfig = queueConfig;
            this.connectionFailureExceptionHandler = connectionFailureExceptionHandler;
        }

        public void Commit()
        {
            session.Commit();
        }
        public void Rollback()
        {
            session.Rollback();
        }

        public Message ReadNext()
        {
            return consumer.Receive(readTimeout);
        }

        public void Close()
        {
            initialized = false;
            string exText = null;

            try
            {
                try
                {
                    if (consumer != null) consumer.Close();
                }
                catch (Exception ex)
                {
                    exText += ex.ToString();
                }
                try
                {
                    if (connection != null) connection.Close();
                }
                catch (Exception ex)
                {
                    exText += ex.ToString();
                }
            }
            catch (Exception ex)
            {
                exText += ex.ToString();
            }

            Log.TraceData(Log.Source, TraceEventType.Error, EmsCoordinationMessages.ErrorDuringEmsResourceCleanup, exText);
        }
        /// <summary>
        /// Opens if is not initialized yet, otherwise just returns with no action.
        /// </summary>
        public void Open()
        {
            if (!initialized)
            {

                ValidateQueueConfiguration();

                try
                {
                    factory = new ConnectionFactory(ServerConfig.Url, ServerConfig.ClientId);
                }
                catch (EMSException e)
                {
                    Log.TraceData(Log.Source, TraceEventType.Error, 15000, "URL/Client ID is wrong. " + e.ToString());
                    throw;
                }

                IConfigurationValueProvider configProvider = new SingleTagSectionConfigurationProvider(this.ServerConfig.AuthenticationSectionName);

                try
                {
                    connection = factory.CreateConnection(configProvider["userName"], configProvider["password"]);
                }
                catch (EMSException e)
                {
                    Log.TraceData(Log.Source, TraceEventType.Error, 15001, "Connection to ems server failed! " + e.ToString());
                    throw;
                }

                try
                {
                    session = connection.CreateSession(this.sessionConfig.IsTransactional, sessionConfig.Mode);
                }
                catch (EMSException e)
                {
                    Log.TraceData(Log.Source, TraceEventType.Error, 15002, "Error during session creation. " + e.ToString());
                    throw;
                }

                try
                {
                    destination =
                        CreateDestination(session, queueConfig.Name, queueConfig.Type);

                    consumer =
                        session.CreateConsumer(destination, queueConfig.MessageSelector,
                                               queueConfig.NoLocal);

                    connection.Start();
                }
                catch (EMSException e)
                {
                    Log.TraceData(Log.Source, TraceEventType.Error, 15003, "Queue initialization error. " + e);
                    throw;
                }
                initialized = true;
            }
        }

        public bool RecoverFromConnectionError(Exception ex)
        {
            if (ex is ConfigurationErrorsException)
            {
                return false;
            }
            if (connectionFailureExceptionHandler != null)
            {
                if (connectionFailureExceptionHandler.HandleFailure(ex) == FailureExceptionType.NonRecoverable)
                {
                    return false;
                }
                return true;
            }
            // if there is no handler, return false so we can fail-fast or stop
            return false;
        }

        private void ValidateQueueConfiguration()
        {
            ErrorTrap.AddAssertion(sessionConfig != null, "SessionConfig != null", EmsCoordinationMessages.InvalidConfiguration, null);

            ErrorTrap.RaiseTrappedErrors<ConfigurationErrorsException>();

        }

        private static Destination CreateDestination(Session sess, string name, QueueType type)
        {
            Destination dest;
            switch (type)
            {
                case QueueType.Queue:
                    dest = sess.CreateQueue(name);
                    break;
                case QueueType.Topic:
                    dest = sess.CreateTopic(name);
                    break;
                default:
                    throw new ApplicationException("Internal error");
            }
            return dest;
        }



        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}