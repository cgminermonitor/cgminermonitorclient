using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;

namespace CgminerMonitorClient.Workers.Control
{
    public class CgminerPowerCommandHandler
    {
        private readonly ControlConfig _controlConfig;

        public CgminerPowerCommandHandler(ControlConfig controlConfig)
        {
            _controlConfig = controlConfig;
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