using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Windows.UI.Core;
using Windows.Devices.Pwm;
using Windows.Devices.Gpio;
//using Microsoft.IoT.DeviceCore.Pwm;
//using Microsoft.IoT.Devices.Pwm;

using BF.Service.Components;
using BF.Service.Events;
using BF.Common.Ids;
using BF.Common.Events;
using BF.Common.Components;
using BF.Services.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;

namespace BF.Service {

    public interface IBeerFactory {
        
        List<Thermometer> Thermometers { get; }

        List<PidController> PidControllers { get; }

        List<Pump> Pumps { get; }

        List<Ssr> Ssrs { get; }
    }

    public class BeerFactory : IBeerFactory {

        private ILogger Logger { get; set; }

        private List<Phase> _phases = new List<Phase>();

        public List<PidController> PidControllers { get; set; } = new List<PidController>();

        public List<Ssr> Ssrs { get; set; } = new List<Ssr>();

        public List<Pump> Pumps { get; set; } = new List<Pump>();

        public List<Thermometer> Thermometers { get; set; } = new List<Thermometer>();

        private IBeerFactoryEventHandler _eventHandler;

        public BeerFactory(IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<BeerFactory>();

            for (int index = 1; index <= (int)ThermometerId.FERM; index++ )
                Thermometers.Add(new Thermometer(_eventHandler, (ComponentId)index, loggerFactory));

            _phases.Add(new Phase(PhaseId.FillStrikeWater, 20));
            _phases.Add(new Phase(PhaseId.HeatStrikeWater, 40));
            _phases.Add(new Phase(PhaseId.Mash, 90));
            _phases.Add(new Phase(PhaseId.MashOut, 90));
            _phases.Add(new Phase(PhaseId.Sparge, 60));
            _phases.Add(new Phase(PhaseId.Boil, 90));
            _phases.Add(new Phase(PhaseId.Chill, 30));

            var hltSsr = new Ssr(_eventHandler, ComponentId.HLT, loggerFactory);
            hltSsr.Percentage = 0;
            hltSsr.Start();

            Ssrs.Add(hltSsr);

            var bkSsr = new Ssr(_eventHandler, ComponentId.BK, loggerFactory);
            bkSsr.Percentage = 0;
            bkSsr.Start();

            Ssrs.Add(bkSsr);

            var _hltPidController = new PidController(_eventHandler, 
                                                      ComponentId.HLT,
                                                      hltSsr, 
                                                      Thermometers.GetById<Thermometer>(ComponentId.HLT).Temperature,
                                                      loggerFactory);
            //_hltPidController.GainProportional = 18;
            //_hltPidController.GainIntegral = 1.5;
            //_hltPidController.GainDerivative = 22.5;

            //_hltPidController.GainProportional = 0.5;
            //_hltPidController.GainIntegral = 0.5;
            //_hltPidController.GainDerivative = 0.5;

            //_hltPidController.SetPoint = 120;

            PidControllers.Add(_hltPidController);
            _hltPidController.Process();


            _eventHandler.ComponentStateRequestFiring<PidControllerState>(new ComponentStateRequest<PidControllerState> {
                Id = ComponentId.HLT,
                RequestState = new PidControllerState {

                    IsEngaged = true,
                    PidMode = PidMode.Temperature,
                    SetPoint = 90,
                    GainDerivative = 22.5,
                    GainIntegral = 1.5,
                    GainProportional = 18
                }
            });

            foreach (var id in new []{ ComponentId.HLT, ComponentId.MT, ComponentId.BK }) {
                Pumps.Add(new Pump {
                    Id = id
                });
            }

            //Used upon reconnection
            _eventHandler.InitializationChangeOccured(BroadcastBeerFactoryState);

            //Trigger now becuase the first one was missed.

            Task.Run(() => {
                Thread.Sleep(10000);
                _eventHandler.InitializationChangeFired(new InitializationChange {
                    Device = Device.RaspberryPi
                });
            });

            
        }

        private void SomeBitch(ComponentStateChange<ThermometerState> thermometerStateChange) {
            Logger.LogInformation($"Holy fucker! {thermometerStateChange.CurrentState.Temperature}");
        }

        public void BroadcastBeerFactoryState(InitializationChange initializationChange) {
            if (initializationChange.Device == Device.RaspberryPi) {
                Logger.LogInformation("Senting Initial Data");
                _eventHandler.ConnectionStatusChangeFired(new ConnectionStatusChange {
                    ClientId = "RaspberryPi",
                    ConnectionState = ConnectionState.Connected,
                    //ThermometerChanges = Thermometers.SelectMany(t => t.ThermometerChanges).ToList(),
                    SsrChanges = Ssrs.Select(s => new SsrChange {
                        Id = s.Id,
                        IsEngaged = s.IsEngaged,
                        Percentage = s.Percentage
                    }).ToList(),
                    PumpChanges = Pumps.Select(p => new PumpChange {
                        Id = p.Id,
                        IsEngaged = p.IsEngaged
                    }).ToList(),
                    //PidChanges = PidControllers.Select(pid => new PidChange {
                    //    Id = pid.Id,
                    //    IsEngaged = pid.IsEngaged,
                    //    PidMode = pid.PidMode,
                    //    SetPoint = pid.SetPoint,
                    //    GainProportional = pid.GainProportional,
                    //    GainIntegral = pid.GainIntegral,
                    //    GainDerivative = pid.GainDerivative
                    //}).ToList()
                });
            }
        }
    }
}
