using BF.Common.Events;
using BF.Service.Events;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Prism.Events {

    public class PrismBeerFactoryEventHandler : IBeerFactoryEventHandler {

        private ILogger Logger { get; set; }

        private IEventAggregator _eventAggregator;

        public PrismBeerFactoryEventHandler(IEventAggregator eventAggregator) {
            _eventAggregator = eventAggregator;
            Logger = Log.Logger;
        }

        public void TemperatureChangeOccured(Action<TemperatureChange> temperatureChangeHandler) {
            _eventAggregator.GetEvent<TemperatureChangeEvent>().Subscribe(temperatureChangeHandler);
        }

        public void ThermometerChangeOccured(Action<ThermometerChange> thermometerChangeHandler) {
            _eventAggregator.GetEvent<ThermometerChangeEvent>().Subscribe(thermometerChangeHandler);
        }


        public void PumpRequestOccured(Action<PumpRequest> pumpRequestHandler) {
            _eventAggregator.GetEvent<PumpRequestEvent>().Subscribe(pumpRequestHandler);
        }

        public void PumpChangeOccured(Action<PumpChange> pumpChangeHandler) {
            _eventAggregator.GetEvent<PumpChangeEvent>().Subscribe(pumpChangeHandler);
        }


        public void PidRequestOccured(Action<PidRequest> pidRequestHandler) {
            _eventAggregator.GetEvent<PidRequestEvent>().Subscribe(pidRequestHandler);
        }

        public void PidChangeOccured(Action<PidChange> pidChangeHandler) {
            _eventAggregator.GetEvent<PidChangeEvent>().Subscribe(pidChangeHandler);
        }


        public void SsrChangeOccured(Action<SsrChange> ssrChangeHandler) {
            _eventAggregator.GetEvent<SsrChangeEvent>().Subscribe(ssrChangeHandler);
        }


        public void ConnectionStatusChangeOccured(Action<ConnectionStatus> connectionStatusChangeHandler) {
            _eventAggregator.GetEvent<ConnectionStatusEvent>().Subscribe(connectionStatusChangeHandler);
        }


        public void TemperatureChangeFired(TemperatureChange temperatureChange) {
            _eventAggregator.GetEvent<TemperatureChangeEvent>().Publish(temperatureChange);
        }

        public void ThermometerChangeFired(ThermometerChange thermometerChange) {
            Log.Information($"Handler Temp Update: {thermometerChange.Id} - {thermometerChange.Value}");
            _eventAggregator.GetEvent<ThermometerChangeEvent>().Publish(thermometerChange);
        }

        public void PumpRequestFired(PumpRequest pumpRequest) {
            _eventAggregator.GetEvent<PumpRequestEvent>().Publish(pumpRequest);
        }

        public void PumpChangeFired(PumpChange pumpChange) {
            _eventAggregator.GetEvent<PumpChangeEvent>().Publish(pumpChange);
        }

        public void PidRequestFired(PidRequest pidRequest) {
            _eventAggregator.GetEvent<PidRequestEvent>().Publish(pidRequest);
        }

        public void PidChangeFired(PidChange pidChange) {
            _eventAggregator.GetEvent<PidChangeEvent>().Publish(pidChange);
        }

        public void SsrChangeFired(SsrChange ssrChange) {
            _eventAggregator.GetEvent<SsrChangeEvent>().Publish(ssrChange);
        }

        public void ConnectionStatusChangeFired(ConnectionStatus connectionStatusChange) {
            _eventAggregator.GetEvent<ConnectionStatusEvent>().Publish(connectionStatusChange);
        }
        
    }

}
