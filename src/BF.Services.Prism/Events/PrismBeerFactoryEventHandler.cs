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
       
        private BFState CurrentState { get; set; } = new BFState();

        private void ConnectionStateHandler(ComponentStateChange<ConnectionState> connectionStateChange) {

            ComponentStateChangeFiring(new ComponentStateChange<BFState> {
                CurrentState = CurrentState
            });

            Logger.LogInformation($"Send entire buttload: {_applicationConfig.Device}");
        }

        private void BFStateHandler(ComponentStateChange<BFState> bfStateChange) {
            CurrentState = bfStateChange.CurrentState;

            foreach (var stateChange in CurrentState.ComponentStateChanges<SsrState>())
                ComponentStateChangeFiring(stateChange);

            foreach (var stateChange in CurrentState.ComponentStateChanges<ThermometerState>())
                ComponentStateChangeFiring(stateChange);

            foreach (var stateChange in CurrentState.ComponentStateChanges<PidControllerState>())
                ComponentStateChangeFiring(stateChange);

            foreach (var stateChange in CurrentState.ComponentStateChanges<PumpState>())
                ComponentStateChangeFiring(stateChange);

            Logger.LogInformation($"Receive entire buttload : {_applicationConfig.Device}");
        }

        public ComponentState CurrentComponentState<T>(ComponentId componentId) where T : ComponentState {
            return CurrentState.CurrentState<T>(componentId);
        }

        public IList<ComponentState> CurrentComponentStates<T>() where T : ComponentState {
            return CurrentState.CurrentStates<T>().Values.ToList();
        }

        //*****************************************************************************

        public virtual void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState {
            CurrentState.UpdateCurrentState<T>(componentStateChange.CurrentState);
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
