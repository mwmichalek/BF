//using Microsoft.IoT.DeviceCore.Pwm;
//using Microsoft.IoT.Devices.Pwm;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Windows.Devices.Gpio;
//using Windows.Devices.Pwm;
//using Windows.UI.Core;
using BF.Service.Events;
using BF.Common.Ids;
using BF.Common.Events;
using Windows.Devices.Gpio;

namespace BF.Service.Components {
    

    public class Ssr {

        public SsrId Id { get; set; }

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
                _percentage = value;
                CalculateDurations();
                SendNotification();
            }
        }

        private GpioPin pin;
        private GpioPinValue pinValue = GpioPinValue.High;

        private bool isRunning = false;

        private int millisOn = 0;
        private int millisOff = 2000;

        private IBeerFactoryEventHandler _eventHandler;

        public Ssr(IBeerFactoryEventHandler eventHandler, SsrId id) {
            _eventHandler = eventHandler;
            Id = id;
            Pin = (int)id;

            var gpio = GpioController.GetDefault();
            if (gpio != null) {
                pin = gpio.OpenPin(Pin);
                pin.SetDriveMode(GpioPinDriveMode.Output);
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
        }

        private void Run() {
            while (isRunning) {
                // Something is causing a random blip.
                if (Percentage != 0 && millisOn > 0) {
                    On();
                    Thread.Sleep(millisOn);
                }
                Off();
                Thread.Sleep(millisOff);
            }
        }

        private void On() {
            if (!IsEngaged) {
                pin?.Write(GpioPinValue.High);
                IsEngaged = true;
                SendNotification();
            }
        }

        private void Off() {
            if (IsEngaged) {
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

    public static class SsrHelper {

        public static Ssr GetById(this List<Ssr> ssrs, SsrId ssrId) {
            return ssrs.SingleOrDefault(s => s.Id == ssrId);
        }

    }

}
