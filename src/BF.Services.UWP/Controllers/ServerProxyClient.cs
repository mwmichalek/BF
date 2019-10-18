using BF.Service.Events;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using BF.Common.Events;

namespace BF.Services.UWP.Controllers {

    public interface IServerProxyClient {

    }

    public class ServerProxyClient : IServerProxyClient {

        private HubConnection connection;


        public ServerProxyClient(IBeerFactoryEventHandler eventHandler) {

            connection = new HubConnectionBuilder()
                .WithUrl("https://emrsd-ws-bf.azurewebsites.net/BFHub")
                .Build();

            //connection.On<string, string>("ReceiveMessage", (user, message) => {
                //Logger.Information($"User: {user}, Message: {message}");
            //});

            try {
                connection.StartAsync();

            } catch (Exception ex) {

            }

            eventHandler.TemperatureChangeOccured(TemperatureChangeOccured);
        }

        public void TemperatureChangeOccured(TemperatureChange temperatureChange) {
            connection.InvokeAsync("TemperatureChangeOccured", temperatureChange);
        }

    }



}
