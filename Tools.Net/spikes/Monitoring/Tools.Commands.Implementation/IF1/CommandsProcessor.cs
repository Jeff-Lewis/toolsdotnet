﻿using System;
using System.Collections.Generic;
using System.Data;

using System.Data.Common;
using System.Configuration;
using Tools.Core.Asserts;
using System.Transactions;
using Tools.Core.Utils;
using System.Runtime.Serialization;
using System.Text;
using System.Data.OracleClient;
using Tools.Coordination.Batch;
using System.Diagnostics;


namespace Tools.Commands.Implementation
{


    public class CommandsProcessor : ScheduleTaskProcessor
    {
        #region Fields
        string readerSPName;
        string invalidCommandStatus = "INVALID";
        IResponseDataProvider responseDataProvider;
        Dictionary<decimal, ICommandExecutor> executors; 
        #endregion

        #region Properties
        CommandSelectionOptions Filter { get; set; }
        public string InvalidCommandStatus { get { return invalidCommandStatus; } set { invalidCommandStatus = value; } } 
        #endregion

        #region Construction

        public CommandsProcessor(string readerSPName, Dictionary<decimal, ICommandExecutor> executors, IResponseDataProvider responseDataProvider)
        {
            Init(readerSPName, executors, responseDataProvider);
        }
        private void Init(string readerSPName, Dictionary<decimal, ICommandExecutor> executors, IResponseDataProvider responseDataProvider)
        {
            ErrorTrap.AddAssertion(!String.IsNullOrEmpty(readerSPName), "readerSPName should be assigned in the ctor of " + this.GetType().FullName + ". Please correct the configuration and restart.");

            ErrorTrap.AddAssertion(executors != null && executors.Count != 0, "Dictionary<decimal, ICommandExecutor> executors can't be null. Please correct the configuration for " + this.GetType().FullName + " and restart.");

            ErrorTrap.AddAssertion(responseDataProvider != null, "responseDataProvider can't be null. Please correct the configuration for " + this.GetType().FullName + " and restart.");

            ErrorTrap.RaiseTrappedErrors<ConfigurationErrorsException>();

            this.readerSPName = readerSPName;
            this.executors = executors;
            this.responseDataProvider = responseDataProvider;
        } 
        #endregion

        #region Scheduling

        protected override void ExecuteSheduleTask()
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Start, CommandMessages.StartingScheduledExecutionIteration, String.Format("Looking for commands [CommandType={0}, SPName={1}]",
                Filter.CommandTypeId, readerSPName));

            base.ExecuteSheduleTask();

            bool thereWasSomethingToProcess = false;

