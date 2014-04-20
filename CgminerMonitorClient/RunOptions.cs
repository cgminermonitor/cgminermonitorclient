using System;
using CgminerMonitorClient.PostMakers;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient
{
    public class RunOptions
    {
        private bool _useCurl;

        public RunOptions()
        {
            ConfigFileName = Consts.DefaultConfigFileName;
        }

        public string ConfigFileName { get; private set; }

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
    }
}