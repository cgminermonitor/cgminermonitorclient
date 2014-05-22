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

        public string Reboot(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing reboot worker command.");
            return SimpleProcessExecutor.Fire(_controlConfig.WorkerRebootCmd);
        }

        public string Shutdown(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing shutdown worker command.");
            return SimpleProcessExecutor.Fire(_controlConfig.WorkerShutdownCmd);
        }
    }
}