using System;
using CgminerMonitorClient.PostMakers;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient
{
    public class RunOptions
    {
        public string ConfigFileName { get; private set; }

        public string DumpFileName { get; private set; }

        private bool _useCurl;

        public RunOptions()
        {
            ConfigFileName = Consts.DefaultConfigFileName;
        }

        public PostMakerType PostMakerType
        {
            get { return _useCurl ? PostMakerType.Curl : PostMakerType.WebClient; }
        }

        public void SetUseCurl()
        {
            if (PlatformCheck.AreWeRunningUnderWindows())
            {
                Log.Instance.Info("curl option is available only under Linux.");
                Environment.Exit(0);
            }
            _useCurl = true;
        }

        public void SetConfigFileName(string configFileName)
        {
            if (!string.IsNullOrEmpty(configFileName))
                ConfigFileName = configFileName;
        }

        public void SetDumpFileName(string dumpFileName)
        {
            DumpFileName = dumpFileName;
        }
    }
}