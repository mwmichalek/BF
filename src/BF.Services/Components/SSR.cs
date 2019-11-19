using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BF.Service.Events;
using BF.Common.Events;
using Windows.Devices.Gpio;
using BF.Common.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;

namespace BF.Services.Components {

    public enum SsrPin {
        HLT = 4,
        BK = 5
    }


    public class Ssr : ComponentBase<SsrState> {

        private ILogger Logger { get; set; }

        private int _pinNumber { get; set; }

        private int _dutyCycleInMillis = 2000;

        private GpioPin _pin;

        private int _millisOn = 0;

        private int _millisOff = 2000;

        private IBeerFactoryEventHandler _eventHandler;

        private SsrState ProposedState;

        public Ssr(ComponentId id, IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<Ssr>();
            _eventHandler = eventHandler;
            CurrentState = new SsrState { Id = id };

            Enum.TryParse(id.ToString(), out SsrPin ssrPin);
            _pinNumber = (int)ssrPin;

            var gpio = GpioController.GetDefault();
            if (gpio != null) {
                _pin = gpio.OpenPin(_pinNumber);
                _pin.SetDriveMode(GpioPinDriveMode.Output);
                _pin.Write(GpioPinValue.Low);
            }

            _eventHandler.ComponentStateRequestOccured<SsrRequestState>(SsrStateRequestOccured);
        }

        private void SsrStateRequestOccured(ComponentStateRequest<SsrRequestState> ssrStateRequest) {
            if (ssrStateRequest.Id == CurrentState.Id && 
                CurrentState.IsDifferent(ssrStateRequest.RequestState)) {

                var stopIt = ssrStateRequest.RequestState.Percentage == 0 && CurrentState.IsEngaged;
                var startIt = ssrStateRequest.RequestState.Percentage > 0 && !CurrentState.IsEngaged;

                if (stopIt || startIt) {
                    PriorState = CurrentState;
                    CurrentState = CurrentState.UpdateRequest(ssrStateRequest.RequestState);
                    CalculateDurations();
                }

                if (startIt) Start();
                if (stopIt) Stop();

                if (stopIt || startIt)
                    SendNotification();
                else
                    ProposedState = CurrentState.UpdateRequest(ssrStateRequest.RequestState);
            }
        }

        private void SendNotification() {
            _eventHandler.ComponentStateChangeFiring<SsrState>(new ComponentStateChange<SsrState> {
                CurrentState = CurrentState.Clone(),
                PriorState = PriorState.Clone()
            });
        }

        private void CalculateDurations() {
            // Calculate On and Off durations
            decimal fraction = ((decimal)CurrentState.Percentage / 100.0m);
            _millisOn = (int)(fraction * (decimal)_dutyCycleInMillis);
            _millisOff = _dutyCycleInMillis - _millisOn;
            //Logger.LogInformation($"SSR: {Id} - CALC PERC {CurrentState.Percentage}, FRACTION {fraction}, MILLISON {_millisOn}, MILLISOFF {_millisOff}");
        }

        private void Start() {
            CurrentState = CurrentState.Engage(true);
            Task.Run(() => Run());
        }

        private void Stop() {
            CurrentState = CurrentState.Engage(false);
            ProposedState = null;
        }

        private void Run() {
            while (CurrentState.IsEngaged) {
                // Something is causing a random blip.
                if (CurrentState.Percentage != 0 && _millisOn > 0) {
                    On();
                    Thread.Sleep(_millisOn);
                }
                if (CurrentState.Percentage != 100 && _millisOff > 0) {
                    Off();
                    Thread.Sleep(_millisOff);
                }

                if (ProposedState != null) {
                    PriorState = CurrentState;
                    CurrentState = ProposedState;
                    ProposedState = null;
                    CalculateDurations();
                    SendNotification();
                }
            }
        }

        private void On() {
            if (!CurrentState.IsFiring) {
                //Logger.LogInformation($"SSR: {Id} - ON {_millisOn}");
                _pin?.Write(GpioPinValue.High);
                PriorState = CurrentState;
                CurrentState = CurrentState.Fire(true);
                CurrentState.IsFiring = true;
                SendNotification();
            }
        }

        private void Off() {
            if (CurrentState.IsFiring) {
                //Logger.LogInformation($"SSR: {Id} - OFF {_millisOff}");
                _pin?.Write(GpioPinValue.Low);
                PriorState = CurrentState;
                CurrentState = CurrentState.Fire(false);
                SendNotification();
            }
        }

    }

}
