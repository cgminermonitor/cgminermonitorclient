using System.IO;
using System.Text;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;

namespace CgminerMonitorClient.Configuration
{
    public class Config
    {
        public string ClientVersion { get; set; }

        public string CgminerIp { get; set; }
        public int CgminerPort { get; set; }

        public string WorkerApiKey { get; set; }
        public string CgminerProcessName { get; set; }

        public ControlConfig ControlOptions { get; set; }

        [JsonIgnore]
        public RunOptions RunOptions { get; set; }

        public Config()
        {
            CgminerIp = "127.0.0.1";
            ControlOptions = new ControlConfig();
        }

        public void Save(string configFileName)
        {
            File.WriteAllText(configFileName, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.UTF8);
        }

        public static Config Read(string configFileName)
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFileName));
            if (string.IsNullOrEmpty(config.ClientVersion) || string.CompareOrdinal(VersionUtil.GetAsString(), config.ClientVersion) != 0) //if config version is different than client version
            {
                config.ClientVersion = VersionUtil.GetAsString(); //update client version
                config.Save(configFileName); //and what's important save config file, creating new json options in file
            }
            return config;
        }
    }
}