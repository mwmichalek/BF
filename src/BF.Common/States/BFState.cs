using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BF.Common.States {

    public class BFState : UpdateableComponentState {

        private IDictionary<Type, IDictionary<ComponentId, ComponentState>> _currentComponentStates = new Dictionary<Type, IDictionary<ComponentId, ComponentState>>();

        public ComponentState CurrentState<T>(ComponentId componentId) where T : ComponentState {
            var currentStates = CurrentStates<T>();
            return currentStates[componentId];
        }

        public IDictionary<ComponentId, ComponentState> CurrentStates<T>() where T : ComponentState {
            if (_currentComponentStates.ContainsKey(typeof(T)))
                return _currentComponentStates[typeof(T)];
            var newCurrentStates = new Dictionary<ComponentId, ComponentState>();
            _currentComponentStates[typeof(T)] = newCurrentStates;
            return newCurrentStates;
        }

        public IList<ComponentStateChange<T>> ComponentStateChanges<T>() where T : ComponentState {
            var currentStates = CurrentStates<T>();
            return currentStates.Values.Select(cs => new ComponentStateChange<T> {
                CurrentState = (T)cs
            }).ToList();
        }

        public void UpdateCurrentState<T>(T componentState) where T : ComponentState {
            var currentStates = CurrentStates<T>();

            // TODO: If this is a temperature, save a bunch of them, otherwise replace

            currentStates[componentState.Id] = componentState;
        }

    }
}
