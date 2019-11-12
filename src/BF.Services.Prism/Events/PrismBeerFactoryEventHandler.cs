using BF.Common.Components;
using BF.Common.Events;
using BF.Common.States;
using BF.Service.Events;
using BF.Services.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Prism.Events {

    public static class ThreadOptionHelper {

        private static IDictionary<ThreadType, ThreadOption> lookup = new Dictionary<ThreadType, ThreadOption> {
            { ThreadType.BackgroundThread, ThreadOption.BackgroundThread },
            { ThreadType.PublisherThread, ThreadOption.PublisherThread },
            { ThreadType.UIThread, ThreadOption.UIThread }
        };

        public static ThreadOption ToThreadOption(this ThreadType threadType) {
            return lookup[threadType];
        }
    }

    public class PrismBeerFactoryEventHandler : IBeerFactoryEventHandler {

        private ILogger Logger { get; set; }

        protected IEventAggregator _eventAggregator;

        protected IApplicationConfig _applicationConfig { get; set; }

        public PrismBeerFactoryEventHandler(IEventAggregator eventAggregator, ILoggerFactory loggerFactory, IApplicationConfig applicationConfig) {
            _eventAggregator = eventAggregator;
            _applicationConfig = applicationConfig;
            Logger = loggerFactory.CreateLogger<IBeerFactoryEventHandler>();

            if (_applicationConfig.Device == Device.RaspberryPi ||
                _applicationConfig.Device == Device.RaspberryPi_PC)
                this.ComponentStateChangeOccured<ConnectionState>(ConnectionStateHandler);

            if (_applicationConfig.Device == Device.Server ||
                _applicationConfig.Device == Device.Server_PC)
                this.ComponentStateChangeOccured<BFState>(BFStateHandler);
        }

        

        private Dictionary<Tuple<Type, ComponentId>, ComponentState> _currentComponentStates = new Dictionary<Tuple<Type, ComponentId>, ComponentState>();

        public virtual ComponentState CurrentComponentState<T>(ComponentId componentId) where T : ComponentState {
            var key = Tuple.Create(typeof(T), componentId);
            if (_currentComponentStates.ContainsKey(key))
                return _currentComponentStates[key];
            return null;
        }

        public IList<ComponentState> CurrentComponentStates<T>() where T : ComponentState {
            return _currentComponentStates.Values.Where(cs => cs.GetType() == typeof(T)).ToList();
        }

        private void ConnectionStateHandler(ComponentStateChange<ConnectionState> connectionStateChange) {

            var bfState = new BFState {
                SsrStates = _currentComponentStates.ToDictionary<SsrState>(),
                ThermometerStates = _currentComponentStates.ToDictionary<ThermometerState>(),
                PidControllerStates = _currentComponentStates.ToDictionary<PidControllerState>(),
                PumpStates = _currentComponentStates.ToDictionary<PumpState>()
            };
            ComponentStateChangeFiring(new ComponentStateChange<BFState> {
                CurrentState = bfState
            });

            Logger.LogInformation($"Send entire buttload: {_applicationConfig.Device}");
        }

        private void BFStateHandler(ComponentStateChange<BFState> bfStateChange) {
            

            foreach (var state in bfStateChange.CurrentState.SsrStates)
                ComponentStateChangeFiring(state.ToComponentStateChange());

            foreach (var state in bfStateChange.CurrentState.ThermometerStates)
                ComponentStateChangeFiring(state.ToComponentStateChange());

            foreach (var state in bfStateChange.CurrentState.PidControllerStates)
                ComponentStateChangeFiring(state.ToComponentStateChange());

            foreach (var state in bfStateChange.CurrentState.PumpStates)
                ComponentStateChangeFiring(state.ToComponentStateChange());

            Logger.LogInformation($"Receive entire buttload : {_applicationConfig.Device}");
        }

        //*****************************************************************************

        public virtual void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState {
            _currentComponentStates[Tuple.Create(typeof(T), componentStateChange.Id)] = componentStateChange.CurrentState;
            _eventAggregator.GetEvent<ComponentStateChangeEvent<ComponentStateChange<T>>>().Publish(componentStateChange);
        }

        public void ComponentStateChangeOccured<T>(Action<ComponentStateChange<T>> componentStateChangeHandler, 
                                                ThreadType threadType = ThreadType.PublisherThread) where T : ComponentState {
            _eventAggregator.GetEvent<ComponentStateChangeEvent<ComponentStateChange<T>>>().Subscribe(componentStateChangeHandler, 
                threadType.ToThreadOption());
        }

        public virtual void ComponentStateRequestFiring<T>(ComponentStateRequest<T> componentStateRequest) where T : UpdateableComponentState {
            _eventAggregator.GetEvent<ComponentStateRequestEvent<ComponentStateRequest<T>>>().Publish(componentStateRequest);
        }

        public void ComponentStateRequestOccured<T>(Action<ComponentStateRequest<T>> componentStateRequestHandler,
                                                ThreadType threadType = ThreadType.PublisherThread) where T : UpdateableComponentState {
            _eventAggregator.GetEvent<ComponentStateRequestEvent<ComponentStateRequest<T>>>().Subscribe(componentStateRequestHandler,
                threadType.ToThreadOption());
        }

    }


}
