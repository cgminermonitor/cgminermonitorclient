using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Utils;
using CgminerMonitorClient.Workers;
using CgminerMonitorClient.Workers.Cgminer;
using CgminerMonitorClient.Workers.Hardware;
using CgminerMonitorClient.Workers.Update;
using NDesk.Options;
using Newtonsoft.Json;

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
                {"configFile=", runOptions.SetConfigFileName},
            };
            p.Parse(args);

            Log.Instance.InfoFormat("Running client: {0}", ClientMetadata.GetCurrentClientMetadata().Dump());
            Log.Instance.DebugFormat("Config file name: {0}", runOptions.ConfigFileName);

            if (!File.Exists(runOptions.ConfigFileName))
                new FirstTimeSetup().Execute(runOptions.ConfigFileName);
            var config = ReadConfig(runOptions.ConfigFileName);

            Log.Instance.InfoFormat("Started process id: {0}.", Process.GetCurrentProcess().Id);
            new CurrentProcessFileNameValidator().Validate();
            StartWorkers(config);

            while (true)
            {
                Thread.Sleep(1000);
            }

// ReSharper disable FunctionNeverReturns //yeah it returns - in updater with Environment.Exit(0)
        }

// ReSharper restore FunctionNeverReturns

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
                new CgminerWorker("StatCgminer")
            };
            if (Consts.Distro != Consts.CustomDistroName)
                workers.Add(new UpdateWorker());
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
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFileName));
            Log.Instance.Debug("Config read.");
            return config;
        }

        private static void ShowHelp()
        {
            Log.Instance.Info("h|?|help - show this message");
            Log.Instance.Info("t|T|troubleshoot - troubleshooting communication problems");
            Log.Instance.Info("v|verbose|d|debug - show detailed activity");
            Log.Instance.Info("configFile - specify config file name instead of default one");
            Environment.Exit(0);
        }
    }
}