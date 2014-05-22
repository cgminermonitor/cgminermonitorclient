using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;
using CgminerMonitorClient.Workers.Cgminer;

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
            Log.Instance.InfoFormat("Sending {0} to cgminer.", command.Value);
            var response = CommandSender.SendMessage(command.Value, _config.CgminerPort);
            Log.Instance.DebugFormat("Command sent. Response is: '{0}'.", response);
            return WorkerCommandResponse.Success(command.Id, response);
        }
    }
}