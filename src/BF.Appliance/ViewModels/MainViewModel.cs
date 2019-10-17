using System;
using BF.Common.Events;
using BF.Common.Ids;
using BF.Service;
using BF.Service.Events;
using BF.Service.Components;
using BF.Service.Pid;
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
            //MyAwesomeCommand = new DelegateCommand<string>(ExecuteMyAwesomeCommand, (str) => Test == "Balls").ObservesProperty(() => Test);

            UpdatePidEnabledCommand = new DelegateCommand(UpdatePid);
            //UpdatePidSetPointCommand = new DelegateCommand(UpdatePid);

            var hltThermometer = beerFactory.Thermometers.GetById(ThermometerId.HLT);
            if (hltThermometer != null)
                HltTemperature = (double)hltThermometer.Temperature;


            var hltPidController = beerFactory.PidControllers.GetById(PidControllerId.HLT);
            if (hltPidController != null)
                HltSetpoint = (int)hltPidController.SetPoint;




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


        //public DelegateCommand<string> MyAwesomeCommand { get; private set; }

        public void TemperatureChangeOccured(TemperatureChange temperatureChange) {

            if (temperatureChange.Id == ThermometerId.HLT) {
                Logger.Information($"HLT Change: {temperatureChange.Value}");
                HltTemperature = Math.Round((double)temperatureChange.Value, 1);
                //connection.InvokeAsync("SendMessage",
                //    "Temp Change", temperatureChange.Value.ToString());
            }
        }

        public void SsrChangeOccured(SsrChange ssrChange) {
            if (ssrChange.Id == SsrId.HLT) {
                HltSsrPercentage = ssrChange.Percentage;
            }
        }

        public void ConnectionStatusChangeOccured(ConnectionStatus connectionStatus) {
            //Title = $"ConnectionStatus: {connectionStatus.Status.ToString()}";
        }


        public DelegateCommand UpdatePidEnabledCommand { get; private set; }


        public void UpdatePid() {
            _eventHandler.PidRequestFired(new PidRequest {
                Id = PidControllerId.HLT,
                IsEngaged = Engaged,
                SetPoint = _hltSetpoint,
                PidMode = PidMode.Temperature
            });
        }

        private bool _engaged;

        public bool Engaged {
            get { return _engaged; }
            set {
                SetProperty(ref _engaged, value);
                //TODO: Switch to delegate
                //UpdatePid();
            }
        }

        private double _hltTemperature;

        public double HltTemperature {
            get { return _hltTemperature; }
            set {
                SetProperty(ref _hltTemperature, value);
                HltTemperatureStr = _hltTemperature.ToString("0.0");
            }
        }

        private string _hltTemperatureStr;

        public string HltTemperatureStr {
            get { return _hltTemperatureStr; }
            set {
                SetProperty(ref _hltTemperatureStr, value);
            }
        }

        public DelegateCommand UpdatePidSetPointCommand { get; private set; }

        private int _hltSetpoint;

        public int HltSetpoint {
            get { return _hltSetpoint; }
            set {
                SetProperty(ref _hltSetpoint, value);
                //TODO: Switch to delegate
                //UpdatePid();
            }
        }

        private int _hltSsrPercentage;

        public int HltSsrPercentage {
            get { return _hltSsrPercentage; }
            set {
                SetProperty(ref _hltSsrPercentage, value);
                //TODO: Switch to delegate
                //UpdatePid();
            }
        }
    }
}
