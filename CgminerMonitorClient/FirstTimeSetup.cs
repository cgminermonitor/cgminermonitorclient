﻿using System;
using System.IO;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;

namespace CgminerMonitorClient
{
    public class FirstTimeSetup
    {
        public void Execute(string configFileName)
        {
            var config = new Config
            {
                ClientVersion = VersionUtil.GetAsString()
            };

            Log.Instance.Info("First time setup!");
            Log.Instance.Info("\t1. Register on site cgminermonitor.com");
            Log.Instance.Info("\t2. Add a new worker under worker page");
            Log.Instance.Info("\t3. Now, type the api key of that worker:");
            while (true)
            {
                var probableApiKey = Console.ReadLine();
                if (string.IsNullOrEmpty(probableApiKey) || probableApiKey.Length != 32)
                    Log.Instance.Info("Api key should have the lenght of 32 characters. Try again.");
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
                    Log.Instance.Info("Enter a port number or leave the line empty.");
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
            
            var exampleProcessName = PlatformCheck.AreWeRunningUnderWindows() ? "cgminer.exe" : "cgminer";
            config.CgminerProcessName = GetStringAnswerToQuestion("\t7. What is cgminer process name? Example: " + exampleProcessName + " (may be different when using e.g. vertminer)");

            ConfigureRemoteMinerControl(config.ControlOptions);
            config.ControlOptions.Bootstrap(config.CgminerProcessName);

            CheckLibcSoLinking();
            Log.Instance.Info("Thats all. Starting up!");
            config.Save(configFileName);
        }

        private static void ConfigureRemoteMinerControl(ControlConfig config)
        {
            Log.Instance.Info("\t8. Remote miner control. (Answer with 'y' or 'n')");
            Log.Instance.Info("\t\t NOTE: if you have security concerns please read http://cgminermonitor.com/Faq.");
            var currentClientMetadata = ClientMetadata.GetCurrentClientMetadata();
            config.AllowWorkerPowerControl = GetYnAnswerToQuestion("\t\t Do you want to be able to reboot or shutdown your worker from the website?");
            config.AllowCgminerPowerControl = GetYnAnswerToQuestion("\t\t Do you want to be able to start, stop or restart cgminer from the website?");
            if (config.AllowCgminerPowerControl)
            {
                string startCgminerExample;
                switch (currentClientMetadata.ClientPlatform)
                {
                    case ClientPlatform.Linux:
                        startCgminerExample = "sh -c '/absolute/path/to/mine/script.sh'";
                        break;
                    case ClientPlatform.Windows:
                        startCgminerExample = @"cmd.exe /c C:\absolute\path\to\bash\script\run.bat --some-params, OR C:\absolute\path\to\cgminer.exe";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                config.CgminerStartCmd = GetStringAnswerToQuestion("\t\t\t How to start cgminer? Example: " + startCgminerExample);
            }
            config.AllowCgminerControl = GetYnAnswerToQuestion("\t\t Do you want to be able to control cgminer (switch pools etc.) from the website?");
            config.AllowCgminerConfigReadingAndWriting = GetYnAnswerToQuestion("\t\t Do you want to be able to read/write cgminer config from the website?");
            if (config.AllowCgminerConfigReadingAndWriting)
            {
                string cgminerConfigFileLocationExample;
                switch (currentClientMetadata.ClientPlatform)
                {
                    case ClientPlatform.Linux:
                        cgminerConfigFileLocationExample = "/absolute/path/to/cgminer.config";
                        break;
                    case ClientPlatform.Windows:
                        cgminerConfigFileLocationExample = @"C:\absolute\path\to\cgminer.config";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                config.CgminerConfigFileLocation = GetStringAnswerToQuestion("\t\t\t Where is your cgminer config file? Example: " + cgminerConfigFileLocationExample);
            }
        }

        private static string GetStringAnswerToQuestion(string question)
        {
            Log.Instance.Info(question);
            while (true)
            {
                var probableAnswer = Console.ReadLine();
                if (string.IsNullOrEmpty(probableAnswer))
                    Console.WriteLine("You should answer with something. Try again.");
                else
                {
                    return probableAnswer;
                }
            }
        }

        private static bool GetYnAnswerToQuestion(string question)
        {
            Log.Instance.Info(question);
            while (true)
            {
                var probableAnswer = Console.ReadLine();
                if (string.IsNullOrEmpty(probableAnswer) || (string.CompareOrdinal("y", probableAnswer) != 0 && string.CompareOrdinal("n", probableAnswer) != 0))
                    Console.WriteLine("You should answer with 'y' or 'n'. Try again.");
                else
                {
                    if (string.CompareOrdinal("y", probableAnswer) == 0)
                        return true;
                    if (string.CompareOrdinal("n", probableAnswer) == 0)
                        return false;
                }
            }
        }

        private static void CheckLibcSoLinking()
        {
            var currentClinetMetadata = ClientMetadata.GetCurrentClientMetadata();
            if (currentClinetMetadata.ClientPlatform == ClientPlatform.Windows)
                return;
            if (currentClinetMetadata.Distro == Consts.Bamt1XDistroName || currentClinetMetadata.Distro == Consts.MacOsxDistroName)
                return;
            Log.Instance.Info("\t8. Checking libc.so linking.");
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