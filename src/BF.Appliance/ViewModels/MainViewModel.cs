using System;
using BF.Common.Events;
using BF.Common.Ids;
using BF.Service;
using BF.Service.Events;
using BF.Service.Components;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Serilog;


namespace BF.Appliance.ViewModels {

    public class MainViewModel : ViewModelBase {

        private ILogger Logger { get; set; }

        //private HubConnection connection;

        private IBeerFactoryEventHandler _eventHandler;

        public MainViewModel(IBeerFactory beerFactory, IBeerFactoryEventHandler eventHandler)  {
            Logger = Log.Logger;
            _eventHandler = eventHandler;
            
            var hltThermometer = beerFactory.Thermometers.GetById(ThermometerId.HLT);
            if (hltThermometer != null)
                Temperature = (double)hltThermometer.Temperature;


            var hltPidController = beerFactory.PidControllers.GetById(PidControllerId.HLT);
            if (hltPidController != null)
                SetPoint = (int)hltPidController.SetPoint;

            _eventHandler.TemperatureChangeOccured(TemperatureChangeOccured);
            _eventHandler.SsrChangeOccured(SsrChangeOccured);
            _eventHandler.ConnectionStatusChangeOccured(ConnectionStatusChangeOccured);

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

        public void TemperatureChangeOccured(TemperatureChange temperatureChange) {

            if (temperatureChange.Id == ThermometerId.HLT) {
                Logger.Information($"HLT Change: {temperatureChange.Value}");
                Temperature = (double)temperatureChange.Value;
                //Temperature = Math.Round((double)temperatureChange.Value, 1);
                //connection.InvokeAsync("SendMessage",
                //    "Temp Change", temperatureChange.Value.ToString());
            }
        }

        public void SsrChangeOccured(SsrChange ssrChange) {
            if (ssrChange.Id == SsrId.HLT) {
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
