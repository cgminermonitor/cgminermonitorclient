using System.IO;
using System.Text;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;

namespace CgminerMonitorClient
{
    public class Config
    {
        public string ClientVersion { get; set; }

        public int CgminerPort { get; set; }
        public string WorkerApiKey { get; set; }

        public bool AllowWorkerPowerControl { get; set; }
        public bool AllowCgminerPowerControl { get; set; }
        public string CgminerStartCmd { get; set; }
        public bool AllowCgminerConfigReadingAndWriting { get; set; }
        public string CgminerConfigFileLocation { get; set; }
        public bool AllowCgminerControl { get; set; }

        [JsonIgnore]
        public RunOptions RunOptions { get; set; }

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