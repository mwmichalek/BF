using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BF.Common.States {

    public class BFRequestState : RequestedComponentState {

    }

    public class BFState : ConfigurableComponentState {

        public Dictionary<ComponentId, ThermometerState> TemperatureStates { get; set; } = new Dictionary<ComponentId, ThermometerState>();

        public Dictionary<ComponentId, SsrState> SsrStates { get; set; } = new Dictionary<ComponentId, SsrState>();

        public Dictionary<ComponentId, PidControllerState> PidControllerStates { get; set; } = new Dictionary<ComponentId, PidControllerState>();

        public Dictionary<ComponentId, PumpState> PumpStates { get; set; } = new Dictionary<ComponentId, PumpState>();

        public List<T> CurrentStates<T>() where T : ComponentState {
            if (typeof(T) == typeof(ThermometerState))
                return TemperatureStates.Select(s => (T)(object)s.Value).ToList();
            if (typeof(T) == typeof(SsrState))
                return SsrStates.Select(s => (T)(object)s.Value).ToList();
            if (typeof(T) == typeof(PidControllerState))
                return PidControllerStates.Select(s => (T)(object)s.Value).ToList();
            if (typeof(T) == typeof(PumpState))
                return PumpStates.Select(s => (T)(object)s.Value).ToList();

            return null;
        }

        public ComponentState CurrentState<T>(ComponentId componentId) where T : ComponentState {
            return CurrentStates<T>().SingleOrDefault(c => c.Id == componentId);
        }

        public IList<ComponentStateChange<T>> ComponentStateChanges<T>() where T : ComponentState {
            var currentStates = CurrentStates<T>();
            return currentStates.Select(cs => new ComponentStateChange<T> {
                CurrentState = (T)cs
            }).ToList();
        }

        public void UpdateCurrentState<T>(T componentState) where T : ComponentState {
            if (typeof(T) != GetType()) {
                if (typeof(T) == typeof(ThermometerState))
                    TemperatureStates[componentState.Id] = (ThermometerState)(object)componentState;
                if (typeof(T) == typeof(SsrState))
                    SsrStates[componentState.Id] = (SsrState)(object)componentState;
                if (typeof(T) == typeof(PidControllerState))
                    PidControllerStates[componentState.Id] = (PidControllerState)(object)componentState;
                if (typeof(T) == typeof(PumpState))
                    PumpStates[componentState.Id] = (PumpState)(object)componentState;

            }
        }

    }
}
