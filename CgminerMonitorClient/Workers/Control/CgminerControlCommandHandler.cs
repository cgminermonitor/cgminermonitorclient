using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerControlCommandHandler
    {
        private readonly Config _config;

        public CgminerControlCommandHandler(Config config)
        {
            _config = config;
        }

        public WorkerCommandResponse SendCommandToCgminer(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}