using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerConfigCommandHandler
    {
        private readonly ControlConfig _controlConfig;

        public CgminerConfigCommandHandler(ControlConfig controlConfig)
        {
            _controlConfig = controlConfig;
        }

        public WorkerCommandResponse Read(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }

        public WorkerCommandResponse Write(WorkerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}