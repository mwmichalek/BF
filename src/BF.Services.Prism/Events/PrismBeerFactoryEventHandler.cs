using BF.Common.Components;
using BF.Common.Events;
using BF.Common.States;
using BF.Service.Events;
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

        public PrismBeerFactoryEventHandler(IEventAggregator eventAggregator, ILoggerFactory loggerFactory) {
            _eventAggregator = eventAggregator;
            Logger = loggerFactory.CreateLogger<IBeerFactoryEventHandler>();
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
