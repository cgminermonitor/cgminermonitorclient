using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Configuration;
using CgminerMonitorClient.Utils;
using CgminerMonitorClient.Workers;
using CgminerMonitorClient.Workers.Cgminer;
using CgminerMonitorClient.Workers.Control;
using CgminerMonitorClient.Workers.Hardware;
using CgminerMonitorClient.Workers.UpdateCheck;
using NDesk.Options;

namespace CgminerMonitorClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var runOptions = new RunOptions();

            var p = new OptionSet
            {
                {"h|?|help", v => ShowHelp()},
                {"t|T|troubleshoot", v => TroubleshootingMode()},
                {"v|verbose|d|debug", v => SetLoggingToVerbose()},
                {"c|curl", v => runOptions.SetUseCurl()},
                {"configFile=", runOptions.SetConfigFileName},
                {"dumpFile=", runOptions.SetDumpFileName},
            };
            p.Parse(args);

            Log.Instance.InfoFormat("Running client: {0}", ClientMetadata.GetCurrentClientMetadata().Dump());
            Log.Instance.DebugFormat("Config file name: {0}", runOptions.ConfigFileName);

            if (!File.Exists(runOptions.ConfigFileName))
                new FirstTimeSetup().Execute(runOptions.ConfigFileName);
            var config = ReadConfig(runOptions.ConfigFileName);
            config.RunOptions = runOptions;

            Log.Instance.InfoFormat("Started process id: {0}.", Process.GetCurrentProcess().Id);

            PipesWarmup();
            StartWorkers(config);

            while (true)
            {
                Thread.Sleep(1000);
            }

// ReSharper disable FunctionNeverReturns //yeah it returns - sometimes with Environment.Exit(0)
        }
// ReSharper restore FunctionNeverReturns

        /// <summary>
        /// access the pipes for the first time (google suggest that this may remove timeout issue in some cases)
        /// </summary>
        private static void PipesWarmup()
        {
            try
            {
                Log.Instance.Debug("Accessing ServicePointManager.DefaultConnectionLimit.");
                int tmp = ServicePointManager.DefaultConnectionLimit;
                Log.Instance.Debug("Accessing ServicePointManager.DefaultConnectionLimit done.");
            }
            catch (Exception e)
            {
                Log.Instance.Info("Error occured while accessing ServicePointManager.DefaultConnectionLimit.", e);
            }
        }

        private static void SetLoggingToVerbose()
        {
            Log.Instance.ToggleLevel(Log.Level.Verbose);
        }

        private static void TroubleshootingMode()
        {
            SetLoggingToVerbose();
            var troubleshooter = new Troubleshooter();
            troubleshooter.Execute();
            Environment.Exit(0);
        }


        private static void StartWorkers(Config config)
        {
            Log.Instance.Debug("Starting workers.");
            var workers = new List<IWorkerDefinition>
            {
                new HardwareWorker("StatHardware"),
                new CgminerWorker("StatCgminer"),
                new ControlWorker("Control"),
                new UpdateCheckWorker(),
            };
            foreach (var worker in workers)
            {
                var thread = new Thread(worker.Start);
                thread.Start(config);
            }
            Log.Instance.Debug("Workers started.");
        }

        private static Config ReadConfig(string configFileName)
        {
            Log.Instance.Debug("Reading config");
            var config = Config.Read(configFileName);
            Log.Instance.Debug("Config read.");
            return config;
        }

        private static void ShowHelp()
        {
            Log.Instance.Info("h|?|help - show this message");
            Log.Instance.Info("t|T|troubleshoot - troubleshooting communication problems");
            Log.Instance.Info("v|verbose|d|debug - show detailed activity");
            Log.Instance.Info("c|curl - use curl to perform POST requests, DO NOT use this, unless you are always getting mysterious timeouts [Only under Linux & requires curl]");
            Log.Instance.Info("configFile - specify config file name instead of default one");
            Log.Instance.Info("dumpFile - save cgminer stats in a dump file (sometimes it is needed to diagnose the problem)");
            Environment.Exit(0);
        }
    }
}