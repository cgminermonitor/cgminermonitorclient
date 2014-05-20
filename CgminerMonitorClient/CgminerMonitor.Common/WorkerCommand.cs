using Newtonsoft.Json;

namespace CgminerMonitorClient.CgminerMonitor.Common
{
    public class WorkerCommand
    {
        [JsonProperty("n")]
        public string HandlerKey { get; set; }

        [JsonProperty("v")]
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", HandlerKey, Value);
        }
    }
}