using Serilog;
using Serilog.Core;
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
using BF.Service.Pid;
using BF.Service.Events;
using BF.Common.Ids;
using BF.Common.Events;

namespace BF.Service {

    public interface IBeerFactory {
        
        List<Thermometer> Thermometers { get; }

        List<PidController> PidControllers { get; }

        List<Ssr> Ssrs { get; }
    }

    public class BeerFactory : IBeerFactory {

        private ILogger Logger { get; set; }

        private List<Phase> _phases = new List<Phase>();

        public List<PidController> PidControllers { get; set; } = new List<PidController>();

        public List<Ssr> Ssrs { get; set; } = new List<Ssr>();

        public List<Thermometer> Thermometers { get; set; } = new List<Thermometer>();

        private IBeerFactoryEventHandler _eventHandler;

        public BeerFactory(IBeerFactoryEventHandler eventHandler) {
            _eventHandler = eventHandler;
            Logger = Log.Logger;

            //_eventAggregator.GetEvent<ThermometerChangeEvent>().Subscribe(PrintStatusPub, ThreadOption.PublisherThread);
            //_eventAggregator.GetEvent<ThermometerChangeEvent>().Subscribe(PrintStatusBack, ThreadOption.BackgroundThread);
            //_eventAggregator.GetEvent<ThermometerChangeEvent>().Subscribe(PrintStatusUi, ThreadOption.UIThread);

            
            

            for (int index = 1; index <= (int)ThermometerId.FERM; index++ )
                Thermometers.Add(new Thermometer(_eventHandler, (ThermometerId)index));

            _phases.Add(new Phase(PhaseId.FillStrikeWater, 20));
            _phases.Add(new Phase(PhaseId.HeatStrikeWater, 40));
            _phases.Add(new Phase(PhaseId.Mash, 90));
            _phases.Add(new Phase(PhaseId.MashOut, 90));
            _phases.Add(new Phase(PhaseId.Sparge, 60));
            _phases.Add(new Phase(PhaseId.Boil, 90));
            _phases.Add(new Phase(PhaseId.Chill, 30));

            var hltSsr = new Ssr(_eventHandler, SsrId.HLT);
            hltSsr.Percentage = 0;
            hltSsr.Start();

            Ssrs.Add(hltSsr);

            var bkSsr = new Ssr(_eventHandler, SsrId.BK);
            bkSsr.Percentage = 0;
            bkSsr.Start();

            Ssrs.Add(bkSsr);

            //ConfigureEvents();

            var _hltPidController = new PidController(_eventHandler, 
                                                      PidControllerId.HLT,
                                                      hltSsr, 
                                                      Thermometers.GetById(ThermometerId.HLT));
            //_hltPidController.GainProportional = 18;
            //_hltPidController.GainIntegral = 1.5;
            //_hltPidController.GainDerivative = 22.5;

            _hltPidController.GainProportional = 0.5;
            _hltPidController.GainIntegral = 0.5;
            _hltPidController.GainDerivative = 0.5;

            _hltPidController.SetPoint = 120;

            PidControllers.Add(_hltPidController);

            //_hltPidController.Start();

            _eventHandler.PidRequestOccured(PidRequestOccured);

        }

        public void PidRequestOccured(PidRequest pidRequest) {

            var pidController = PidControllers.SingleOrDefault(pid => pid.Id == pidRequest.Id);
            pidController.IsEngaged = pidRequest.IsEngaged;
            pidController.SetPoint = pidRequest.SetPoint;
        }

    }
}
