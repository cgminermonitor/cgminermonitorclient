using System.Collections.Generic;
using CgminerMonitorClient.CgminerMonitor.Common;

namespace CgminerMonitorClient.Workers.Control
{
    public class ControlAccessRightsValidator
    {
        private delegate bool IsCommandAllowed();
        private readonly Dictionary<string, IsCommandAllowed> _commandValidators;

        private static Dictionary<string, IsCommandAllowed> BootstrapCommandValidators(Config config)
        {
            return new Dictionary<string, IsCommandAllowed>
            {
                { WorkerCommandHandlerKeys.CgminerCommand, () => config.AllowCgminerControl},

                { WorkerCommandHandlerKeys.ReadCgminerConfig, () => config.AllowCgminerConfigReadingAndWriting},
                { WorkerCommandHandlerKeys.WriteCgminerConfig, () => config.AllowCgminerConfigReadingAndWriting},

                { WorkerCommandHandlerKeys.StartCgminer, () => config.AllowCgminerPowerControl},
                { WorkerCommandHandlerKeys.StopCgminer, () => config.AllowCgminerPowerControl},
                { WorkerCommandHandlerKeys.RebootCgminer, () => config.AllowCgminerPowerControl},

                { WorkerCommandHandlerKeys.Reboot, () => config.AllowWorkerPowerControl},
                { WorkerCommandHandlerKeys.Shutdown, () => config.AllowWorkerPowerControl}
            };
        }

        public ControlAccessRightsValidator(Config config)
        {
            _commandValidators = BootstrapCommandValidators(config);
        }

        public bool CanExecute(WorkerCommand command)
        {
            if (_commandValidators.ContainsKey(command.HandlerKey))
                return _commandValidators[command.HandlerKey]();
            return false; //do not allow unknown commands
        }
    }
}