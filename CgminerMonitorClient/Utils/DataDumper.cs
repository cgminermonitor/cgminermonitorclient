using System;
using System.IO;
using CgminerMonitorClient.CgminerMonitor.Common;
using Newtonsoft.Json;

namespace CgminerMonitorClient.Utils
{
    public class DataDumper
    {
        private bool _dumpPerformed;
        private readonly string _dumpFileName;
        private readonly string _statsKey;

        public DataDumper(string dumpFileName, string statsKey)
        {
            Log.Instance.DebugFormat("Starting '{0}' data dumper", statsKey);

            if (!string.IsNullOrEmpty(dumpFileName))
                _dumpFileName = dumpFileName + "_" + statsKey;
            _statsKey = statsKey;
        }

        public void MakeDumpIfNeeded(StatisticsInsertMessage message)
        {
            if (_dumpPerformed)
                return;
            if (string.IsNullOrEmpty(_dumpFileName))
                return;

            try
            {
                File.WriteAllText(_dumpFileName, JsonConvert.SerializeObject(message).Replace(message.ApiKey, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"));
                Log.Instance.InfoFormat("'{0}' dump was saved in: {1}.", _statsKey, _dumpFileName);
                _dumpPerformed = true;
            }
            catch (Exception e)
            {
                Log.Instance.Info("Could not make dump file.", e);
            }
        }
    }
}