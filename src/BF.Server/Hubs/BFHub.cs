using BF.Common.Events;
using BF.Service.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.Server.Hubs {

    public class BFHub : Hub {

        private IBeerFactoryEventHandler _eventHandler;

        public BFHub(IBeerFactoryEventHandler eventHandler) {
            _eventHandler = eventHandler;
        }

        public async Task TemperatureChangeOccured(string temperatureChangeJson) {
            _eventHandler.TemperatureChangeFired(temperatureChangeJson.ToEvent<TemperatureChange>());
            await Clients.All.SendAsync("TemperatureChangeOccured", temperatureChangeJson);
        }

        public async Task PumpChangeOccured(string pumpChangeJson) {
            _eventHandler.PumpChangeFired(pumpChangeJson.ToEvent<PumpChange>());
            await Clients.All.SendAsync("PumpChangeOccured", pumpChangeJson);
        }

        public async Task PidChangeOccured(string pidChangeJson) {
            _eventHandler.PidChangeFired(pidChangeJson.ToEvent<PidChange>());
            await Clients.All.SendAsync("PidChangeOccured", pidChangeJson);
        }

        public async Task SsrChangeOccured(string ssrChangeJson) {
            _eventHandler.SsrChangeFired(ssrChangeJson.ToEvent<SsrChange>());
            await Clients.All.SendAsync("SsrChangeOccured", ssrChangeJson);
        }

        public async Task ConnectionStatusChangeOccured(string connectionStatusChangeJson) {
            _eventHandler.ConnectionStatusChangeFired(connectionStatusChangeJson.ToEvent<ConnectionStatusChange>());
            await Clients.All.SendAsync("ConnectionStatusChangeOccured", connectionStatusChangeJson);
        }

        // Only to Raspberry PI
        public async Task PidRequestOccured(string pidRequestJson) {
            _eventHandler.PidRequestFired(pidRequestJson.ToEvent<PidRequest>());
            await Clients.All.SendAsync("PidRequestOccured", pidRequestJson);
        }

        // Only to Raspberry PI
        public async Task PumpRequestOccured(string pumpRequestJson) {
            _eventHandler.PidRequestFired(pumpRequestJson.ToEvent<PidRequest>());
            await Clients.All.SendAsync("PumpRequestOccured", pumpRequestJson);
        }



        public async Task Message(string messageReceived) {
            await Clients.All.SendAsync("Message", messageReceived);
        }

        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }

    public class ComponentRestrictedRequirement : 
                    AuthorizationHandler<ComponentRestrictedRequirement, HubInvocationContext>,
                    IAuthorizationRequirement {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ComponentRestrictedRequirement requirement,
                                                       HubInvocationContext resource) {
            if (IsComponentAllowedToDoThis(resource.HubMethodName, context.User.Identity.Name) &&
                context.User.Identity.Name.EndsWith("@microsoft.com")) {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        private bool IsComponentAllowedToDoThis(string hubMethodName, string currentUsername) {
            return !(currentUsername.Equals("asdf42@microsoft.com") &&
                hubMethodName.Equals("banUser", StringComparison.OrdinalIgnoreCase));
        }
    }
}
