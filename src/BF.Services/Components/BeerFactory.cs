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
using BF.Common.Components;
using BF.Services.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;

namespace BF.Services.Components {

    public class BeerFactory {

        private ILogger Logger { get; set; }

        private List<Phase> _phases = new List<Phase>();

        private PidController[] _pidControllers;

        private Ssr[] _ssrs;

        private Pump[] _pumps;

        private Thermometer[] _thermometers;

        private IBeerFactoryEventHandler _eventHandler;

        public BeerFactory(Thermometer[] thermometers,
                           Ssr[] ssrs,
                           PidController[] pidControllers,
                           Pump[] pumps,
                           IBeerFactoryEventHandler eventHandler, 
                           ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<BeerFactory>();
            _pidControllers = pidControllers;
            _ssrs = ssrs;
            _thermometers = thermometers;
            _pumps = pumps;

            _eventHandler.ComponentStateRequestOccured<BFState>((req) => BroadcastCurrentState());
        }

        public void BroadcastCurrentState() {

            var currentBFState = new BFState {

                PidControllerStates = _pidControllers.ToDictionary(pid => pid.Id, pid => pid.CurrentState),
                SsrStates = _ssrs.ToDictionary(s => s.Id, s => s.CurrentState),
                ThermometerStates = _thermometers.ToDictionary(t => t.Id, t => t.CurrentState),
                PumpStates = _pumps.ToDictionary(p => p.Id, p => p.CurrentState)
            };

            Logger.LogInformation($"Sending BFState to Server.");
            _eventHandler.ComponentStateChangeFiring<BFState>(new ComponentStateChange<BFState> {
                CurrentState = currentBFState
            });
        }

    }
}
