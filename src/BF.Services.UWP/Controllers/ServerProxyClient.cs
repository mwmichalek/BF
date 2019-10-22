using BF.Service.Events;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using BF.Common.Events;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace BF.Services.UWP.Controllers {

    public interface IServerProxyClient {

    }

    public class ServerProxyClient : IServerProxyClient {

        private ILogger Logger { get; set; }

        private HubConnection _connection;
        private IBeerFactoryEventHandler _eventHandler;

        public ServerProxyClient(IBeerFactoryEventHandler eventHandler) {
            Logger = Log.Logger;
            _eventHandler = eventHandler;

            Task.Run(() => Connect());

        }

        public async Task Connect() {
            var processorArchitecture = Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture;

            try {

                if (processorArchitecture == ProcessorArchitecture.Arm ||
                       processorArchitecture == ProcessorArchitecture.X86) {
                    _connection = new HubConnectionBuilder()
                        .WithUrl("https://emrsd-ws-bf.azurewebsites.net/bfHub")
                        .WithAutomaticReconnect()
                        .Build();
                } else {
                    _connection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:44355/bfHub", options => {
                            options.HttpMessageHandlerFactory = (handler) => {
                                if (handler is HttpClientHandler clientHandler) {
                                    clientHandler.ServerCertificateCustomValidationCallback = ValidateCertificate;
                                }
                                return handler;
                            };
                        })
                        .WithAutomaticReconnect()
                        .Build();
                }

                _connection.On<string>("PumpRequestFired", PumpRequestFired);
                _connection.On<string>("PidRequestFired", PidRequestFired);
                _connection.On<string>("ConnectionStatusRequestFired", ConnectionStatusRequestFired);

                _eventHandler.TemperatureChangeOccured(TemperatureChangeOccured);
                _eventHandler.PumpChangeOccured(PumpChangeOccured);
                _eventHandler.PidChangeOccured(PidChangeOccured);
                _eventHandler.SsrChangeOccured(SsrChangeOccured);
                _eventHandler.ConnectionStatusChangeOccured(ConnectionStatusChangeOccured);
                //eventHandler.ThermometerChangeOccured(ThermometerChangeOccured);
                //eventHandler.PumpRequestOccured(PumpRequestOccured);\
                //eventHandler.PidRequestOccured(PidRequestOccured);

                await _connection.StartAsync();
            } catch (Exception ex) {
                Logger.Warning($"Unable to connecto to SignalR server: {ex}");
            }
        }

        public void TemperatureChangeOccured(TemperatureChange temperatureChange) {            
            _connection.InvokeAsync("TemperatureChangeOccured", temperatureChange.ToJson());
        }

        public void PumpChangeOccured(PumpChange pumpChange) {
            _connection.InvokeAsync("PumpChangeOccured", pumpChange.ToJson());
        }

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

        //public void PidRequestOccured(PidRequest pidRequest) {
        //    _connection.InvokeAsync("PidRequestOccured", pidRequest.ToJson());
        //}

        //public void ThermometerChangeOccured(ThermometerChange thermometerChange) {
        //    _connection.InvokeAsync("ThermometerChangeOccured", thermometerChange.ToJson());
        //}


        //public void PumpRequestOccured(PumpRequest pumpRequest) {
        //    _connection.InvokeAsync("PumpRequestOccured", pumpRequest.ToJson());
        //}

        bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            // TODO: You can do custom validation here, or just return true to always accept the certificate.
            // DO NOT use custom validation logic in a production application as it is insecure.
            return true;
        }
    }



}
