using System;
using System.IO;
using System.Text;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
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
            Log.Instance.Info("\t1. Register on site cgminermonitor.com");
            Log.Instance.Info("\t2. Add a new worker under worker page");
            Log.Instance.Info("\t3. Now, type the api key of that worker:");
            while (true)
            {
                var probableApiKey = Console.ReadLine();
                if (string.IsNullOrEmpty(probableApiKey) || probableApiKey.Length != 32)
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
                if (!string.IsNullOrEmpty(probablePort))
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
            Log.Instance.Info("\t6. Checking read-write permissions.");
            if (PermissionCheckSucceeded())
                Log.Instance.Info("\t\tAll is good.");
            CheckLibcSoLinking();
            Log.Instance.Info("Thats all. Starting up!");
            File.WriteAllText(configFileName, JsonConvert.SerializeObject(config, Formatting.Indented), Encoding.UTF8);
        }

        private static void CheckLibcSoLinking()
        {
            var currentClinetMetadata = ClientMetadata.GetCurrentClientMetadata();
            if (currentClinetMetadata.ClientPlatform == ClientPlatform.Windows)
                return;
            if (currentClinetMetadata.Distro == Consts.Bamt1XDistroName || currentClinetMetadata.Distro == Consts.MacOsxDistroName)
                return;
            Log.Instance.Info("\t7. Checking libc.so linking.");
            if (File.Exists("libc.so"))
                Log.Instance.Info("\t\tGreat, libc.so found!");
            else
            {
                Log.Instance.Info("libc.so NOT found!");
                switch (currentClinetMetadata.ClientArchitecture)
                {
                    case ClientArchitecture.x64:
                        Log.Instance.Info("Please run command: 'sudo ln -s /lib/x86_64-linux-gnu/libc.so.6 ./libc.so'");
                        break;
                    case ClientArchitecture.x86:
                        Log.Instance.Info("Please run command: 'sudo ln -s /lib/i386-linux-gnu/libc.so.6 ./libc.so'");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Log.Instance.Info(
                    "Be warned! Client will probably fail to run if you don't have Mono installed! Press enter to continue.");
                Console.ReadLine();
            }
        }

        public bool PermissionCheckSucceeded()
        {
            return PermissionCheckSucceeded_CurrentDirectory();
        }

        /// <summary>
        ///     Current directory - running, config
        /// </summary>
        /// <returns></returns>
        private static bool PermissionCheckSucceeded_CurrentDirectory()
        {
            try
            {
                const string exampleConfigFile = "test.txt";
                File.WriteAllText(exampleConfigFile, "test");
                Thread.Sleep(25);
                File.ReadAllText(exampleConfigFile);
                Thread.Sleep(25);
                File.Delete(exampleConfigFile);
            }
            catch (Exception)
            {
                Log.Instance.InfoFormat(
                    "\t\tERROR!!! Cannot write or read in '{0}'. This is necessary and running the client will FAIL!",
                    Environment.CurrentDirectory);
                return false;
            }
            return true;
        }
    }
}