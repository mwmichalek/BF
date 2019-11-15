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


        //*********************************** CACHING ******************************************

        private void ConnectionStateHandler(ComponentStateChange<ConnectionState> connectionStateChange) {

            var bfState = new BFState {
                SsrStates = CurrentComponentStates<SsrState>(),
                ThermometerStates = CurrentComponentStates<ThermometerState>(),
                PidControllerStates = CurrentComponentStates<PidControllerState>(),
                PumpStates = CurrentComponentStates<PumpState>(),
            };


            ComponentStateChangeFiring(new ComponentStateChange<BFState> {
                CurrentState = bfState
            });
            Logger.LogInformation($"Send entire buttload: {_applicationConfig.Device}");
        }

        private void BFStateHandler(ComponentStateChange<BFState> bfStateChange) {
            var bfState = bfStateChange.CurrentState;
            _componentStateCacheLookup[typeof(SsrState)] = bfState.SsrStates.ToDictionary(ss => ss.Id, ss => (ComponentState)ss);
            _componentStateCacheLookup[typeof(ThermometerState)] = bfState.ThermometerStates.ToDictionary(ss => ss.Id, ss => (ComponentState)ss);
            _componentStateCacheLookup[typeof(PidControllerState)] = bfState.PidControllerStates.ToDictionary(ss => ss.Id, ss => (ComponentState)ss);
            _componentStateCacheLookup[typeof(PumpState)] = bfState.PumpStates.ToDictionary(ss => ss.Id, ss => (ComponentState)ss);


            //CurrentState = bfStateChange.CurrentState;

            //foreach (var stateChange in CurrentState.ComponentStateChanges<SsrState>())
            //    ComponentStateChangeFiring(stateChange);

            //foreach (var stateChange in CurrentState.ComponentStateChanges<ThermometerState>())
            //    ComponentStateChangeFiring(stateChange);

            //foreach (var stateChange in CurrentState.ComponentStateChanges<PidControllerState>())
            //    ComponentStateChangeFiring(stateChange);

            //foreach (var stateChange in CurrentState.ComponentStateChanges<PumpState>())
            //    ComponentStateChangeFiring(stateChange);

            Logger.LogInformation($"Receive entire buttload : {_applicationConfig.Device}");
        }

        private Dictionary<Type, Dictionary<ComponentId, ComponentState>> _componentStateCacheLookup = new Dictionary<Type, Dictionary<ComponentId, ComponentState>>();

        public T CurrentComponentState<T>(ComponentId componentId) where T : ComponentState {
            var cache = ComponentStateCache<T>();
            if (cache.ContainsKey(componentId))
                return (T)cache[componentId];
            return null;
        }

        public IList<T> CurrentComponentStates<T>() where T : ComponentState {
            return ComponentStateCache<T>().Select(cs => (T)cs.Value).ToList();
        }

        private Dictionary<ComponentId, ComponentState> ComponentStateCache<T>() {
            if (_componentStateCacheLookup.ContainsKey(typeof(T)))
                return _componentStateCacheLookup[typeof(T)];
            var componentStateCache = new Dictionary<ComponentId, ComponentState>();
            _componentStateCacheLookup[typeof(T)] = componentStateCache;
            return componentStateCache;
        }

        private void CacheComponentStateChange<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState {
            ComponentStateCache<T>()[componentStateChange.Id] = componentStateChange.CurrentState;
        }

        //*********************************** EVENT HANDLING ******************************************

        public virtual void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState {
            CacheComponentStateChange<T>(componentStateChange);
            _eventAggregator.GetEvent<ComponentStateChangeEvent<ComponentStateChange<T>>>().Publish(componentStateChange);
        }

        public void ComponentStateChangeOccured<T>(Action<ComponentStateChange<T>> componentStateChangeHandler, 
                                                ThreadType threadType = ThreadType.PublisherThread) where T : ComponentState {
            _eventAggregator.GetEvent<ComponentStateChangeEvent<ComponentStateChange<T>>>().Subscribe(componentStateChangeHandler, 
                threadType.ToThreadOption());
        }

        public virtual void ComponentStateRequestFiring<T>(ComponentStateRequest<T> componentStateRequest) where T : RequestedComponentState {
            _eventAggregator.GetEvent<ComponentStateRequestEvent<ComponentStateRequest<T>>>().Publish(componentStateRequest);
        }

        public void ComponentStateRequestOccured<T>(Action<ComponentStateRequest<T>> componentStateRequestHandler,
                                                ThreadType threadType = ThreadType.PublisherThread) where T : RequestedComponentState {
            _eventAggregator.GetEvent<ComponentStateRequestEvent<ComponentStateRequest<T>>>().Subscribe(componentStateRequestHandler,
                threadType.ToThreadOption());
        }

    }

}
