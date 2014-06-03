﻿using CgminerMonitorClient.Utils;

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

        public string CgminerConfigFileLocation { get; set; }

        public string WorkerRebootCmd { get; set; }
        public string WorkerShutdownCmd { get; set; }

        public string RootPrivilegesCmd { get; set; }
        public string RootPrivilegesCmdResult { get; set; }

        public void Bootstrap(string cgminerProcessName)
        {
            GenerateCgminerKillCmdBasedOnCgminerProcessName(cgminerProcessName);
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
                WorkerRebootCmd = "shutdown -r 1";
                WorkerShutdownCmd = "shutdown -p 1";
                RootPrivilegesCmd = "whoami";
                RootPrivilegesCmdResult = "root";
            }
        }

        private void GenerateCgminerKillCmdBasedOnCgminerProcessName(string cgminerProcessName)
        {
            if (PlatformCheck.AreWeRunningUnderWindows())
                CgminerKillCmd = string.Format(@"taskkill /F /IM ""{0}""", cgminerProcessName);
            else
                CgminerKillCmd = string.Format(@"pkill -9 ""{0}""", cgminerProcessName);
        }
    }
}