using Newtonsoft.Json;

namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public class StatisticsResultMessage
    {
        [JsonProperty("s")]
        public bool Success { get; set; }
        [JsonProperty("em")]
        public string ErrorMessage { get; set; }
        [JsonProperty("ec")]
        public string ErrorCode { get; set; }
        [JsonProperty("c")]
        public WorkerCommand Command { get; set; }
        [JsonProperty("ss")]
        public int SleepSeconds { get; set; }
    }
}