using System;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;
using OpenHardwareMonitor.Hardware;

namespace CgminerMonitorClient.Workers.Hardware
{
    public class HardwareWorker : AbstractWorker
    {
        private IComputer _computer;
        private IVisitor _updateVisitor;

        public HardwareWorker(string statisticsKey) : base(statisticsKey)
        {
        }

        protected override bool CheckAvailability()
        {
            if (PlatformCheck.AreWeRunningUnderWindows())
                return true;
            Log.Instance.InfoFormat("'{0}' worker is not available on UNIX platform. Sorry.", StatisticKey);
            return false;
        }

        protected override string GetStats(Config config)
        {
            _computer.Accept(_updateVisitor);
            var stats = JsonConvert.SerializeObject(ComputerModel.FromComputer(_computer));
            return stats;
        }

        protected override void PreInit(Config config)
        {
            var computer = new Computer();
            _updateVisitor = new UpdateVisitor();
            computer.Open();
            computer.CPUEnabled = true;
            computer.FanControllerEnabled = true;
            computer.GPUEnabled = true;
            computer.HDDEnabled = true;
            computer.MainboardEnabled = true;
            computer.RAMEnabled = true;
            _computer = computer;
        }
    }
}