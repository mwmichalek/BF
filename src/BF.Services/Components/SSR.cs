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

namespace BF.Service.Components {
    

    public class Ssr : IComponent {

        private ILogger Logger { get; set; }

        public ComponentId Id { get; set; }

        public int Pin { get; set; }


        private bool _isEngaged;

        public bool IsEngaged {
            get { return _isEngaged; }
            set {
                _isEngaged = value;
            }
        }

        private int _dutyCycleInMillis = 2000;
        public int DutyCycleInMillis {
            get {
                return _dutyCycleInMillis;
            }
            set {
                _dutyCycleInMillis = value;
                CalculateDurations();
            }
        }

        private int _percentage = 0;
        public int Percentage {
            get {
                return _percentage;
            }
            set {
                if (value != _percentage) {
                    _percentage = value;
                    CalculateDurations();
                    SendNotification();
                }
            }
        }

        private GpioPin pin;
        private GpioPinValue pinValue = GpioPinValue.High;

        private bool isRunning = false;

        private int millisOn = 0;
        private int millisOff = 2000;

        private IBeerFactoryEventHandler _eventHandler;

        public Ssr(IBeerFactoryEventHandler eventHandler, ComponentId id, ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<Ssr>();
            _eventHandler = eventHandler;
            Id = id;
            Pin = (int)id;

            var gpio = GpioController.GetDefault();
            if (gpio != null) {
                pin = gpio.OpenPin(Pin);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                pin.Write(GpioPinValue.Low);
            }
        }

        public void Start() {
            isRunning = true;
            CalculateDurations();

            // Call new thread to run
            Task.Run(() => Run());
        }

        private void CalculateDurations() {
            // Calculate On and Off durations
            decimal fraction = ((decimal)_percentage / 100.0m);
            millisOn = (int)(fraction * (decimal)_dutyCycleInMillis);
            millisOff = _dutyCycleInMillis - millisOn;
            Logger.LogInformation($"SSR: {Id} - CALC PERC {Percentage}, FRACTION {fraction}, MILLISON {millisOn}, MILLISOFF {millisOff}");
        }

        private void Run() {
            while (isRunning) {
                // Something is causing a random blip.
                if (Percentage != 0 && millisOn > 0) {
                    On();
                    Thread.Sleep(millisOn);
                }
                if (Percentage != 100 && millisOff > 0) {
                    Off();
                    Thread.Sleep(millisOff);
                }
            }
        }

        private void On() {
            if (!IsEngaged) {
                Logger.LogInformation($"SSR: {Id} - ON {millisOn}");
                pin?.Write(GpioPinValue.High);
                IsEngaged = true;
                SendNotification();
            }
        }

        private void Off() {

            if (IsEngaged) {
                Logger.LogInformation($"SSR: {Id} - OFF {millisOff}");
                pin?.Write(GpioPinValue.Low);
                IsEngaged = false;
                SendNotification();
            }
        }

        private void SendNotification() {
            _eventHandler.SsrChangeFired(new SsrChange {
                Id = Id,
                Percentage = Percentage,
                IsEngaged = IsEngaged
            });
        }

        public void Stop() {
            isRunning = false;
        }
    }

}
