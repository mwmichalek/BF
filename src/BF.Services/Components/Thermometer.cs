using BF.Common.Events;
using BF.Common.Ids;
using BF.Service.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Components {

    public class Thermometer {

        private ILogger Logger { get; set; }

        private List<ThermometerChange> _thermometerChange = new List<ThermometerChange>();

        public ThermometerId Id { get; private set; }

        public decimal Change { get; set; }

        private decimal _changeThreshold = 0.10m;

        private int _changeWindowInMillis = 1000;

        private int _changeEventRetentionInMins = 60 * 6;

        private IBeerFactoryEventHandler _eventHandler;

        public Thermometer(IBeerFactoryEventHandler eventHandler, ThermometerId id) {
            _eventHandler = eventHandler;
            Logger = Log.Logger;
            Id = id;
            RegisterEvents();
        }

        public Thermometer(IBeerFactoryEventHandler eventHandler, ThermometerId id, int changeThreshold, int changeWindowInMillis, int changeEventRetentionInMins) {
            _eventHandler = eventHandler; 
            Logger = Log.Logger;
            _changeThreshold = changeThreshold;
            _changeWindowInMillis = changeWindowInMillis;
            _changeWindowInMillis = changeWindowInMillis;
            Id = id;
            RegisterEvents();
        }

        private void RegisterEvents() {
            _eventHandler.ThermometerChangeOccured(ThermometerChangeOccured);
        }

        private decimal _temperature;

        public decimal Temperature {
            get { return _temperature; }
            set {
                //Change = value - _temperature;
                Timestamp = DateTime.Now;
                _temperature = value;
            }
        }

        public DateTime Timestamp { get; set; }

        public void ThermometerChangeOccured(ThermometerChange thermometerChange) {
            if (thermometerChange.Id == Id) {
                //Logger.Information($"ThermometerChangeOccured[{Id}] : {thermometerChange.Value}");

                Temperature = thermometerChange.Value;
  
                // Determin Change - Get all changes at least this old, order by newest, take first
                var earliestTimeOfChange = DateTime.Now.AddMilliseconds(-_changeWindowInMillis);
                var previousChange = _thermometerChange.Where(tc => tc.Timestamp < earliestTimeOfChange).OrderByDescending(tc => tc.Timestamp).FirstOrDefault();
                if (previousChange != null)
                    Change = thermometerChange.Value - previousChange.Value;

                // Determine Retention
                var oldestTimeOfChange = DateTime.Now.AddMinutes(-_changeEventRetentionInMins);
                var changesToRemove = _thermometerChange.RemoveAll(tc => tc.Timestamp < oldestTimeOfChange);

                _thermometerChange.Add(thermometerChange);

                // If change is big enough, broadcast Temperature Change
                if (Math.Abs(Change) > _changeThreshold) {
                    //Logger.Information($"Id:{thermometerChange.Id}, Value:{thermometerChange.Value}, Change:{Change}");
                    _eventHandler.TemperatureChangeFired(new TemperatureChange {
                        Id = thermometerChange.Id,
                        Change = Change,
                        Value = thermometerChange.Value,
                        PercentChange = Change / previousChange.Value * 100,
                        Timestamp = thermometerChange.Timestamp
                    });
                } else if (previousChange == null) { // First event 
                    //Logger.Information($"First Id:{thermometerChange.Id}, Value:{thermometerChange.Value}, Change:{Change}");
                    _eventHandler.TemperatureChangeFired(new TemperatureChange {
                        Id = thermometerChange.Id,
                        Change = Change,
                        Value = thermometerChange.Value,
                        PercentChange = 0,
                        Timestamp = thermometerChange.Timestamp
                    });
                }
        
            }
        }

    }

    public static class ThermometerHelper {

        public static Thermometer GetById(this List<Thermometer> thermometers, ThermometerId thermometerId) {
            return thermometers.SingleOrDefault(t => t.Id == thermometerId);
        }

    }
}
