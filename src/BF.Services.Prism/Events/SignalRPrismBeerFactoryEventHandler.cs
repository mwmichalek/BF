using BF.Common.Events;
using BF.Service.Prism.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BF.Services.Prism.Events {
    public class SignalRPrismBeerFactoryEventHandler : PrismBeerFactoryEventHandler {

        private ILogger Logger { get; set; }

        private HubConnection _connection;

        public SignalRPrismBeerFactoryEventHandler(IEventAggregator eventAggregator) : base(eventAggregator) {
            Logger = Log.Logger;
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

                _connection.On<string>("TemperatureChangeFired", (jsonEvent) => base.TemperatureChangeFired(jsonEvent.ToEvent<TemperatureChange>()));
                _connection.On<string>("ThermometerChangeFired", (jsonEvent) => base.ThermometerChangeFired(jsonEvent.ToEvent<ThermometerChange>()));
                _connection.On<string>("PumpRequestFired", (jsonEvent) => base.PumpRequestFired(jsonEvent.ToEvent<PumpRequest>()));
                _connection.On<string>("PumpChangeFired", (jsonEvent) => base.PumpChangeFired(jsonEvent.ToEvent<PumpChange>()));
                _connection.On<string>("PidRequestFired", (jsonEvent) => base.PidRequestFired(jsonEvent.ToEvent<PidRequest>()));
                _connection.On<string>("PidChangeFired", (jsonEvent) => base.PidChangeFired(jsonEvent.ToEvent<PidChange>()));
                _connection.On<string>("SsrChangeFired", (jsonEvent) => base.SsrChangeFired(jsonEvent.ToEvent<SsrChange>()));
                _connection.On<string>("ConnectionStatusRequestFired", (jsonEvent) => base.ConnectionStatusRequestFired(jsonEvent.ToEvent<ConnectionStatusRequest>()));
                _connection.On<string>("ConnectionStatusChangeFired", (jsonEvent) => base.ConnectionStatusChangeFired(jsonEvent.ToEvent<ConnectionStatusChange>()));
                await _connection.StartAsync();
            } catch (Exception ex) {
                Logger.Warning($"Unable to connecto to SignalR server: {ex}");
            }
        }

        public override void TemperatureChangeFired(TemperatureChange temperatureChange) {
            _connection.InvokeAsync("TemperatureChangeFired", temperatureChange.ToJson());
            base.TemperatureChangeFired(temperatureChange);
        }

        public override void ThermometerChangeFired(ThermometerChange thermometerChange) {
            _connection.InvokeAsync("ThermometerChangeFired", thermometerChange.ToJson());
            base.ThermometerChangeFired(thermometerChange);
        }

        public override void PumpRequestFired(PumpRequest pumpRequest) {
            _connection.InvokeAsync("PumpRequestFired", pumpRequest.ToJson());
            base.PumpRequestFired(pumpRequest);
        }

        public override void PumpChangeFired(PumpChange pumpChange) {
            _connection.InvokeAsync("PumpChangeFired", pumpChange.ToJson());
            base.PumpChangeFired(pumpChange);
        }

        public override void PidRequestFired(PidRequest pidRequest) {
            _connection.InvokeAsync("PidRequestFired", pidRequest.ToJson());
            base.PidRequestFired(pidRequest);
        }

        public override void PidChangeFired(PidChange pidChange) {
            _connection.InvokeAsync("PidChangeFired", pidChange.ToJson());
            base.PidChangeFired(pidChange);
        }

        public override void SsrChangeFired(SsrChange ssrChange) {
            _connection.InvokeAsync("SsrChangeFired", ssrChange.ToJson());
            base.SsrChangeFired(ssrChange);
        }

        public override void ConnectionStatusRequestFired(ConnectionStatusRequest connectionStatusRequest) {
            _connection.InvokeAsync("ConnectionStatusRequestFired", connectionStatusRequest.ToJson());
            base.ConnectionStatusRequestFired(connectionStatusRequest);
        }

        public override void ConnectionStatusChangeFired(ConnectionStatusChange connectionStatusChange) {
            _connection.InvokeAsync("ConnectionStatusChangeFired", connectionStatusChange.ToJson());
            base.ConnectionStatusChangeFired(connectionStatusChange);
        }



        //public void TemperatureChangeOccured(TemperatureChange temperatureChange) {
        //    _connection.InvokeAsync("TemperatureChangeOccured", temperatureChange.ToJson());
        //}

        //public void PumpRequestOccured(PumpRequest pumpRequest) {
        //    _connection.InvokeAsync("PumpRequestOccured", pumpRequest.ToJson());
        //}

        //public void PumpChangeOccured(PumpChange pumpChange) {
        //    _connection.InvokeAsync("PumpChangeOccured", pumpChange.ToJson());
        //}

        //public void PidChangeOccured(PidChange pidChange) {
        //    _connection.InvokeAsync("PidChangeOccured", pidChange.ToJson());
        //}


        //public void SsrChangeOccured(SsrChange ssrChange) {
        //    _connection.InvokeAsync("SsrChangeOccured", ssrChange.ToJson());
        //}


        //public void ConnectionStatusChangeOccured(ConnectionStatusChange connectionStatusChange) {
        //    _connection.InvokeAsync("ConnectionStatusChangeOccured", connectionStatusChange.ToJson());
        //}

        //public void PidRequestOccured(PidRequest pidRequest) {
        //    _connection.InvokeAsync("PidRequestOccured", pidRequest.ToJson());
        //}

        //public void ThermometerChangeOccured(ThermometerChange thermometerChange) {
        //    _connection.InvokeAsync("ThermometerChangeOccured", thermometerChange.ToJson());
        //}




        public void PumpRequestFired(string pumpRequestJson) {
            PumpRequestFired(pumpRequestJson.ToEvent<PumpRequest>());
        }

        public void PidRequestFired(string pidRequestJson) {
            PidRequestFired(pidRequestJson.ToEvent<PidRequest>());
        }

        public void ConnectionStatusRequestFired(string connectionStatusRequestJson) {
            ConnectionStatusRequestFired(connectionStatusRequestJson.ToEvent<ConnectionStatusRequest>());
        }



        private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }
    }
}
