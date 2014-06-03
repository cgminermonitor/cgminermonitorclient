using System;
using System.Diagnostics;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerPowerCommandHandler
    {
        private const string WindowsProcessNameExtension = ".exe";
        private readonly ControlConfig _controlConfig;
        private readonly string _cgminerProcessName;

        public CgminerPowerCommandHandler(ControlConfig controlConfig, string cgminerProcessName)
        {
            _controlConfig = controlConfig;
            _cgminerProcessName = cgminerProcessName;
        }

        public WorkerCommandResponse Start(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing start cgminer command.");
            var startCgminerExecutionResult = SimpleProcessExecutor.Fire(_controlConfig.CgminerStartCmd, true);
            if (startCgminerExecutionResult.Success)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                var cgminerIsRunning = IsCgminerProcessRunning(_cgminerProcessName);
                if (cgminerIsRunning)
                    return WorkerCommandResponse.FromProcessExecutionResult(command.Id, startCgminerExecutionResult);
                return WorkerCommandResponse.Failure(command.Id, GetNoCgminerProcessMessage());
            }
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, startCgminerExecutionResult);
        }

        private string GetNoCgminerProcessMessage()
        {
            return string.Format("Command executed properly, however no '{0}' process was found after 5 seconds.", _cgminerProcessName);
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
            if (start.Success)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                var cgminerIsRunning = IsCgminerProcessRunning(_cgminerProcessName);
                if (cgminerIsRunning)
                {
                    if (string.IsNullOrEmpty(start.Output))
                        start = ProcessExecutorResult.Succeeded(GetNoCgminerProcessMessage());
                }
                else
                {
                    start = ProcessExecutorResult.Failed(GetNoCgminerProcessMessage());
                }
            }
            var resultOutput = string.Concat("Start: ", start.Output, Environment.NewLine, "Stop: ", stop.Output);
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