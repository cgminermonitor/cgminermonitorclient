using Newtonsoft.Json;

namespace CgminerMonitorClient
{
    public class Config
    {
        public int CgminerPort { get; set; }
        public string WorkerApiKey { get; set; }

        [JsonIgnore]
        public RunOptions RunOptions { get; set; }
    }
}