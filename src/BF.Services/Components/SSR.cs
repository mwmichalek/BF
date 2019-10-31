using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BF.Service.Events;
using BF.Common.Ids;
using BF.Common.Events;
using Windows.Devices.Gpio;
using BF.Common.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;

namespace BF.Service.Components {

    public enum SsrPin {
        HLT = 4,
        BK = 5
    }


    public class Ssr : IComponent {

        public ComponentId Id { get; set; }

        public SsrState CurrentState { get; set; }

        public SsrState PriorState { get; set; }

        private ILogger Logger { get; set; }

        private int _pinNumber { get; set; }

        private int _dutyCycleInMillis = 2000;

        //public int DutyCycleInMillis {
        //    get {
        //        return _dutyCycleInMillis;
        //    }
        //    set {
        //        _dutyCycleInMillis = value;
        //        CalculateDurations();
        //    }
        //}

        //private int _percentage = 0;
        //public int Percentage {
        //    get {
        //        return _percentage;
        //    }
        //    set {
        //        if (value != _percentage) {
        //            Logger.LogInformation($"Setting Percentage to {value}");
        //            _percentage = value;
        //            CalculateDurations();
        //            SendNotification();
        //        }
        //    }
        //}

        private GpioPin _pin;
        private GpioPinValue _pinValue = GpioPinValue.High;

        private bool _isRunning = false;

        private int _millisOn = 0;
        private int _millisOff = 2000;

        private IBeerFactoryEventHandler _eventHandler;

        public Ssr(ComponentId id, IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<Ssr>();
            _eventHandler = eventHandler;
            Id = id;
            //Pin = (int)id;

            Enum.TryParse(id.ToString(), out SsrPin ssrPin);
            _pinNumber = (int)ssrPin;

            var gpio = GpioController.GetDefault();
            if (gpio != null) {
                _pin = gpio.OpenPin(_pinNumber);
                _pin.SetDriveMode(GpioPinDriveMode.Output);
                _pin.Write(GpioPinValue.Low);
            }

            _eventHandler.ComponentStateRequestOccured<SsrState>(SsrStateRequestOccured);
        }

        private void SsrStateRequestOccured(ComponentStateRequest<SsrState> ssrStateRequest) {
            if (ssrStateRequest.Id == Id) {
                PriorState = CurrentState;
                CurrentState = CurrentState.Update(ssrStateRequest.RequestState);



                //if (ssrStateRequest.RequestState.Percentage != _)

                //_percentage = value;
                //            CalculateDurations();
                //            SendNotification();
            }
        }
    

        public void Start() {
            _isRunning = true;
            CalculateDurations();

            // Call new thread to run
            Task.Run(() => Run());
        }

        private void CalculateDurations() {
            // Calculate On and Off durations
            decimal fraction = ((decimal)CurrentState.Percentage / 100.0m);
            _millisOn = (int)(fraction * (decimal)_dutyCycleInMillis);
            _millisOff = _dutyCycleInMillis - _millisOn;
            Logger.LogInformation($"SSR: {Id} - CALC PERC {CurrentState.Percentage}, FRACTION {fraction}, MILLISON {_millisOn}, MILLISOFF {_millisOff}");
        }

        private void Run() {
            while (_isRunning) {
                // Something is causing a random blip.
                if (CurrentState.Percentage != 0 && _millisOn > 0) {
                    On();
                    Thread.Sleep(_millisOn);
                }
                if (CurrentState.Percentage != 100 && _millisOff > 0) {
                    Off();
                    Thread.Sleep(_millisOff);
                }
            }
        }

        private void On() {
            if (!CurrentState.IsFiring) {
                Logger.LogInformation($"SSR: {Id} - ON {_millisOn}");
                _pin?.Write(GpioPinValue.High);
                PriorState = CurrentState;
                CurrentState = CurrentState.Update(true);
                CurrentState.IsFiring = true;
                SendNotification();
            }
        }

        private void Off() {
            if (CurrentState.IsFiring) {
                Logger.LogInformation($"SSR: {Id} - OFF {_millisOff}");
                _pin?.Write(GpioPinValue.Low);
                PriorState = CurrentState;
                CurrentState = CurrentState.Update(false);
                SendNotification();
            }
        }

        private void SendNotification() {
            _eventHandler.ComponentStateChangeFiring<SsrState>(new ComponentStateChange<SsrState> {
                Id = Id,
                CurrentState = CurrentState,
                PriorState = PriorState
            });
        }

        public void Stop() {
            _isRunning = false;
        }
    }

}
