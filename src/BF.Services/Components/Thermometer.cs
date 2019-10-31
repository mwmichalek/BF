using BF.Common.Components;
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

    public interface IThermometer {

        ComponentId Id { get; }

        ThermometerState CurrentState { get; }
    }

    public class Thermometer : IComponent, IThermometer {

        public ThermometerState CurrentState { get; set; }

        public List<ThermometerState> PreviousStates { get; set; } = new List<ThermometerState>();

        private ILogger Logger { get; set; }


        public ComponentId Id { get; private set; }

        public double Change { get; set; }

        private double _changeThreshold = 0.0d;

        private int _changeWindowInMillis = 1000;

        private int _changeEventRetentionInMins = 60 * 6;

        private IBeerFactoryEventHandler _eventHandler;

        

        public Thermometer(ComponentId id, IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Thermometer>();
            Id = id;

            _eventHandler.ComponentStateChangeOccured<ThermometerState>(ThermometerStateChangeOccured);
        }

        public Thermometer(ComponentId id, 
                           int changeThreshold, 
                           int changeWindowInMillis, 
                           int changeEventRetentionInMins, 
                           IBeerFactoryEventHandler eventHandler, 
                           ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Thermometer>();

            _changeThreshold = changeThreshold;
            _changeWindowInMillis = changeWindowInMillis;
            _changeWindowInMillis = changeWindowInMillis;

            Id = id;

            _eventHandler.ComponentStateChangeOccured<ThermometerState>(ThermometerStateChangeOccured);
        }

        public double Temperature {
            get { return (CurrentState != null) ? CurrentState.Temperature : double.MinValue; }
        }

        public DateTime Timestamp { get; set; }


        private void ThermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) {

            CurrentState = thermometerStateChange.CurrentState;

            if (thermometerStateChange.Id == Id) {
                //Logger.Information($"ThermometerChangeOccured[{Id}] : {thermometerChange.Value}");
                
  
                // Determin Change - Get all changes at least this old, order by newest, take first
                // TODO: Refactor to use previous state 
                //var earliestTimeOfChange = DateTime.Now.AddMilliseconds(-_changeWindowInMillis);
                //var previousChange = ThermometerStates.Where(tc => tc.Timestamp < earliestTimeOfChange).OrderByDescending(tc => tc.Timestamp).FirstOrDefault();

                if (thermometerStateChange.PriorState != null)
                    Change = thermometerStateChange.CurrentState.Temperature - thermometerStateChange.PriorState.Temperature;

                // Determine Retention
                // TODO: Move this to a background thread
                var oldestTimeOfChange = DateTime.Now.AddMinutes(-_changeEventRetentionInMins);
                var changesToRemove = PreviousStates.RemoveAll(tc => tc.Timestamp < oldestTimeOfChange);

                PreviousStates.Add(CurrentState);
            }
        }

        private void RemoveOldStates() {

        }

    }

}
