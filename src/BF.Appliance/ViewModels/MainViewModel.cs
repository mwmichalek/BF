using System;
using BF.Common.Events;
using BF.Common.Components;
using BF.Service;
using BF.Service.Events;
using BF.Service.Components;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Microsoft.Extensions.Logging;
using BF.Common.States;
using System.Collections.Generic;

namespace BF.Appliance.ViewModels {

    public class MainViewModel : ViewModelBase {

        private ILogger Logger { get; set; }

        private IBeerFactoryEventHandler _eventHandler;

        public MainViewModel(IBeerFactoryEventHandler eventHandler, Thermometer[] thermometers, ILoggerFactory loggerFactory)  {
            Logger = loggerFactory.CreateLogger<MainViewModel>();
            _eventHandler = eventHandler;

            Temperature = (double)thermometers.GetById(ComponentId.HLT)?.Temperature;
            
            //var hltPidController = beerFactory.PidControllers.GetById<PidController>(ComponentId.HLT);
            //if (hltPidController != null)
            //    SetPoint = (int)hltPidController.CurrentState.SetPoint;

            _eventHandler.ComponentStateChangeOccured<ThermometerState>(ThermometerStateChangeOccured, ThreadType.UIThread);
            _eventHandler.SsrChangeOccured(SsrChangeOccured, ThreadType.UIThread);
            _eventHandler.ConnectionStatusChangeOccured(ConnectionStatusChangeOccured, ThreadType.UIThread);

            //connection = new HubConnectionBuilder()
            //    .WithUrl("https://emrsd-ws-bf.azurewebsites.net/ChatHub")
            //    .Build();

            //connection.On<string, string>("ReceiveMessage", (user, message) => {
            //    Logger.Information($"User: {user}, Message: {message}");
            //});

            //try {
            //    connection.StartAsync();

            //} catch (Exception ex) {

            //}
        }

        private void ThermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) {
            if (thermometerStateChange.Id == ComponentId.HLT) {
                Logger.LogInformation($"HLT Change: {thermometerStateChange.CurrentState.Temperature}");
                Temperature = thermometerStateChange.CurrentState.Temperature;
            }
        }

        public void SsrChangeOccured(SsrChange ssrChange) {
            if (ssrChange.Id == ComponentId.HLT) {
                SsrPercentage = ssrChange.Percentage;
            }
        }

        public void ConnectionStatusChangeOccured(ConnectionStatusChange connectionStatus) {
            //Title = $"ConnectionStatus: {connectionStatus.Status.ToString()}";
        }

        private bool _engaged;

        public bool Engaged {
            get { return _engaged; }
            set { SetProperty(ref _engaged, value); }
        }

        private string _stage = "STRIKE";

        public string Stage {
            get { return _stage; }
            set { SetProperty(ref _stage, value); }
        }

        private double _temperature;

        public double Temperature {
            get { return _temperature; }
            set {
                SetProperty(ref _temperature, value);
                TemperatureStr = _temperature.ToString("0.0");
            }
        }

        private string _temperatureStr;

        public string  TemperatureStr {
            get { return _temperatureStr; }
            set { SetProperty(ref _temperatureStr, value); }
        }

        private int _setPoint;

        public int SetPoint {
            get { return _setPoint; }
            set { SetProperty(ref _setPoint, value); }
        }

        private int _ssrPercentage;

        public int SsrPercentage {
            get { return _ssrPercentage; }
            set { SetProperty(ref _ssrPercentage, value); }
        }
    }
}
