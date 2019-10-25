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
using BF.Common.Ids;
using BF.Service.Events;
using BF.Common.Events;
using Serilog;
using BF.Common.Components;

namespace BF.Service.Controllers {
    public class FakeArduinoTemperatureControllerService : TemperatureControllerService {

        private ILogger Logger { get; set; }

        private IBeerFactoryEventHandler _eventHandler;

        public FakeArduinoTemperatureControllerService(IBeerFactoryEventHandler eventHandler) {
            Logger = Log.Logger;
            _eventHandler = eventHandler;
            _eventHandler.SsrChangeOccured(SsrChangeOccured);
        }

        private int hltSsrPercentage = 0;

        public void SsrChangeOccured(SsrChange ssr) {
            if (ssr.Id == ComponentId.HLT) {
                hltSsrPercentage = ssr.Percentage;
            }
        }

        private List<double> temperatures = new List<double> { 70.01d, 69.54d, 70.12d,
                                                                 70.43d, 69.72d, 68.91d,
                                                                 71.44d, 70.54d, 69.87d };

        private Random rnd = new Random();

        public override async Task Run() {

            while (true) {
                try {
                    //int index = rnd.Next(0, 10);
                    //if (index != 0)
                    //    temperatures[index] += rnd.NextDouble();

                    //var thermometerId = (ThermometerId)Enum.Parse(typeof(ThermometerId), (index + 1).ToString());

                    //_eventHandler.ThermometerChangeFired(new ThermometerChange {
                    //    Id = thermometerId,
                    //    Value = temperatures[index],
                    //    Timestamp = DateTime.Now
                    //});

                    var newTemperature = (hltSsrPercentage == 0) ?
                        temperatures[0] - 0.2 :
                        temperatures[0] + (hltSsrPercentage * .01);



                    if (newTemperature < 70)
                        newTemperature = 70;

                    if (newTemperature != temperatures[0]) {
                        Logger.Information($"Fake: OLD: {temperatures[0]} - NEW: {newTemperature}");
                        _eventHandler.ThermometerChangeFired(new ThermometerChange {
                            Id = ComponentId.HLT,
                            Value = newTemperature,
                            Timestamp = DateTime.Now
                        });

                        temperatures[0] = newTemperature;
                    }

                    
                } catch (Exception) {

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

