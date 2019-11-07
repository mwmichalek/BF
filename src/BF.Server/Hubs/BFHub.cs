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

    [Authorize]
    public class BFHub : Hub {

        private IBeerFactoryEventHandler _eventHandler;
        private ILogger logger;

        public BFHub(IBeerFactoryEventHandler eventHandler, ILogger<BFHub> logger) {
            this.logger = logger;
            _eventHandler = eventHandler;
        }

        public override Task OnConnectedAsync() {
            var connectionId = Context.ConnectionId;


            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task ComponentStateChangeBroadcasted(string componentStateType, string componentStateChangeJson) {
            await Clients.Others.SendAsync("ComponentStateChangeReceived", componentStateType, componentStateChangeJson);
        }

        public async Task ComponentStateRequestBroadcasted(string componentStateType, string componentStateRequestJson) {
            await Clients.Others.SendAsync("ComponentStateRequestReceived", componentStateType, componentStateRequestJson);
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
