using System;
using System.Diagnostics;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerPowerCommandHandler
    {
        private const string WindowsProcessNameExtension = ".exe";
        private readonly ControlConfig _controlConfig;

        public CgminerPowerCommandHandler(ControlConfig controlConfig)
        {
            _controlConfig = controlConfig;
        }
        public WorkerCommandResponse Start(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing start cgminer command.");
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, SimpleProcessExecutor.Fire(_controlConfig.CgminerStartCmd, true));
        }

        public WorkerCommandResponse Stop(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing stop cgminer command.");
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, SimpleProcessExecutor.Fire(_controlConfig.CgminerKillCmd, false));
        }

        public WorkerCommandResponse Reboot(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing restart cgminer command.");
            var stop = SimpleProcessExecutor.Fire(_controlConfig.CgminerKillCmd, false);
            var start = SimpleProcessExecutor.Fire(_controlConfig.CgminerStartCmd, true);
            var msgStart = start.Output;
            if (string.IsNullOrEmpty(msgStart))
            {
                msgStart = "OK";
            }
            var resultOutput = string.Concat("Start: ", msgStart, Environment.NewLine, "Stop: ", stop.Output);
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

        public static bool IsCgminerProcessRunning(string cgminerProcessName)
        {
            if (cgminerProcessName.EndsWith(WindowsProcessNameExtension))
            {
                cgminerProcessName = cgminerProcessName.Substring(0, cgminerProcessName.Length - WindowsProcessNameExtension.Length);
            }
            return Process.GetProcessesByName(cgminerProcessName).Length >= 1;
        }
    }
}