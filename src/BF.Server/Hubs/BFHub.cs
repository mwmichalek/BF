using BF.Common.Events;
using BF.Service.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.Server.Hubs {

    public class BFHub : Hub {

        private IBeerFactoryEventHandler _eventHandler;
        private ILogger logger;

        public BFHub(IBeerFactoryEventHandler eventHandler, ILogger<BFHub> logger) {
            this.logger = logger;
            _eventHandler = eventHandler;
        }

        public override Task OnConnectedAsync() {
            return base.OnConnectedAsync();
        }

        public async Task InitializationChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("InitializationChangeFired", jsonEvent);
        }

        public async Task TemperatureChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("TemperatureChangeFired", jsonEvent);
        }

        public async Task ThermometerChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("ThermometerChangeFired", jsonEvent);
        }

        public async Task PumpRequestFired(string jsonEvent) {
            await Clients.Others.SendAsync("PumpRequestFired", jsonEvent);
        }

        public async Task PumpChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("PumpChangeFired", jsonEvent);
        }

        public async Task PidRequestFired(string jsonEvent) {
            await Clients.Others.SendAsync("PidRequestFired", jsonEvent);
        }

        public async Task PidChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("PidChangeFired", jsonEvent);
        }

        public async Task SsrChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("SsrChangeFired", jsonEvent);
        }

        public async Task ConnectionStatusRequestFired(string jsonEvent) {
            await Clients.Others.SendAsync("ConnectionStatusRequestFired", jsonEvent);
        }

        public async Task ConnectionStatusChangeFired(string jsonEvent) {
            await Clients.Others.SendAsync("ConnectionStatusChangeFired", jsonEvent);
        }

        //public async Task ComponentStateChangeBroadcasted(string componentStateType) {
        //    await Clients.Others.SendAsync("ComponentStateChangeReceived", componentStateType);
        //}

        public async Task ComponentStateChangeBroadcasted(string componentStateType, string componentStateChangeJson) {
            await Clients.Others.SendAsync("ComponentStateChangeReceived", componentStateType, componentStateChangeJson);
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
