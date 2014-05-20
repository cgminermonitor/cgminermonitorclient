using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient.Workers.Control
{
    public class WorkerPowerCommandHandler
    {
        private readonly Config _config;

        public WorkerPowerCommandHandler(Config config)
        {
            _config = config;
        }

        public string Reboot(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }

        public string Shutdown(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}