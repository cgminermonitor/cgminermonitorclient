using System;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Workers.Control
{
    public class WorkerPowerCommandHandler
    {
        private readonly ControlConfig _controlConfig;

        public WorkerPowerCommandHandler(ControlConfig controlConfig)
        {
            _controlConfig = controlConfig;
        }

        public WorkerCommandResponse Reboot(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing reboot worker command.");
            return ExecuteCommandWithDelay(command.Id, _controlConfig.WorkerRebootCmd, _controlConfig);
        }

        public WorkerCommandResponse Shutdown(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing shutdown worker command.");
            return ExecuteCommandWithDelay(command.Id, _controlConfig.WorkerShutdownCmd, _controlConfig);
        }

        private static WorkerCommandResponse ExecuteCommandWithDelay(string commandId, string command, ControlConfig controlConfig)
        {
            if (PlatformCheck.AreWeRunningUnderWindows()) //under windows we can schedule reboot in 10secs
                return WorkerCommandResponse.FromProcessExecutionResult(commandId, SimpleProcessExecutor.Fire(command, false));

            var whoAmIResult = SimpleProcessExecutor.Fire(controlConfig.RootPrivilegesCmd, false);
            if (whoAmIResult.Success && string.CompareOrdinal(whoAmIResult.Output.Trim(), controlConfig.RootPrivilegesCmdResult) != 0)
            {
                return WorkerCommandResponse.Failure(commandId, string.Format("Checking root privileges failed. '{0}' command returned '{1}'. Expected: '{2}'.",
                        controlConfig.RootPrivilegesCmd, whoAmIResult.Output, controlConfig.RootPrivilegesCmdResult));
            }

            //under linux we have to schedule reboot in 1 min and wait 50 seconds to get the same behaviour
            var cmdResult = SimpleProcessExecutor.Fire(command, false);
            if (cmdResult.Success)
            {
                Log.Instance.Debug("Sleeping 50 seconds, to not fetch next command without executing this one.");
                Thread.Sleep(TimeSpan.FromSeconds(50));
                Log.Instance.Debug("Sleeping done.");
            }
            return WorkerCommandResponse.FromProcessExecutionResult(commandId, cmdResult);
        }
    }
}