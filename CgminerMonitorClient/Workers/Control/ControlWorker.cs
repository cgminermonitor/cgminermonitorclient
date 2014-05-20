using System;
using System.Collections.Generic;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.PostMakers;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Control
{
    public class ControlWorker : IWorkerDefinition
    {
        protected readonly string StatisticKey;
        private readonly RetryingHttpClient _client;

        protected ControlWorker(string statisticsKey)
        {
            StatisticKey = statisticsKey;
            _client = new RetryingHttpClient();
        }

        public void Start(object configObject)
        {
            var config = (Config)configObject;
            try
            {
                Log.Instance.DebugFormat("Starting {0} worker.", StatisticKey);
                Log.Instance.DebugFormat("Setting up handlers.");
                var commandKeyToCommandHandler = BootstrapCommandHandlers(config);
                string previousCommandResult = null;

                while (true)
                {
                    var message = new StatisticsInsertMessage(config.WorkerApiKey, StatisticKey);
                    if (!string.IsNullOrEmpty(previousCommandResult))
                        Log.Instance.DebugFormat("Sending previous command result: {0}", previousCommandResult);
                    message.Stats = previousCommandResult;

                    var result = _client.MakePost(message, config.RunOptions.PostMakerType);

                    if (result.Success)
                    {
                        Log.Instance.DebugFormat("{0} worker push/pull completed successfully.", StatisticKey);
                        if (result.Command != null)
                        {
                            Log.Instance.DebugFormat("Retreived '{0}' command.", result.Command);
                            previousCommandResult = ExecuteCommand(commandKeyToCommandHandler, result.Command);
                        }
                        Log.Instance.DebugFormat("Sleeping for {0} seconds.", result.SleepSeconds);
                        Thread.Sleep(TimeSpan.FromSeconds(result.SleepSeconds));
                    }
                    else
                    {
                        Log.Instance.InfoFormat("Error occured while sending {0} data. Error code: {1}, Message: {2}", StatisticKey, result.ErrorCode, result.ErrorMessage);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                }
            }
            catch (ThreadAbortException)
            {
                //its ok.
            }
            catch (Exception e)
            {
                Log.Instance.Info("Unknown error occured.", e);
            }
        }

        private delegate string ExecuteCommandFunc(WorkerCommand command);

        private static Dictionary<string, ExecuteCommandFunc> BootstrapCommandHandlers(Config config)
        {
            var workerPowerCommandHandler = new WorkerPowerCommandHandler(config);
            var cgminerPowerCommandHandler = new CgminerPowerCommandHandler(config);
            var cgminerConfigCommandHandler = new CgminerConfigCommandHandler(config);
            var cgminerControlCommandHandler = new CgminerControlCommandHandler(config);

            return new Dictionary<string, ExecuteCommandFunc>
            {
                { WorkerCommandHandlerKeys.CgminerCommand, cgminerControlCommandHandler.SendCommandToCgminer},

                { WorkerCommandHandlerKeys.ReadCgminerConfig, cgminerConfigCommandHandler.Read},
                { WorkerCommandHandlerKeys.WriteCgminerConfig, cgminerConfigCommandHandler.Write},

                { WorkerCommandHandlerKeys.StartCgminer, cgminerPowerCommandHandler.Start},
                { WorkerCommandHandlerKeys.StopCgminer, cgminerPowerCommandHandler.Stop},
                { WorkerCommandHandlerKeys.RebootCgminer, cgminerPowerCommandHandler.Reboot},

                { WorkerCommandHandlerKeys.Reboot, workerPowerCommandHandler.Reboot},
                { WorkerCommandHandlerKeys.Shutdown, workerPowerCommandHandler.Shutdown}
            };
        }

        private static string ExecuteCommand(IDictionary<string, ExecuteCommandFunc> commandKeyToCommandHandler, WorkerCommand command)
        {
            if (commandKeyToCommandHandler.ContainsKey(command.HandlerKey))
            {
                try
                {
                    var executeCommandFunc = commandKeyToCommandHandler[command.HandlerKey];
                    var result = executeCommandFunc(command);
                    return result;
                }
                catch (Exception e)
                {
                    Log.Instance.Info(string.Format("Executing '{0}' command resulted in error.", command), e);
                    return e.ToString();
                }
            }

            Log.Instance.InfoFormat("Could not find handler for '{0}' command. Do you have the latest client version?", command.HandlerKey);
            return string.Format("Could not find handler for '{0}' command.", command.HandlerKey);
        }
    }
}