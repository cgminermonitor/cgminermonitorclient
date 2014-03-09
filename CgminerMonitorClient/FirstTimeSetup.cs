using System;
using System.IO;
using System.Text;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;

namespace CgminerMonitorClient
{
    public class FirstTimeSetup
    {
        public void Execute(string configFileName)
        {
            var config = new Config();
            Log.Instance.Info("First time setup!");
            //TODO: read/write in current directory test
            Log.Instance.Info("\t1. Register on site cgminermonitor.com");
            Log.Instance.Info("\t2. Add a new worker under worker page");
            Log.Instance.Info("\t3. Now, type the api key of that worker:");
            while (true)
            {
                string probableApiKey = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(probableApiKey) || probableApiKey.Length != 32)
                    Console.WriteLine("Api key should have the lenght of 32 characters. Try again.");
                else
                {
                    config.WorkerApiKey = probableApiKey;
                    break;
                }
            }
            Log.Instance.Info("\t4. Configure cgminer:");
            Log.Instance.InfoRaw(@"Pass additional command-line arguments: 
        --api-listen --api-allow W:127.0.0.1
Or add additional configuration file entries: 
        ""api-listen"" : true,
        ""api-allow"" : ""W:127.0.0.1""
Press enter when you are done.");
            Log.Instance.Info("\t5. If you changed default API port, type it now. Otherwise press enter.");
            var port = 4028;
            while (true)
            {
                var probablePort = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(probablePort))
                {
                    int parsedPort;
                    if (int.TryParse(probablePort, out parsedPort))
                    {
                        port = parsedPort;
                        break;
                    }
                    Console.WriteLine("Enter a port number or leave the line empty.");
                }
                else
                {
                    break;
                }
            }
            config.CgminerPort = port;
            Log.Instance.Info("Thats all.");
            File.WriteAllText(configFileName, JsonConvert.SerializeObject(config, Formatting.Indented), Encoding.UTF8);
        }
    }
}