using System;
using System.Linq;
using BF.Common.Events;
using BF.Common.Components;
using BF.Service;
using BF.Service.Events;
using BF.Services.Components;
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

        public MainViewModel(PidController[] pidControllers,
                             Thermometer[] thermometers,
                             Ssr[] ssrs,
                             IBeerFactoryEventHandler eventHandler,
                             ILoggerFactory loggerFactory)  {

            Logger = loggerFactory.CreateLogger<MainViewModel>();
            _eventHandler = eventHandler;

            Temperature = (double)thermometers.Single(t => t.Id == ComponentId.HLT).Temperature;
            SetPoint = (int)pidControllers.Single(t => t.Id == ComponentId.HLT).CurrentState.SetPoint;
            SsrPercentage = (int)ssrs.Single(t => t.Id == ComponentId.HLT).CurrentState.Percentage;

            _eventHandler.SubscribeToComponentStateChange<ThermometerState>(ThermometerStateChangeOccured, ComponentId.HLT, ThreadType.UIThread);
            _eventHandler.SubscribeToComponentStateChange<SsrState>(SsrStateChangeOccured, ComponentId.HLT, ThreadType.UIThread);
            _eventHandler.SubscribeToComponentStateChange<PidControllerState>(PidControllerStateChangeOccured, ComponentId.HLT, ThreadType.UIThread);
        }

        private void SsrStateChangeOccured(ComponentStateChange<SsrState> ssrStateChange) {
            Logger.LogInformation($"RaspPI: Ssr Change: {ssrStateChange.CurrentState.Percentage}");
            SsrPercentage = ssrStateChange.CurrentState.Percentage;
     
        }

        private void ThermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) {
            Logger.LogInformation($"RaspPI: Thermometer Change: {thermometerStateChange.CurrentState.Temperature}");
            Temperature = thermometerStateChange.CurrentState.Temperature;
        }

        private void PidControllerStateChangeOccured(ComponentStateChange<PidControllerState> pidControllerStateChange) {
            Logger.LogInformation($"RaspPI: Pid Change: {pidControllerStateChange.CurrentState.SetPoint}");
            SetPoint = (int)pidControllerStateChange.CurrentState.SetPoint;
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
