using Newtonsoft.Json;

namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public class StatisticsInsertMessage
    {
        public string ApiKey { get; private set; }
        public ClientMetadata ClientMetadata { get; private set; }
        [JsonProperty("wcr")]
        public WorkerCommandResponse WorkerCommandResponse { get; set; }
        [JsonProperty("cpir")]
        public bool? CgminerProcessIsRunning { get; set; }
        public string Stats { get; set; }
        public string StatsKey { get; private set; }
        public string StatTime { get; set; }

        public StatisticsInsertMessage(string apiKey, string statsKey)
        {
            ApiKey = apiKey;
            StatsKey = statsKey;

            ClientMetadata = ClientMetadata.GetCurrentClientMetadata();
        }
    }
}