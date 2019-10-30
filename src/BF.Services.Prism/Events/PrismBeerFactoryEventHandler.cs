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

        public void InitializationChangeOccured(Action<InitializationChange> initializationChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
            _eventAggregator.GetEvent<InitializationChangeEvent>().Subscribe(initializationChangeHandler, threadType.ToThreadOption());
        }

        //public void TemperatureChangeOccured(Action<TemperatureChange> temperatureChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
        //    _eventAggregator.GetEvent<TemperatureChangeEvent>().Subscribe(temperatureChangeHandler, threadType.ToThreadOption());
        //}

        //public void ThermometerChangeOccured(Action<ThermometerChange> thermometerChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
        //    _eventAggregator.GetEvent<ThermometerChangeEvent>().Subscribe(thermometerChangeHandler, threadType.ToThreadOption());
        //}

        //public void PumpRequestOccured(Action<PumpRequest> pumpRequestHandler, ThreadType threadType = ThreadType.PublisherThread) {
        //    _eventAggregator.GetEvent<PumpRequestEvent>().Subscribe(pumpRequestHandler, threadType.ToThreadOption());
        //}

        //public void PumpChangeOccured(Action<PumpChange> pumpChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
        //    _eventAggregator.GetEvent<PumpChangeEvent>().Subscribe(pumpChangeHandler, threadType.ToThreadOption());
        //}


        //public void PidRequestOccured(Action<PidRequest> pidRequestHandler, ThreadType threadType = ThreadType.PublisherThread) {
        //    _eventAggregator.GetEvent<PidRequestEvent>().Subscribe(pidRequestHandler, threadType.ToThreadOption());
        //}

        //public void PidChangeOccured(Action<PidChange> pidChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
            
        //    _eventAggregator.GetEvent<PidChangeEvent>().Subscribe(pidChangeHandler, threadType.ToThreadOption());
        //}


        public void SsrChangeOccured(Action<SsrChange> ssrChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
            _eventAggregator.GetEvent<SsrChangeEvent>().Subscribe(ssrChangeHandler, threadType.ToThreadOption());
        }

        public void ConnectionStatusRequestOccured(Action<ConnectionStatusRequest> connectionStatusRequestHandler, ThreadType threadType = ThreadType.PublisherThread) {
            _eventAggregator.GetEvent<ConnectionStatusRequestEvent>().Subscribe(connectionStatusRequestHandler, threadType.ToThreadOption());
        }

        public void ConnectionStatusChangeOccured(Action<ConnectionStatusChange> connectionStatusChangeHandler, ThreadType threadType = ThreadType.PublisherThread) {
            _eventAggregator.GetEvent<ConnectionStatusChangeEvent>().Subscribe(connectionStatusChangeHandler, threadType.ToThreadOption());
        }

        public virtual void InitializationChangeFired(InitializationChange initializationChange) {
            _eventAggregator.GetEvent<InitializationChangeEvent>().Publish(initializationChange);
        }

        //public virtual void TemperatureChangeFired(TemperatureChange temperatureChange) {
        //    _eventAggregator.GetEvent<TemperatureChangeEvent>().Publish(temperatureChange);
        //}

        //public virtual void ThermometerChangeFired(ThermometerChange thermometerChange) {
        //    _eventAggregator.GetEvent<ThermometerChangeEvent>().Publish(thermometerChange);
        //}

        //public virtual void PumpRequestFired(PumpRequest pumpRequest) {
        //    _eventAggregator.GetEvent<PumpRequestEvent>().Publish(pumpRequest);
        //}

        //public virtual void PumpChangeFired(PumpChange pumpChange) {
        //    _eventAggregator.GetEvent<PumpChangeEvent>().Publish(pumpChange);
        //}

        //public virtual void PidRequestFired(PidRequest pidRequest) {
        //    _eventAggregator.GetEvent<PidRequestEvent>().Publish(pidRequest);
        //}

        //public virtual void PidChangeFired(PidChange pidChange) {
        //    _eventAggregator.GetEvent<PidChangeEvent>().Publish(pidChange);
        //}

        public virtual void SsrChangeFired(SsrChange ssrChange) {
            _eventAggregator.GetEvent<SsrChangeEvent>().Publish(ssrChange);
        }

        public virtual void ConnectionStatusRequestFired(ConnectionStatusRequest connectionStatusRequest) {
            _eventAggregator.GetEvent<ConnectionStatusRequestEvent>().Publish(connectionStatusRequest);
        }

        public virtual void ConnectionStatusChangeFired(ConnectionStatusChange connectionStatusChange) {
            _eventAggregator.GetEvent<ConnectionStatusChangeEvent>().Publish(connectionStatusChange);
        }





        //*****************************************************************************

        public virtual void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState {
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
