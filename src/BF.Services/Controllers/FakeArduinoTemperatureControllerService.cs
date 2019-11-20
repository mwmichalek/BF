using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;
using Windows.Storage.Streams;
using Windows.UI.Core;
using SerialPortLib;
using SerialArduino;
using System.Linq;
using BF.Service.Events;
using BF.Common.Events;
using BF.Common.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;
using BF.Services.Components;

namespace BF.Service.Controllers {

    public class FakeArduinoTemperatureControllerService : TemperatureControllerService {

        private ILogger Logger { get; set; }

        private IBeerFactoryEventHandler _eventHandler;

        private Random _rnd = new Random();

        private Dictionary<ComponentId, SsrState> _ssrStateLookup = new Dictionary<ComponentId, SsrState>();

        private Dictionary<ComponentId, ThermometerState> _thermometerStateLookup = new Dictionary<ComponentId, ThermometerState>();

        public FakeArduinoTemperatureControllerService(Thermometer[] thermometers, 
                                                       Ssr[] ssrs, 
                                                       IBeerFactoryEventHandler eventHandler, 
                                                       ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<FakeArduinoTemperatureControllerService>();
            _eventHandler = eventHandler;

            _ssrStateLookup = ssrs.ToDictionary(ssr => ssr.CurrentState.Id, ssr => ssr.CurrentState);
            _thermometerStateLookup = thermometers.ToDictionary(therm => therm.CurrentState.Id, therm => therm.CurrentState);

            _eventHandler.SubscribeToComponentStateChange<SsrState>(ssrStateChangeOccured);
            _eventHandler.SubscribeToComponentStateChange<ThermometerState>(thermometerStateChangeOccured);

            foreach (var componentId in ComponentHelper.AllComponentIds) {
                _eventHandler.ComponentStateChangeFiring(new ComponentStateChange<ThermocoupleState> {
                    CurrentState = new ThermocoupleState {
                        Id = componentId,
                        Temperature = 70
                    }
                });
            }
        }

        private void ssrStateChangeOccured(ComponentStateChange<SsrState> ssrStateChange) {
            _ssrStateLookup[ssrStateChange.Id] = ssrStateChange.CurrentState;
        }

        private void thermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) {
            _thermometerStateLookup[thermometerStateChange.Id] = thermometerStateChange.CurrentState;
        }

        public override async Task Run() {

            while (true) {
                try {

                    foreach (var ssrComponentId in _ssrStateLookup.Keys.ToList()) {

                        if (_ssrStateLookup.ContainsKey(ssrComponentId) && _thermometerStateLookup.ContainsKey(ssrComponentId)) {
                            var ssrState = _ssrStateLookup[ssrComponentId];
                            var thermometerState = _thermometerStateLookup[ssrComponentId];

                            var newTemperature = (ssrState.Percentage == 0) ?
                                thermometerState.Temperature - 0.05 :
                                thermometerState.Temperature + (ssrState.Percentage * .01);

                            if (newTemperature < 70)
                                newTemperature = 70;

                            if (newTemperature != thermometerState.Temperature) {
                                //Logger.LogInformation($"Fake: OLD: {thermometerState.Temperature} - NEW: {newTemperature}");

                                _eventHandler.ComponentStateChangeFiring(new ComponentStateChange<ThermocoupleState> {
                                    
                                    CurrentState = new ThermocoupleState {
                                        Id = ssrComponentId,
                                        Temperature = newTemperature
                                    }
                                });
                            }

                        }
                    }        
                    
                } catch (Exception ex) {
                    Logger.LogError(ex.ToString());
                }

                await Task.Delay(1000);
            }

        }
    }

    public static class Bullshit {

        public static double NextDouble(this Random rnd) {
            double tempChange = (rnd.Next(0, 20) / 100) - 0.10d;
            return tempChange;
        }
    }

}

