using System.IO;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;

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
            Log.Instance.InfoFormat("Executing read cgminer config command.");
            var configFile = File.ReadAllText(_controlConfig.CgminerConfigFileLocation);
            return WorkerCommandResponse.Success(command.Id, configFile);
        }

        public WorkerCommandResponse Write(WorkerCommand command)
        {
            Log.Instance.InfoFormat("Executing write cgminer config command.");
            File.WriteAllText(_controlConfig.CgminerConfigFileLocation, command.Value);
            return WorkerCommandResponse.Success(command.Id, null);
        }
    }
}