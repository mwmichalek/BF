using BF.Common.Components;
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

    public class Thermometer : IComponent {

        private ILogger Logger { get; set; }

        public List<ThermometerChange> ThermometerChanges = new List<ThermometerChange>();

        public ComponentId Id { get; private set; }

        public double Change { get; set; }

        private double _changeThreshold = 0.0d;

        private int _changeWindowInMillis = 1000;

        private int _changeEventRetentionInMins = 60 * 6;

        private IBeerFactoryEventHandler _eventHandler;

        public Thermometer(IBeerFactoryEventHandler eventHandler, ComponentId id) {
            _eventHandler = eventHandler;
            Logger = Log.Logger;
            Id = id;
            RegisterEvents();
        }

        public Thermometer(IBeerFactoryEventHandler eventHandler, ComponentId id, int changeThreshold, int changeWindowInMillis, int changeEventRetentionInMins) {
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

        private double _temperature;

        public double Temperature {
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
                var previousChange = ThermometerChanges.Where(tc => tc.Timestamp < earliestTimeOfChange).OrderByDescending(tc => tc.Timestamp).FirstOrDefault();
                if (previousChange != null)
                    Change = thermometerChange.Value - previousChange.Value;

                // Determine Retention
                var oldestTimeOfChange = DateTime.Now.AddMinutes(-_changeEventRetentionInMins);
                var changesToRemove = ThermometerChanges.RemoveAll(tc => tc.Timestamp < oldestTimeOfChange);

                ThermometerChanges.Add(thermometerChange);

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

}
