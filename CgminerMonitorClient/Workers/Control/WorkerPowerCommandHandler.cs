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
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, SimpleProcessExecutor.Fire(_controlConfig.WorkerRebootCmd));
        }

        public WorkerCommandResponse Shutdown(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing shutdown worker command.");
            return WorkerCommandResponse.FromProcessExecutionResult(command.Id, SimpleProcessExecutor.Fire(_controlConfig.WorkerShutdownCmd));
        }
    }
}