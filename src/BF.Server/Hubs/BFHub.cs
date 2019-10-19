using BF.Common.Events;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.Server.Hubs {

    public class BFHub : Hub {

        public async Task TemperatureChangeOccured(string temperatureChangeJson) {
            await Clients.All.SendAsync("TemperatureChangeOccured", temperatureChangeJson);
        }

        public async Task Message(string messageReceived) {
            await Clients.All.SendAsync("Message", messageReceived);
        }

        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
