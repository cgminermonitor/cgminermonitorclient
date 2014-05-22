using System;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerPowerCommandHandler
    {
        private readonly ControlConfig _controlConfig;

        public CgminerPowerCommandHandler(ControlConfig controlConfig)
        {
            _controlConfig = controlConfig;
        }
        public WorkerCommandResponse Start(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing start cgminer command.");
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, SimpleProcessExecutor.Fire(_controlConfig.CgminerStartCmd));
        }

        public WorkerCommandResponse Stop(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing stop cgminer command.");
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, SimpleProcessExecutor.Fire(_controlConfig.CgminerKillCmd));
        }

        public WorkerCommandResponse Reboot(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing restart cgminer command.");
            var stop = SimpleProcessExecutor.Fire(_controlConfig.CgminerKillCmd);
            var start = SimpleProcessExecutor.Fire(_controlConfig.CgminerStartCmd);
            var resultOutput = string.Concat(start.Output, Environment.NewLine, stop.Output);
            if (stop.Success && start.Success)
            {
                return WorkerCommandResponse.Success(command.Id, resultOutput);
            }
            if (!stop.Success && !start.Success)
            {
                return WorkerCommandResponse.Failure(command.Id, resultOutput);
            }
            return WorkerCommandResponse.Warning(command.Id, resultOutput);
        }
    }
}