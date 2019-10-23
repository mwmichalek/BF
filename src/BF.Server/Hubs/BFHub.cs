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

        public async Task PumpChangeOccured(string pumpChangeJson) {
            await Clients.All.SendAsync("PumpChangeOccured", pumpChangeJson);
        }

        public async Task PidChangeOccured(string pidChangeJson) {
            await Clients.All.SendAsync("PidChangeOccured", pidChangeJson);
        }

        public async Task SsrChangeOccured(string ssrChangeJson) {
            await Clients.All.SendAsync("SsrChangeOccured", ssrChangeJson);
        }

        public async Task ConnectionStatusChangeOccured(string connectionStatusChangeJson) {
            await Clients.All.SendAsync("ConnectionStatusChangeOccured", connectionStatusChangeJson);
        }


        public async Task PidRequestOccured(string pidRequestJson) {
            await Clients.All.SendAsync("PidRequestOccured", pidRequestJson);
        }





        public async Task Message(string messageReceived) {
            await Clients.All.SendAsync("Message", messageReceived);
        }

        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
