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
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, ExecuteStart());
        }

        public WorkerCommandResponse Stop(WorkerCommand command)
        {
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, ExecuteStop());
        }

        public WorkerCommandResponse Reboot(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing restart cgminer command.");
            var stopResult = ExecuteStop();
            var startResult = ExecuteStart();
            var resultOutput = string.Concat("Start: ", startResult.Output, Environment.NewLine, "Stop: ", stopResult.Output);
            if (stopResult.Success && startResult.Success)
            {
                return WorkerCommandResponse.Success(command.Id, resultOutput);
            }
            if (!stopResult.Success && !startResult.Success)
            {
                return WorkerCommandResponse.Failure(command.Id, resultOutput);
            }
            return WorkerCommandResponse.Warning(command.Id, resultOutput);
        }

        private ProcessExecutorResult ExecuteStart()
        {
            Log.Instance.InfoFormat("Executing start cgminer command.");
            var cgminerIsRunningBeforeStart = IsCgminerProcessRunning(_cgminerProcessName);
            if (cgminerIsRunningBeforeStart)
                return ProcessExecutorResult.Failed(CgminerProcessIsAlreadyStarted);

            var start = SimpleProcessExecutor.Fire(_controlConfig.CgminerStartCmd, true);
            if (start.Success)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                var cgminerIsRunning = IsCgminerProcessRunning(_cgminerProcessName);
                if (cgminerIsRunning)
                {
                    if (string.IsNullOrEmpty(start.Output))
                        start = ProcessExecutorResult.Succeeded("OK");
                }
                else
                {
                    start = ProcessExecutorResult.Failed(NoCgminerProcessAfterStartMessage);
                }
            }
            return start;
        }

        private ProcessExecutorResult ExecuteStop()
        {
            Log.Instance.InfoFormat("Executing stop cgminer command.");
            var cgminerIsRunningBeforeStop = IsCgminerProcessRunning(_cgminerProcessName);
            ProcessExecutorResult stopResult;
            if (!cgminerIsRunningBeforeStop)
            {
                stopResult = ProcessExecutorResult.Failed(CgminerProcessIsNotRunningMessage);
            }
            else
            {
                stopResult = SimpleProcessExecutor.Fire(_controlConfig.CgminerKillCmd, false);
            }
            return stopResult;
        }

        public static bool IsCgminerProcessRunning(string cgminerProcessName)
        {
            if (cgminerProcessName.EndsWith(WindowsProcessNameExtension))
            {
                cgminerProcessName = cgminerProcessName.Substring(0, cgminerProcessName.Length - WindowsProcessNameExtension.Length);
            }
            return Process.GetProcessesByName(cgminerProcessName).Length >= 1;
        }

        private string CgminerProcessIsAlreadyStarted
        {
            get
            {
                return string.Format("'{0}' process is already running. Aborted start cgminer command.", _cgminerProcessName);
            }
        }

        private string NoCgminerProcessAfterStartMessage
        {
            get
            {
                return string.Format("Command executed properly, however no '{0}' process was found after 5 seconds.", _cgminerProcessName);
            }
        }

        private string CgminerProcessIsNotRunningMessage
        {
            get { return string.Format("'{0}' process is not running. Aborted stop cgminer command.", _cgminerProcessName); }
        }
    }
}