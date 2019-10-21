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

namespace BF.Service.Controllers {
    public class FakeArduinoTemperatureControllerService : TemperatureControllerService {

        private IBeerFactoryEventHandler _eventHandler;

        public FakeArduinoTemperatureControllerService(IBeerFactoryEventHandler eventHandler) {
            _eventHandler = eventHandler;
            _eventHandler.SsrChangeOccured(SsrChangeOccured);
        }

        public void SsrChangeOccured(SsrChange ssr) {
            if (ssr.Id == SsrId.HLT) {
                // If above Z, add that amount to the temp
                temperatures[0] = (ssr.Percentage == 0) ? 
                    temperatures[0] - 1 : 
                    temperatures[0] + (ssr.Percentage * .01);

                _eventHandler.ThermometerChangeFired(new ThermometerChange {
                    Id = ThermometerId.HLT,
                    Value = temperatures[0],
                    Timestamp = DateTime.Now
                });
            }
        }

        private List<double> temperatures = new List<double> { 70.01d, 69.54d, 70.12d,
                                                                 70.43d, 69.72d, 68.91d,
                                                                 71.44d, 70.54d, 69.87d };

        private Random rnd = new Random();

        public override async Task Run() {

            foreach (var temperature in temperatures.Select((value, index) => new { Value = value, Index = index + 1 })) {
                //await _beerFactory.UpdateTemperatureAsync((ThermometerId)temperature.Index, temperature.Value);

                //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                //    _eventAggregator.GetEvent<TemperatureChangeEvent>().Publish(new TemperatureChange { Index = temperature.Index, Value = temperature.Value });
                //});

                //_eventManager.Publish<ThermometerChangeEvent>(new ThermometerChange { Index = temperature.Index, Value = temperature.Value });

            }

            while (true) {
                try {
                    int index = rnd.Next(0, 10);
                    if (index != 0)
                        temperatures[index] += rnd.NextDouble();

                    var thermometerId = (ThermometerId)Enum.Parse(typeof(ThermometerId), (index + 1).ToString());

                    _eventHandler.ThermometerChangeFired(new ThermometerChange {
                        Id = thermometerId,
                        Value = temperatures[index],
                        Timestamp = DateTime.Now
                    });




                    //await _beerFactory.UpdateTemperatureAsync((ThermometerId)(index + 1), temperatures[index]);

                    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    //    _eventAggregator.GetEvent<TemperatureChangeEvent>().Publish(new TemperatureChange { Index = index + 1, Value = temperatures[index] });
                    //});

                    //_eventAggregator.Publish<TemperatureChange>(new TemperatureChange { Index = index + 1, Value = temperatures[index] });
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

