using BF.Service.Events;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using BF.Common.Events;
using Newtonsoft.Json;

namespace BF.Services.UWP.Controllers {

    public interface IServerProxyClient {

    }

    public class ServerProxyClient : IServerProxyClient {

        private HubConnection _connection;
        private IBeerFactoryEventHandler _eventHandler;

        public ServerProxyClient(IBeerFactoryEventHandler eventHandler) {
            _eventHandler = eventHandler;
            _connection = new HubConnectionBuilder()
                .WithUrl("https://emrsd-ws-bf.azurewebsites.net/bfHub")
                //.WithUrl("https://localhost:44355/bfHub")
                .Build();

            _connection.On<string>("PumpRequestFired", PumpRequestFired);
            _connection.On<string>("PidRequestFired", PidRequestFired);
            _connection.On<string>("ConnectionStatusRequestFired", ConnectionStatusRequestFired);

            eventHandler.TemperatureChangeOccured(TemperatureChangeOccured);
            //eventHandler.ThermometerChangeOccured(ThermometerChangeOccured);
            //eventHandler.PumpRequestOccured(PumpRequestOccured);
            eventHandler.PumpChangeOccured(PumpChangeOccured);
            //eventHandler.PidRequestOccured(PidRequestOccured);
            eventHandler.PidChangeOccured(PidChangeOccured);
            eventHandler.SsrChangeOccured(SsrChangeOccured);
            eventHandler.ConnectionStatusChangeOccured(ConnectionStatusChangeOccured);

            try {
                _connection.StartAsync();
            } catch (Exception ex) {

            }

        }

        public void TemperatureChangeOccured(TemperatureChange temperatureChange) {            
            _connection.InvokeAsync("TemperatureChangeOccured", temperatureChange.ToJson());
        }

        //public void ThermometerChangeOccured(ThermometerChange thermometerChange) {
        //    _connection.InvokeAsync("ThermometerChangeOccured", thermometerChange.ToJson());
        //}


        //public void PumpRequestOccured(PumpRequest pumpRequest) {
        //    _connection.InvokeAsync("PumpRequestOccured", pumpRequest.ToJson());
        //}

        public void PumpChangeOccured(PumpChange pumpChange) {
            _connection.InvokeAsync("PumpChangeOccured", pumpChange.ToJson());
        }


        //public void PidRequestOccured(PidRequest pidRequest) {
        //    _connection.InvokeAsync("PidRequestOccured", pidRequest.ToJson());
        //}

        public void PidChangeOccured(PidChange pidChange) {
            _connection.InvokeAsync("PidChangeOccured", pidChange.ToJson());
        }


        public void SsrChangeOccured(SsrChange ssrChange) {
            _connection.InvokeAsync("SsrChangeOccured", ssrChange.ToJson());
        }


        public void ConnectionStatusChangeOccured(ConnectionStatusChange connectionStatusChange) {
            _connection.InvokeAsync("ConnectionStatusChangeOccured", connectionStatusChange.ToJson());
        }





        public void PumpRequestFired(string pumpRequestJson) {
            _eventHandler.PumpRequestFired(pumpRequestJson.ToEvent<PumpRequest>());
        }

        public void PidRequestFired(string pidRequestJson) {
            _eventHandler.PidRequestFired(pidRequestJson.ToEvent<PidRequest>());
        }

        public void ConnectionStatusRequestFired(string connectionStatusRequestJson) { 
            _eventHandler.ConnectionStatusRequestFired(connectionStatusRequestJson.ToEvent<ConnectionStatusRequest>());
        }
    }



}