            try
            {
                thereWasSomethingToProcess = ExecuteNextCommand();
                if (thereWasSomethingToProcess)
                    Schedule.SetForImmidiateRun();
            }
            catch (Exception ex)
            {
                Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Error, CommandMessages.ScheduledIterationFailed, ex.ToString());
                throw;
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns true if there was something to process</returns>
        public bool ExecuteNextCommand()
        {
            ErrorTrap.AddAssertion(Filter != null, "Filter parameter of ExecuteNextCommand should not be null!");
            ErrorTrap.RaiseTrappedErrors<ArgumentNullException>();

            Filter.MachineName = Environment.MachineName;
            OracleTransaction transaction = null;

            bool thereWasSomethingToProcess = false;
            //NOTE: (SD) The code bellow is written for only one record!!!
            //Different handling of exception/transaction would be required if there are multiple records
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["SourceDB"].ConnectionString))
                {

                    connection.Open();

                    transaction = connection.BeginTransaction();

                    Dictionary<decimal, GenericCommand> commands = PopulateCommands(Filter, connection, transaction);

                    LogCommands(commands);

                    GenericCommand command = null;
                    ICommandExecutor executor = null;

                    foreach (decimal reqId in commands.Keys)
                    {
                        try
                        {
                            #region Process single command
                            command = commands[reqId];
                            executor = executors[command.CommandType];
                            thereWasSomethingToProcess = true;

                            if (executor.Execute(command))
                            {
                                transaction.Commit();
                                executor.Commit();
                                Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Information,
CommandMessages.WorkOnCommandCommitted,
                                    "Work on command Id " + command.ReqId + " commited.");
                            }
                            else
                            {
                                //if (transaction != null) transaction.Rollback();

                                Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Information,
CommandMessages.WorkOnCommandCommitted, "Command " + command.ReqId + " is identified as invalid because:\r\n" + ErrorTrap.Text);

                                if (ErrorTrap.HasErrors)
                                {


                                    // normalize the error text for the length as db column is limited to 1000 chars.
                                    string errorDescription = null;
                                    if (!String.IsNullOrEmpty(ErrorTrap.Text))
                                    {
                                        errorDescription = (ErrorTrap.Text.Length <= 1000) ? ErrorTrap.Text : ErrorTrap.Text.Substring(0, 1000);
                                    }


                                    responseDataProvider.UpdateResponseToFtPro(
                                        command.ReqId,
                                        "B",
                                        invalidCommandStatus,
                                        "CEA",
                                        DateTime.Now,
                                        errorDescription,
                                        String.Empty,
                                        connection,
                                        transaction);

                                    ErrorTrap.Reset();
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {

                            if (transaction != null) transaction.Rollback();

                            Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Error, CommandMessages.ErrorWhileExecutingTheCommand,
                                String.Format("Exception happened while trying to execute the command with id{0} Changes are rollbacked. Exception text is: \r\n{1} and command is: \r\n{2}", command.ReqId, ex.ToString(), SerializationUtility.Serialize2String(command)));

                            // normalize the error text for the length as db column is limited to 1000 chars.
                            //string errorDescription = 
                            //    errorDescription = (ex.ToString() <= 1000) ? ex.ToString() : ex.ToString().Substring(0, 1000);
                            //if (command != null)
                            //{
                            //    responseDataProvider.UpdateResponseToFtPro(
                            //        command.ReqId,
                            //        "W",
                            //        invalidCommandStatus,
                            //        "CEA",
                            //        DateTime.Now,
                            //        errorDescription,
                            //        String.Empty,
                            //        connection);
                            //}

                            throw;
                        }
                    }


                    //transaction.Rollback(); // Only for test purposes
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //unfortunately there is no status on the oracle transaction
                    if (transaction != null) transaction.Rollback();
                }
                catch (Exception ex2)
                {

                }

                throw;
            }
            return thereWasSomethingToProcess;
        } 
        #endregion



        #region Data Access
        private Dictionary<decimal, GenericCommand> PopulateCommands(CommandSelectionOptions options, OracleConnection con, OracleTransaction tx)
        {


            Dictionary<decimal, GenericCommand> commands = new Dictionary<decimal, GenericCommand>();

            Dictionary<decimal, MarketingPackage> mps = new Dictionary<decimal, MarketingPackage>();


            // create the command object and set attributes
            using (OracleCommand cmd = new OracleCommand(readerSPName, con, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region test parameters

                OracleCommandBuilder.DeriveParameters(cmd);

                //foreach (OracleParameter p in cmd.Parameters)
                //{
                //    Console.WriteLine(String.Format("Name: {0}, OracleType: {1}", p.ParameterName, p.OracleType));
                //}

                //return null;

                #endregion


                //OracleParameter pRefCursor = cmd.Parameters["P_COMMANDS"]; //new OracleParameter("P_COMMANDS", OracleType.Cursor);


                AssignOracleParameter2String("P_PARTITIONNAME", options.PartitionName, cmd);
                AssignOracleParameter2String("P_BATCHID", options.BatchId, cmd);

                cmd.Parameters["P_COMMANDTYPE"].Value = options.CommandTypeId;

                AssignOracleParameter2String("P_MACHINENAME", options.MachineName, cmd);
                AssignOracleParameter2String("P_USERNAME", Environment.UserName, cmd);
                AssignOracleParameter2String("P_ACTIVITYID", options.ActivityId, cmd);

                try
                {

                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            //Console.WriteLine(Convert.ToDecimal(dr["REQ_ID"]) + ":" + dr["Status"]);

                            GenericCommand gCmd = new GenericCommand();

                            gCmd.ReqId = GetDefaultDecimal(dr, "REQ_ID", true, 0);

                            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                            Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Start, CommandMessages.CommandDispatchedFromTheDatabase, "Command[ " + options.CommandTypeId + ":" + gCmd.ReqId + "]");

                            gCmd.CommandType = GetDefaultDecimal(dr, "COMMAND_TYPE", true, 0);
                            gCmd.ReqTime = GetDefaultDate(dr, "REQ_TIME", true, DateTime.Now);
                            gCmd.TisCustomerId = GetDefaultString(dr, "TIS_CUSTOMER_ID", false, String.Empty);

                            gCmd.TisWalletId = GetDefaultString(dr, "TIS_WALLET_ID", false, String.Empty);
                            gCmd.TisTDId = GetDefaultDecimal(dr, "TIS_TD_ID", false, 0);
                            gCmd.CustomerType = GetDefaultString(dr, "CUSTOMER_TYPE", false, String.Empty);
                            gCmd.Name = GetDefaultString(dr, "NAME", false, String.Empty);
                            gCmd.BillingCycle = GetNullableDecimal(dr, "BILLING_CYCLE", false, null);

                            gCmd.TaxGroup = GetDefaultString(dr, "TAX_GROUP", false, String.Empty);
                            gCmd.TDType = GetDefaultDecimal(dr, "TD_TYPE", false, 0);

                            gCmd.MonthlyLimit = GetDefaultDecimal(dr, "MONTHLY_LIMIT", false, 0);

                            gCmd.IccId = GetDefaultString(dr, "ICCID", false, String.Empty);
                            gCmd.PhoneNumber = GetDefaultString(dr, "PHONE_NUMBER", false, String.Empty);
                            gCmd.BlockReason = GetDefaultString(dr, "BLOCK_REASON", false, String.Empty);
                            gCmd.BlockStatus = GetDefaultDecimal(dr, "BLOCK_STATUS", false, 0);

                            gCmd.VpnProfile = GetDefaultDecimal(dr, "VPN_PROFILE", false, 0);
                            gCmd.ShortNumber = GetDefaultString(dr, "SHORT_NUMBER", false, String.Empty);
                            gCmd.NewPhoneNumber = GetDefaultString(dr, "NEW_PHONE_NUMBER", false, String.Empty);
                            gCmd.NewIccId = GetDefaultString(dr, "NEW_ICCID", false, String.Empty);
                            gCmd.ContractEndDate = GetDefaultDate(dr, "CONTRACT_END_DATE", false, DateTime.MinValue);
                            gCmd.ValidFrom = GetDefaultDate(dr, "VALID_FROM", false, DateTime.MinValue);
                            gCmd.P2P = GetDefaultString(dr, "P2P", false, String.Empty);
                            gCmd.OnetimeLimitAmount = GetDefaultDecimal(dr, "ONETIME_LIMIT_AMOUNT", false, 0);
                            gCmd.Status = GetDefaultString(dr, "STATUS", false, String.Empty);
                            gCmd.TisPosAOId = GetDefaultString(dr, "TIS_POSAO_ID", false, String.Empty);
                            gCmd.Priority = GetDefaultDecimal(dr, "PRIORITY", false, 0);

                            gCmd.ActivityId = options.ActivityId;

                            commands.Add(gCmd.ReqId, gCmd);

                        }
                        // Next result is marketing package instances
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                //Console.WriteLine(Convert.ToDecimal(dr["REQ_ID"]) + ":mpid:" + Convert.ToDecimal(dr["MP_INSTANCE_ID"]));

                                MarketingPackage mp = new MarketingPackage
                                {
                                    ReqId = GetDefaultDecimal(dr, "REQ_ID", true, 0),
                                    MPInstanceId = GetDefaultDecimal(dr, "MP_INSTANCE_ID", true, 0),
                                    MPId = GetDefaultDecimal(dr, "MP_ID", true, 0),
                                    MPType = GetDefaultString(dr, "MP_TYPE", true, String.Empty)
                                };

                                commands[mp.ReqId].MarketingPackages.Add(mp);
                                mps.Add(mp.MPInstanceId, mp);
                            }
                            //}

                            // Next result is marketing package parameters
                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    //Console.WriteLine("mpid: " + Convert.ToDecimal(dr["MP_INSTANCE_ID"]) + ":" + dr["PARAM_CODE"]);

                                    PackageParameter p = new PackageParameter
                                    {
                                        ReqId = GetDefaultDecimal(dr, "REQ_ID", true, 0),
                                        MPInstanceId = GetDefaultDecimal(dr, "MP_INSTANCE_ID", true, 0),
                                        ParamCode = GetDefaultString(dr, "PARAM_CODE", true, String.Empty),
                                        ProductCode = GetDefaultString(dr, "PRODUCT_CODE", true, String.Empty),
                                        Value = GetDefaultString(dr, "VALUE", true, String.Empty)
                                    };

                                    mps[p.MPInstanceId].Parameters.Add(p);

                                }
                            }
                        }

                        ErrorTrap.RaiseTrappedErrors<InvalidOperationException>();
                        return commands;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    //if (pMpsCursor != null) pMpsCursor.Dispose();
                    //if (pParamsCursor != null) pParamsCursor.Dispose();
                }
            }

            return commands;
        } 
        #endregion

        #region Helper methods
        private static void LogCommands(Dictionary<decimal, GenericCommand> commands)
        {
            if (commands.Count > 0)
            {
                StringBuilder sb = new StringBuilder(4000);

                foreach (decimal reqId in commands.Keys)
                {
                    sb.Append(SerializationUtility.Serialize2String(commands[reqId]));
                }

                Log.TraceData(Log.Source, System.Diagnostics.TraceEventType.Information, CommandMessages.CommandDispatchedFromTheDatabase, sb.ToString());
            }
        }
        private static void AssignOracleParameter2String(string paramName, string valueToAssign, OracleCommand cmd)
        {

            if (String.IsNullOrEmpty(valueToAssign))
            {
                cmd.Parameters[paramName].Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters[paramName].Value = valueToAssign;
            }
        }

        String GetDefaultString(IDataReader dr, string fieldName, bool mandatory, string defaultValue)
        {
            try
            {
                bool isNull = dr.IsDBNull(dr.GetOrdinal(fieldName));
                if (!ErrorTrap.AddAssertion(!(mandatory && isNull),
                    String.Format("Field {0} is mandatoryfor application but its value is null", fieldName)))
                {
                    return null;
                }
                if (!mandatory && isNull) return defaultValue;

                return dr.GetString(dr.GetOrdinal(fieldName));
            }
            catch (Exception ex)
            {
                ErrorTrap.AddAssertion(ex == null, String.Format("Exception when converting field {0}. \r\n {1}",
                    fieldName, ex));
                return null;
            }
        }

        Decimal GetDefaultDecimal(IDataReader dr, string fieldName, bool mandatory, decimal defaultValue)
        {
            try
            {
                bool isNull = dr.IsDBNull(dr.GetOrdinal(fieldName));

                if (!ErrorTrap.AddAssertion(!(mandatory && isNull),
                    String.Format("Field {0} is mandatoryfor application but its value is null", fieldName)))
                {
                    return defaultValue;
                }
                if (!mandatory && isNull) return defaultValue;

                return dr.GetDecimal(dr.GetOrdinal(fieldName));

            }
            catch (Exception ex)
            {
                ErrorTrap.AddAssertion(ex == null, String.Format("Exception when converting field {0}. \r\n {1}",
                    fieldName, ex));
                return 0;
            }
        }
        Decimal? GetNullableDecimal(IDataReader dr, string fieldName, bool mandatory, decimal? defaultValue)
        {
            try
            {
                bool isNull = dr.IsDBNull(dr.GetOrdinal(fieldName));

                if (!ErrorTrap.AddAssertion(!(mandatory && isNull),
                    String.Format("Field {0} is mandatoryfor application but its value is null", fieldName)))
                {
                    return defaultValue;
                }
                if (!mandatory && isNull) return defaultValue;

                return dr.GetDecimal(dr.GetOrdinal(fieldName));

            }
            catch (Exception ex)
            {
                ErrorTrap.AddAssertion(ex == null, String.Format("Exception when converting field {0}. \r\n {1}",
                    fieldName, ex));
                return null;
            }
        }
        DateTime GetDefaultDate(IDataReader dr, string fieldName, bool mandatory, DateTime defaultValue)
        {
            try
            {
                bool isNull = dr.IsDBNull(dr.GetOrdinal(fieldName));
                if (!ErrorTrap.AddAssertion(!(mandatory && isNull),
                    String.Format("Field {0} is mandatoryfor application but its value is null", fieldName)))
                {
                    return DateTime.MinValue;
                }
                if (!mandatory && isNull) return defaultValue;

                return dr.GetDateTime(dr.GetOrdinal(fieldName));
            }
            catch (Exception ex)
            {
                ErrorTrap.AddAssertion(ex == null, String.Format("Exception when converting field {0}. \r\n {1}",
                    fieldName, ex));
                return DateTime.MinValue; ;
            }
        } 
        #endregion
    }
}

