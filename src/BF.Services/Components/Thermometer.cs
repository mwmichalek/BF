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

namespace BF.Services.Components {

    //public interface IThermometer {

    //    ComponentId Id { get; }

    //    ThermometerState CurrentState { get; }
    //}

    public class Thermometer : ComponentBase<ThermometerState> {

        public List<ThermometerState> PreviousStates { get; set; } = new List<ThermometerState>();

        private ILogger Logger { get; set; }

        public double Change { get; set; }

        private double _changeThreshold = 0.0d;

        private int _changeWindowInMillis = 1000;

        private int _changeEventRetentionInMins = 60 * 6;

        private IBeerFactoryEventHandler _eventHandler;

        public Thermometer(ComponentId id, 
                           IBeerFactoryEventHandler eventHandler,
                           ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Thermometer>();

            CurrentState = new ThermometerState { Id = id };

            _eventHandler.ComponentStateChangeOccured<ThermocoupleState>(ThermocoupleStateChangeOccured);
        }

        public Thermometer(ComponentId id, 
                           int changeThreshold, 
                           int changeWindowInMillis, 
                           int changeEventRetentionInMins, 
                           IBeerFactoryEventHandler eventHandler, 
                           ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Thermometer>();

            CurrentState = new ThermometerState { Id = id };

            _changeThreshold = changeThreshold;
            _changeWindowInMillis = changeWindowInMillis;
            _changeWindowInMillis = changeWindowInMillis;

            _eventHandler.ComponentStateChangeOccured<ThermocoupleState>(ThermocoupleStateChangeOccured);
        }

        public double Temperature {
            get { return (CurrentState != null) ? CurrentState.Temperature : double.MinValue; }
        }

        public DateTime Timestamp { get; set; }


        private void ThermocoupleStateChangeOccured(ComponentStateChange<ThermocoupleState> thermocoupleStateChange) {

            if (thermocoupleStateChange.Id == Id) {
                //var 

                var currentState = new ThermometerState {
                    Id = Id,
                    Temperature = thermocoupleStateChange.CurrentState.Temperature,
                    Timestamp = thermocoupleStateChange.CurrentState.Timestamp
                };

                // Archive old shit.
                if (CurrentState != null) {
                    PreviousStates.Add(currentState);
                    PriorState = CurrentState;
                }
                                                                          
                CurrentState = currentState;

                _eventHandler.ComponentStateChangeFiring<ThermometerState>(new ComponentStateChange<ThermometerState> {
                    CurrentState = CurrentState.Clone(),
                    PriorState = PriorState.Clone()
                });

                //Logger.Information($"ThermometerChangeOccured[{Id}] : {thermometerChange.Value}");


                // Determin Change - Get all changes at least this old, order by newest, take first
                // TODO: Refactor to use previous state 
                //var earliestTimeOfChange = DateTime.Now.AddMilliseconds(-_changeWindowInMillis);
                //var previousChange = ThermometerStates.Where(tc => tc.Timestamp < earliestTimeOfChange).OrderByDescending(tc => tc.Timestamp).FirstOrDefault();

                //if (thermometerStateChange.PriorState != null)
                //    Change = thermometerStateChange.CurrentState.Temperature - thermometerStateChange.PriorState.Temperature;

                // Determine Retention
                // TODO: Move this to a background thread
                var oldestTimeOfChange = DateTime.Now.AddMinutes(-_changeEventRetentionInMins);
                var changesToRemove = PreviousStates.RemoveAll(tc => tc.Timestamp < oldestTimeOfChange);


            }
        }

        private void RemoveOldStates() {

        }

    }

}
