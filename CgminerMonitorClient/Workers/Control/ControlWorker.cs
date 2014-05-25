using System;
using System.Collections.Generic;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.PostMakers;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Control
{
    public class ControlWorker : IWorkerDefinition
    {
        protected readonly string StatisticKey;
        private readonly RetryingHttpClient _client;

        public ControlWorker(string statisticsKey)
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
                var accessRightsCommandValidator = new ControlAccessRightsValidator(config.ControlOptions);
                var commandKeyToCommandHandler = BootstrapCommandHandlers(config);

                while (true)
                {
                    var message = new StatisticsInsertMessage(config.WorkerApiKey, StatisticKey);
                    var pulledCommand = _client.MakePost(message, config.RunOptions.PostMakerType);

                    if (pulledCommand.Success)
                    {
                        if (pulledCommand.Command != null)
                        {
                            Log.Instance.DebugFormat("Retrieved '{0}' command.", pulledCommand.Command);
                            var previousCommandResult = ExecuteCommand(commandKeyToCommandHandler, accessRightsCommandValidator, pulledCommand.Command);
                            Log.Instance.DebugFormat("Sending '{0}' command result.", pulledCommand.Command);
                            var responseMessage = new StatisticsInsertMessage(config.WorkerApiKey, StatisticKey)
                            {
                                WorkerCommandResponse = previousCommandResult
                            };
                            var responseInsertionStatus = _client.MakePost(responseMessage, config.RunOptions.PostMakerType);
                            if (responseInsertionStatus.Success)
                                Log.Instance.Info("Command response sent successfully.");
                            else
                                Log.Instance.InfoFormat("Error occured while sending command response. Error code: {0}, Message: {1}", responseInsertionStatus.ErrorCode, responseInsertionStatus.ErrorMessage);
                        }
                        Log.Instance.DebugFormat("Sleeping for {0} seconds.", pulledCommand.SleepSeconds);
                        Thread.Sleep(TimeSpan.FromSeconds(pulledCommand.SleepSeconds));
                    }
                    else
                    {
                        Log.Instance.InfoFormat("Error occured while sending {0} data. Error code: {1}, Message: {2}", StatisticKey, pulledCommand.ErrorCode, pulledCommand.ErrorMessage);
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

        private delegate WorkerCommandResponse ExecuteCommandFunc(WorkerCommand command);

        private static Dictionary<string, ExecuteCommandFunc> BootstrapCommandHandlers(Config config)
        {
            var controlConfig = config.ControlOptions;
            var workerPowerCommandHandler = new WorkerPowerCommandHandler(controlConfig);
            var cgminerPowerCommandHandler = new CgminerPowerCommandHandler(controlConfig);
            var cgminerConfigCommandHandler = new CgminerConfigCommandHandler(controlConfig);
            var cgminerControlCommandHandler = new CgminerControlCommandHandler(config);

            return new Dictionary<string, ExecuteCommandFunc>
            {
                { WorkerCommandHandlerKeys.CgminerCommand, cgminerControlCommandHandler.SendCommandToCgminer},

                { WorkerCommandHandlerKeys.ReadCgminerConfig, cgminerConfigCommandHandler.Read},
                { WorkerCommandHandlerKeys.WriteCgminerConfig, cgminerConfigCommandHandler.Write},

                { WorkerCommandHandlerKeys.StartCgminer, cgminerPowerCommandHandler.Start},
                { WorkerCommandHandlerKeys.StopCgminer, cgminerPowerCommandHandler.Stop},
                { WorkerCommandHandlerKeys.RestartCgminer, cgminerPowerCommandHandler.Reboot},

                { WorkerCommandHandlerKeys.Reboot, workerPowerCommandHandler.Reboot},
                { WorkerCommandHandlerKeys.Shutdown, workerPowerCommandHandler.Shutdown}
            };
        }

        private static WorkerCommandResponse ExecuteCommand(IDictionary<string, ExecuteCommandFunc> commandKeyToCommandHandler, ControlAccessRightsValidator accessRightsCommandValidator, WorkerCommand command)
        {
            if (commandKeyToCommandHandler.ContainsKey(command.HandlerKey))
            {
                try
                {
                    var canExecuteCommand = accessRightsCommandValidator.CanExecute(command);
                    if (canExecuteCommand)
                    {
                        Log.Instance.DebugFormat("Executing '{0}' command is allowed.", command);
                        var executeCommandFunc = commandKeyToCommandHandler[command.HandlerKey];
                        var result = executeCommandFunc(command);
                        Log.Instance.Debug(result);
                        return result;
                    }

                    var response = WorkerCommandResponse.NotAllowed(command.Id);
                    Log.Instance.Info(string.Format("You do not allow to execute '{0}' command.", command));
                    return response;
                }
                catch (Exception e)
                {
                    Log.Instance.Info(string.Format("Executing '{0}' command resulted in error.", command), e);
                    return WorkerCommandResponse.Failure(command.Id, e.ToString());
                }
            }

            Log.Instance.InfoFormat("Could not find handler for '{0}' command. Do you have the latest client version?", command.HandlerKey);
            return WorkerCommandResponse.Unknown(command.Id);
        }
    }
}