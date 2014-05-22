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

        public void GenerateCgminerKillCmdBasedOnCgminerProcessName()
        {
            if (PlatformCheck.AreWeRunningUnderWindows())
                CgminerKillCmd = string.Format(@"taskkill /F /IM ""{0}""", CgminerProcessName);
            else
                CgminerKillCmd = string.Format(@"pkill -9 ""{0}""", CgminerProcessName);
        }
    }
}