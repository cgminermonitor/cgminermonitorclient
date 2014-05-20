using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerPowerCommandHandler
    {
        private readonly Config _config;

        public CgminerPowerCommandHandler(Config config)
        {
            _config = config;
        }

        public string Reboot(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }

        public string Start(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }

        public string Stop(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}