#region Commands table dll

//"COMMAND_TYPE" NUMBER, 
//    "REQ_ID" NUMBER NOT NULL ENABLE, 
//    "REQ_TIME" DATE, 
//    "TIS_CUSTOMER_ID" Varchar(15), 
//    "TIS_WALLET_ID" Varchar(20), 
//    "TIS_TD_ID" NUMBER, 
//    "CUSTOMER_TYPE" CHAR(1), 
//    "NAME" Varchar(200), 
//    "BILLING_CYCLE" NUMBER, 
//    "TAX_GROUP" Varchar(1), 
//    "TD_TYPE" NUMBER, 
//    "MONTHLY_LIMIT" NUMBER(22,4), 
//    "ICCID" Varchar(64), 
//    "PHONE_NUMBER" Varchar(64), 
//    "BLOCK_REASON" Varchar(20), 
//    "BLOCK_STATUS" NUMBER, 
//    "VPN_PROFILE" NUMBER, 
//    "SHORT_NUMBER" Varchar(64), 
//    "NEW_PHONE_NUMBER" Varchar(64), 
//    "NEW_ICCID" Varchar(64), 
//    "CONTRACT_END_DATE" DATE, 
//    "VALID_FROM" DATE, 
//    "P2P" Varchar(2), 
//    "ONETIME_LIMIT_AMOUNT" NUMBER(10,2), 
//    "STATUS" Varchar(20) DEFAULT 'NEW', 
//    "TIS_POSAO_ID" Varchar(15), 
//    "PRIORITY" NUMBER, 
//     CONSTRAINT "FORIS_COMMANDS_PROV_PK" PRIMARY KEY ("REQ_ID")
//  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
//  STORAGE(INITIAL 2097152 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
//  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
//  TABLESPACE "USERS"  ENABLE, 
//     CONSTRAINT "PROV_BLOCK_CHECK" CHECK (block_reason in ('BLOCK_USER','BLOCK_FINANCIAL','BLOCK_USAGE','BLOCK_ADMIN')) ENABLE, 
//     CONSTRAINT "PROV_C_TYPE_CHECK" CHECK (customer_type in ('O','P')) ENABLE, 
//     CONSTRAINT "PROV_STATUS_CHECK" CHECK (status in ('NEW','PROCESSING','DONE','FAILED','XXX','YYY','OFFLINE','TEST','OFFLINE1','BAD','PP','GOOD','EXISTCM')) ENABLE, 
//     CONSTRAINT "FORIS_COMMAND_PROV_TYPE_FK" FOREIGN KEY ("COMMAND_TYPE")
//      REFERENCES "FTPRO"."FORIS_COMMAND_TYPE" ("COMMAND_TYPE_ID") ENABLE NOVALIDATE
//   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
//  STORAGE(INITIAL 6291456 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
//  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
//  TABLESPACE "FTPRO" ;

//  CREATE UNIQUE INDEX "FTPRO"."FORIS_COMMANDS_PROV_PK" ON "FTPRO"."FORIS_COMMANDS_PROV" ("REQ_ID") 
//  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
//  STORAGE(INITIAL 2097152 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
//  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
//  TABLESPACE "USERS" ;

//  CREATE INDEX "FTPRO"."I_FCP_PHONE_NUMBER" ON "FTPRO"."FORIS_COMMANDS_PROV" ("PHONE_NUMBER") 
//  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
//  STORAGE(INITIAL 196608 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
//  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
//  TABLESPACE "FTPRO" ;

//  ALTER TABLE "FTPRO"."FORIS_COMMANDS_PROV" ADD CONSTRAINT "FORIS_COMMANDS_PROV_PK" PRIMARY KEY ("REQ_ID")
//  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
//  STORAGE(INITIAL 2097152 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
//  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
//  TABLESPACE "USERS"  ENABLE;

#endregion