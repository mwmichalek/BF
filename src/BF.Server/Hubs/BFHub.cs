using BF.Common.Events;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.Server.Hubs {

    public class BFHub : Hub {

        public async Task TemperatureChangeOccured(TemperatureChange temperatureChange) {
            await Clients.Others.SendAsync("TemperatureChangeOccured", temperatureChange);
        }

    }
}
