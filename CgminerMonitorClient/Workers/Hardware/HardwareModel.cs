using System;
using System.Collections.Generic;
using CgminerMonitorClient.CgminerMonitor.Common;
using CgminerMonitorClient.Utils;
using Newtonsoft.Json;
using OpenHardwareMonitor.Hardware;

namespace CgminerMonitorClient.Workers.Hardware
{
    public class ComputerModel : IAmAveragableData
    {
        public ComputerModel()
        {
            Hardware = new Dictionary<NameTypeKey, HardwareModel>();
        }

        [JsonProperty(PropertyName = "H")]
        public Dictionary<NameTypeKey, HardwareModel> Hardware { get; set; }

        public void SumWithTheOther(IAmAveragableData data)
        {
            var cm = (ComputerModel) data;
            foreach (var theOtherHardwareModel in cm.Hardware)
            {
                if (Hardware.ContainsKey(theOtherHardwareModel.Key))
                {
                    var item = Hardware[theOtherHardwareModel.Key];
                    item.SumWithTheOther(theOtherHardwareModel.Value);
                }
                else
                    Hardware.Add(theOtherHardwareModel.Key, theOtherHardwareModel.Value);
            }
        }

        public void DivideByInt(int i)
        {
            foreach (var hardwareModel in Hardware)
            {
                hardwareModel.Value.DivideByInt(i);
            }
        }

        public IAmAveragableData Clone()
        {
            string serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<ComputerModel>(serialized);
        }

        public static ComputerModel FromComputer(IComputer computer)
        {
            var result = new ComputerModel();
            foreach (var hardware in computer.Hardware)
            {
                var suffix = 0;
                var key = new NameTypeKey(hardware.Name, (int) hardware.HardwareType);
                while (result.Hardware.ContainsKey(key))
                {
                    var hardwareName = string.Format("{0} ({1})", hardware.Name, suffix++);
                    key = new NameTypeKey(hardwareName, (int) hardware.HardwareType);
                }
                result.Hardware.Add(key, HardwareModel.FromHardware(hardware));
            }
            return result;
        }
    }

    public class HardwareModel : IAmAveragableData
    {
        public HardwareModel()
        {
            Sensors = new Dictionary<NameTypeKey, SensorModel>();
        }

        [JsonProperty(PropertyName = "S")]
        public Dictionary<NameTypeKey, SensorModel> Sensors { get; set; }

        public void SumWithTheOther(IAmAveragableData data)
        {
            var hm = (HardwareModel) data;
            foreach (var theOtherHardwareModel in hm.Sensors)
            {
                if (Sensors.ContainsKey(theOtherHardwareModel.Key))
                {
                    SensorModel item = Sensors[theOtherHardwareModel.Key];
                    item.SumWithTheOther(theOtherHardwareModel.Value);
                }
                else
                    Sensors.Add(theOtherHardwareModel.Key, theOtherHardwareModel.Value);
            }
        }

        public void DivideByInt(int i)
        {
            foreach (var sensor in Sensors)
            {
                sensor.Value.DivideByInt(i);
            }
        }

        public IAmAveragableData Clone()
        {
            throw new NotImplementedException("This method should't be needed because JsonConverter is used.");
        }

        public static HardwareModel FromHardware(IHardware hardware)
        {
            var result = new HardwareModel();
            foreach (var sensor in hardware.Sensors)
            {
                var suffix = 0;
                var key = new NameTypeKey(sensor.Name, (int)sensor.SensorType);
                while (result.Sensors.ContainsKey(key))
                {
                    var sensorName = string.Format("{0} ({1})", sensor.Name, suffix++);
                    key = new NameTypeKey(sensorName, (int)sensor.SensorType);
                }
                result.Sensors.Add(key, SensorModel.FromSensor(sensor));
            }
            return result;
        }
    }

    public class SensorModel : IAmAveragableData
    {
        [JsonProperty(PropertyName = "V")]
        public float? Value { get; set; }

        public void SumWithTheOther(IAmAveragableData data)
        {
            var sm = (SensorModel) data;
            if (sm == null) throw new ArgumentNullException("data");
            if (Value.HasValue && sm.Value.HasValue)
            {
                Value += sm.Value.Value;
            }
            else
            {
                if (Value.HasValue)
                    return;
                if (sm.Value.HasValue)
                    Value = sm.Value;
            }
        }

        public void DivideByInt(int i)
        {
            if (Value.HasValue)
                Value /= i;
        }

        public IAmAveragableData Clone()
        {
            throw new NotImplementedException("This method should't be needed because JsonConverter is used.");
        }

        public static SensorModel FromSensor(ISensor sensor)
        {
            if (sensor.Value.HasValue)
            {
                var result = new SensorModel
                {
                    Value = (float) Math.Round(sensor.Value.Value, 2),
                };
                return result;
            }
            return new SensorModel();
        }
    }

    public class NameTypeKey : Tuple<string, int>
    {
        public NameTypeKey(string name, int type) : base(name.Replace(',', '\'').Replace("(", "[").Replace(")", "]"), type)
        {
        }
    }
}