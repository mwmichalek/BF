﻿using System;
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
using BF.Common.Ids;
using BF.Service.Events;
using BF.Common.Events;
using BF.Common.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;
using BF.Service.Components;

namespace BF.Service.Controllers {

    public class FakeArduinoTemperatureControllerService : TemperatureControllerService {

        private ILogger Logger { get; set; }

        private IBeerFactoryEventHandler _eventHandler;

        private Random rnd = new Random();

        private Dictionary<ComponentId, SsrState> ssrStateLookup = new Dictionary<ComponentId, SsrState>();

        private Dictionary<ComponentId, ThermometerState> thermometerStateLookup = new Dictionary<ComponentId, ThermometerState>();

        public FakeArduinoTemperatureControllerService(Thermometer[] thermometers, 
                                                       Ssr[] ssrs, 
                                                       IBeerFactoryEventHandler eventHandler, 
                                                       ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<FakeArduinoTemperatureControllerService>();
            _eventHandler = eventHandler;

            foreach (var ssr in ssrs)
                ssrStateLookup[ssr.Id] = ssr.CurrentState;

            foreach (var thermometer in thermometers)
                thermometerStateLookup[thermometer.Id] = thermometer.CurrentState;

            _eventHandler.ComponentStateChangeOccured<SsrState>(ssrStateChangeOccured);
            _eventHandler.ComponentStateChangeOccured<ThermometerState>(thermometerStateChangeOccured);

            StartFakeness();
        }

        public void StartFakeness() {
            // Initialize all Thermometers with 70
            foreach (var componentId in ComponentHelper.AllComponentIds) {
                _eventHandler.ComponentStateChangeFiring(new ComponentStateChange<ThermocoupleState> {
                    Id = componentId,
                    CurrentState = new ThermocoupleState {
                        Temperature = 70.0
                    }
                });
            }

            // Set HLT Pid to 90 degrees
            _eventHandler.ComponentStateRequestFiring(new ComponentStateRequest<PidControllerState> {
                Id = ComponentId.HLT,
                RequestState = new PidControllerState {
                    IsEngaged = true,
                    SetPoint = 90,
                    GainProportional = 18,
                    GainIntegral = 1.5,
                    GainDerivative = 22.5
                }
            });
        }

        private void ssrStateChangeOccured(ComponentStateChange<SsrState> ssrStateChange) {
            ssrStateLookup[ssrStateChange.Id] = ssrStateChange.CurrentState;
        }

        private void thermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) {
            thermometerStateLookup[thermometerStateChange.Id] = thermometerStateChange.CurrentState;
        }

        public override async Task Run() {

            while (true) {
                try {

                    foreach (var ssrComponentId in ssrStateLookup.Keys.ToList()) {

                        if (ssrStateLookup.ContainsKey(ssrComponentId) && thermometerStateLookup.ContainsKey(ssrComponentId)) {
                            var ssrState = ssrStateLookup[ssrComponentId];
                            var thermometerState = thermometerStateLookup[ssrComponentId];

                            var newTemperature = (ssrState.Percentage == 0) ?
                                thermometerState.Temperature - 0.2 :
                                thermometerState.Temperature + (ssrState.Percentage * .01);

                            if (newTemperature < 70)
                                newTemperature = 70;

                            if (newTemperature != thermometerState.Temperature) {
                                Logger.LogInformation($"Fake: OLD: {thermometerState.Temperature} - NEW: {newTemperature}");

                                _eventHandler.ComponentStateChangeFiring(new ComponentStateChange<ThermocoupleState> {
                                    Id = ssrComponentId,
                                    CurrentState = new ThermocoupleState {
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

