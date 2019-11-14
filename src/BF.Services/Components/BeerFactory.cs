using System;
using System.Collections.Generic;
using BF.Service.Events;
using Microsoft.Extensions.Logging;


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
        }

    }
}
