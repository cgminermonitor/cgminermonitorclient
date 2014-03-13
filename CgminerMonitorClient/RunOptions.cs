namespace CgminerMonitorClient
{
    public class RunOptions
    {
        public RunOptions()
        {
            ConfigFileName = Consts.DefaultConfigFileName;
        }

        public string ConfigFileName { get; private set; }

        public void SetConfigFileName(string configFileName)
        {
            if (!string.IsNullOrEmpty(configFileName))
                ConfigFileName = configFileName;
        }
    }
}