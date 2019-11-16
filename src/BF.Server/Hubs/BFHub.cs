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

    public static class UserHandler {

        public static HashSet<string> UserNames = new HashSet<string>();

        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    [Authorize]
    public class BFHub : Hub {

        private IBeerFactoryEventHandler _eventHandler;
        private ILogger logger;

        public BFHub(IBeerFactoryEventHandler eventHandler, ILoggerFactory loggerFactory) {
            this.logger = loggerFactory.CreateLogger<BFHub>();
            _eventHandler = eventHandler;
        }

        public override async Task OnConnectedAsync() {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);

            var userName = Context.User.Identity.Name;
            if (UserHandler.UserNames.Add(userName)) {

                var componentStateChange = new ComponentStateChange<ConnectionState> {
                    FromUserName = userName,
                    CurrentState = new ConnectionState {
                        Status = ConnectionStatus.Connected
                    }
                };
                logger.LogInformation($"User connected: {userName} : {UserHandler.UserNames.Count} : {UserHandler.ConnectedIds.Count}");
                await Clients.All.SendAsync("ComponentStateChangeReceived",
                                               componentStateChange.GetType().ToString(),
                                               componentStateChange.ToJson());
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);

            var userName = Context.User.Identity.Name;
            if (UserHandler.UserNames.Remove(userName)) {
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
            //logger.LogInformation($"HUB-Change: {userName} : {componentStateType} : {UserHandler.UserNames.Count} : {UserHandler.ConnectedIds.Count}");
            await Clients.Others.SendAsync("ComponentStateChangeReceived", userName, componentStateType, componentStateChangeJson);
        }

        public async Task ComponentStateRequestBroadcasted(string userName, string componentStateType, string componentStateRequestJson) {
            //logger.LogInformation($"HUB-Request: {userName} : {componentStateType} : {UserHandler.UserNames.Count} : {UserHandler.ConnectedIds.Count}");
            await Clients.Others.SendAsync(userName, "ComponentStateRequestReceived", userName, componentStateType, componentStateRequestJson);
        }


        public async Task Message(string messageReceived) {
            await Clients.All.SendAsync("Message", messageReceived);
        }

        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }

}
