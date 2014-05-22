using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient.Configuration
{
    public class ControlConfig
    {
        public bool AllowWorkerPowerControl { get; set; }
        public bool AllowCgminerPowerControl { get; set; }
        public bool AllowCgminerConfigReadingAndWriting { get; set; }
        public bool AllowCgminerControl { get; set; }

        public string CgminerStartCmd { get; set; }
        public string CgminerKillCmd { get; set; }
        public string CgminerProcessName { get; set; }

        public string CgminerConfigFileLocation { get; set; }

        public string WorkerRebootCmd { get; set; }
        public string WorkerShutdownCmd { get; set; }

        public void Bootstrap()
        {
            GenerateCgminerKillCmdBasedOnCgminerProcessName();
            GenerateWorkerPowerCommands();
        }

        private void GenerateWorkerPowerCommands()
        {
            if (PlatformCheck.AreWeRunningUnderWindows())
            {
                WorkerRebootCmd = "shutdown /r /t 10";
                WorkerShutdownCmd = "shutdown /s /t 10";
            }
            else
            {
                WorkerRebootCmd = "shutdown -r 10";
                WorkerShutdownCmd = "shutdown -p 10";
            }
        }

        private void GenerateCgminerKillCmdBasedOnCgminerProcessName()
        {
            if (PlatformCheck.AreWeRunningUnderWindows())
                CgminerKillCmd = string.Format(@"taskkill /F /IM ""{0}""", CgminerProcessName);
            else
                CgminerKillCmd = string.Format(@"pkill -9 ""{0}""", CgminerProcessName);
        }
    }
}