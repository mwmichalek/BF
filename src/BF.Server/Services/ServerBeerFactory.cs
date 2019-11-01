using BF.Service.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.Server.Services {

    public class ServerBeerFactory {

        private ILogger Logger { get; set; }

        public IBeerFactoryEventHandler _eventHandler;

        public ServerBeerFactory(IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {

        }

    }
}
