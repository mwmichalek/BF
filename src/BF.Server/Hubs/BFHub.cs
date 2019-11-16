using BF.Common.Events;
using BF.Common.States;
using BF.Service.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.Server.Hubs {

    [Authorize]
    public class BFHub : Hub {

        private IBeerFactoryEventHandler _eventHandler;
        private ILogger logger;

        private static IList<string> userNames = new List<string>();

        public BFHub(IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {
            this.logger = loggerFactory.CreateLogger<BFHub>();
            _eventHandler = eventHandler;
        }

        public override async Task OnConnectedAsync() {
            var userName = Context.User.Identity.Name;
            if (!userNames.Contains(userName)) {
                userNames.Add(userName);

                var componentStateChange = new ComponentStateChange<ConnectionState> {
                    FromUserName = userName,
                    CurrentState = new ConnectionState {
                        Status = ConnectionStatus.Connected
                    }
                };
                logger.LogInformation($"User connected: {userName}");
                await Clients.All.SendAsync("ComponentStateChangeReceived",
                                               componentStateChange.GetType().ToString(),
                                               componentStateChange.ToJson());
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            var userName = Context.User.Identity.Name;
            if (userNames.Contains(userName)) {
                userNames.Remove(userName);

                var componentStateChange = new ComponentStateChange<ConnectionState> {
                    FromUserName = userName,
                    CurrentState = new ConnectionState {
                        Status = ConnectionStatus.Disconnected
                    }
                };
                logger.LogInformation($"User disconnected: {userName}");

                await Clients.All.SendAsync("ComponentStateChangeReceived",
                                               componentStateChange.GetType().ToString(),
                                               componentStateChange.ToJson());
            }
        }

        public async Task ComponentStateChangeBroadcasted(string userName, string componentStateType, string componentStateChangeJson) {
            //var userName = Context.User.Identity.Name;

            await Clients.Others.SendAsync("ComponentStateChangeReceived", userName, componentStateType, componentStateChangeJson);
            //await Clients.Client("server").SendAsync("ComponentStateChangeReceived", userName, componentStateType, componentStateChangeJson);
        }

        public async Task ComponentStateRequestBroadcasted(string userName, string componentStateType, string componentStateRequestJson) {
            //var userName = Context.User.Identity.Name;

            await Clients.Others.SendAsync(userName, "ComponentStateRequestReceived", userName, componentStateType, componentStateRequestJson);
            //await Clients.Client("raspberrypi").SendAsync(userName, "ComponentStateRequestReceived", userName, componentStateType, componentStateRequestJson);
        }


        public async Task Message(string messageReceived) {
            await Clients.All.SendAsync("Message", messageReceived);
        }

        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }

    //public class ComponentRestrictedRequirement : 
    //                AuthorizationHandler<ComponentRestrictedRequirement, HubInvocationContext>,
    //                IAuthorizationRequirement {

    //    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
    //                                                   ComponentRestrictedRequirement requirement,
    //                                                   HubInvocationContext resource) {
    //        if (IsComponentAllowedToDoThis(resource.HubMethodName, context.User.Identity.Name) &&
    //            context.User.Identity.Name.EndsWith("@microsoft.com")) {
    //            context.Succeed(requirement);
    //        }
    //        return Task.CompletedTask;
    //    }

    //    private bool IsComponentAllowedToDoThis(string hubMethodName, string currentUsername) {
    //        return !(currentUsername.Equals("asdf42@microsoft.com") &&
    //            hubMethodName.Equals("banUser", StringComparison.OrdinalIgnoreCase));
    //    }
    //}
}
