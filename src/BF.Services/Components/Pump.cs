using BF.Common.Components;
using BF.Common.Events;
using BF.Common.States;
using BF.Service.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Gpio;

namespace BF.Services.Components {

    public enum PumpPin {
        HLT = 16,
        MT = 20,
        BK = 21
    }

    public class Pump : ConfirgurableComponentBase<PumpState> {

        private ILogger Logger { get; set; }

        private IBeerFactoryEventHandler _eventHandler;

        private int _pinNumber { get; set; }

        private GpioPin _pin;

        public Pump(ComponentId id,
                    IBeerFactoryEventHandler eventHandler,
                    ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<Pump>();
            CurrentState = new PumpState { Id = id };

            Enum.TryParse(id.ToString(), out PumpPin pumpPin);
            _pinNumber = (int)pumpPin;

            var gpio = GpioController.GetDefault();
            if (gpio != null) {
                _pin = gpio.OpenPin(_pinNumber);
                _pin.SetDriveMode(GpioPinDriveMode.Output);
                _pin.Write(GpioPinValue.Low);
            }

            _eventHandler.SubscribeToComponentStateRequest<PumpRequestState>(PumpStateRequestOccured);
        }

        private void PumpStateRequestOccured(ComponentStateRequest<PumpRequestState> pumpRequestState) {
            bool correctPump = pumpRequestState.Id == CurrentState.Id;
            if (pumpRequestState.Id == CurrentState.Id && CurrentState.IsDifferent(pumpRequestState.RequestState)) {
                CurrentState = CurrentState.Update(pumpRequestState.RequestState);

                if (CurrentState.IsEngaged)
                    _pin?.Write(GpioPinValue.High);
                else
                    _pin?.Write(GpioPinValue.Low);

                Logger.LogInformation($"Pump: {CurrentState.Id} - {CurrentState.IsEngaged}");

                _eventHandler.ComponentStateChangeFiring(new ComponentStateChange<PumpState> {
                    CurrentState = CurrentState.Clone()
                });
            }
        }



    }
}
