using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerControlCommandHandler
    {
        private readonly Config _config;

        public CgminerControlCommandHandler(Config config)
        {
            _config = config;
        }

        public string SendCommandToCgminer(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}