using System;
using System.Collections.Generic;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;

namespace CgminerMonitorClient.Workers.Cgminer
{
    public class CgminerWorker : AbstractWorker
    {
        //read-only commands: "devs", "version", "config", "summary", "pools" 
        private static readonly List<string> StatCommandsList = new List<string> { "coin", "devs", "pools", "summary", "version" };

        public CgminerWorker(string statisticsKey) : base(statisticsKey)
        {
        }

        protected override bool CheckAvailability()
        {
            return true;
        }

        protected override string GetStats(Config config)
        {
            var result = new Dictionary<string, string>();
            foreach (var cmd in StatCommandsList)
            {
                var cgminerCommand = string.Format(Consts.CgminerCommandTemplate, cmd, string.Empty);
                Log.Instance.DebugFormat("Sending {0} to cgminer.", cgminerCommand);
                try
                {
                    var response = CommandSender.SendMessage(cgminerCommand, config.CgminerIp, config.CgminerPort);
                    if (string.IsNullOrEmpty(response))
                    {
                        Log.Instance.DebugFormat("'{0}' command sent to cgminer resulted in empty response.", cgminerCommand);
                        return null;
                    }
                    result.Add(cmd, response);
                }
                catch (Exception e)
                {
                    Log.Instance.Info("Error occured while sending request to cgminer.");
                    Log.Instance.Debug("Error occured while sending request to cgminer.", e);
                    return null;
                }
            }
            var stats = JsonConvert.SerializeObject(result);
            return stats;
        }

        protected override void PreInit(Config config)
        {
        }
    }
}