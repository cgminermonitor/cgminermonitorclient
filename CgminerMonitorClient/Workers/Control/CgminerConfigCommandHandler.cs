using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerConfigCommandHandler
    {
        private readonly Config _config;

        public CgminerConfigCommandHandler(Config config)
        {
            _config = config;
        }

        public string Read(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }

        public string Write(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}