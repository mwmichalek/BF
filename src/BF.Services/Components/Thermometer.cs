﻿using BF.Common.Components;
using BF.Common.Events;
using BF.Common.Ids;
using BF.Common.States;
using BF.Service.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Components {

    public class Thermometer : IComponent {

        private ILogger Logger { get; set; }

        public List<ThermometerState> ThermometerStates = new List<ThermometerState>();

        public ComponentId Id { get; private set; }

        public double Change { get; set; }

        private double _changeThreshold = 0.0d;

        private int _changeWindowInMillis = 1000;

        private int _changeEventRetentionInMins = 60 * 6;

        private IBeerFactoryEventHandler _eventHandler;

        public Thermometer(IBeerFactoryEventHandler eventHandler, ComponentId id, ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Thermometer>();
            Id = id;
            RegisterEvents();
        }

        public Thermometer(IBeerFactoryEventHandler eventHandler, ComponentId id, 
            int changeThreshold, int changeWindowInMillis, int changeEventRetentionInMins, 
            ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Thermometer>();
            _changeThreshold = changeThreshold;
            _changeWindowInMillis = changeWindowInMillis;
            _changeWindowInMillis = changeWindowInMillis;
            Id = id;
            RegisterEvents();
        }

        private void RegisterEvents() {
            //_eventHandler.ThermometerChangeOccured(ThermometerChangeOccured);
            _eventHandler.ComponentStateChangeOccured<ThermometerState>(ThermometerStateChangeOccured);
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

        //public void ThermometerChangeOccured(ThermometerChange thermometerChange) {
        private void ThermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) {

            var thermometerChange = thermometerStateChange.CurrentState;
            if (thermometerStateChange.Id == Id) {
                //Logger.Information($"ThermometerChangeOccured[{Id}] : {thermometerChange.Value}");

                Temperature = thermometerChange.Temperature;
  
                // Determin Change - Get all changes at least this old, order by newest, take first
                // TODO: Refactor to use previous state 
                var earliestTimeOfChange = DateTime.Now.AddMilliseconds(-_changeWindowInMillis);
                var previousChange = ThermometerStates.Where(tc => tc.Timestamp < earliestTimeOfChange).OrderByDescending(tc => tc.Timestamp).FirstOrDefault();
                if (previousChange != null)
                    Change = thermometerChange.Temperature - previousChange.Temperature;

                // Determine Retention
                // TODO: Move this to a background thread
                var oldestTimeOfChange = DateTime.Now.AddMinutes(-_changeEventRetentionInMins);
                var changesToRemove = ThermometerStates.RemoveAll(tc => tc.Timestamp < oldestTimeOfChange);

                ThermometerStates.Add(thermometerChange);

                // If change is big enough, broadcast Temperature Change
                if (Math.Abs(Change) > _changeThreshold) {
                    //Logger.Information($"Id:{thermometerChange.Id}, Value:{thermometerChange.Value}, Change:{Change}");
                    _eventHandler.TemperatureChangeFired(new TemperatureChange {
                        Id = Id,
                        Change = Change,
                        Value = thermometerChange.Temperature,
                        PercentChange = Change / previousChange.Temperature * 100,
                        Timestamp = thermometerChange.Timestamp
                    });
                } else if (previousChange == null) { // First event 
                    //Logger.Information($"First Id:{thermometerChange.Id}, Value:{thermometerChange.Value}, Change:{Change}");
                    _eventHandler.TemperatureChangeFired(new TemperatureChange {
                        Id = Id,
                        Change = Change,
                        Value = thermometerChange.Temperature,
                        PercentChange = 0,
                        Timestamp = thermometerChange.Timestamp
                    });
                }
        
            }
        }

        private void RemoveOldStates() {

        }

    }

}
