using BF.Common.Events;
using BF.Service.Events;
using BF.Service.Prism.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Prism.Events;
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

        //private ILogger Logger { get; set; }

        private HubConnection _connection;

        //public SignalRPrismBeerFactoryEventHandler(IEventAggregator eventAggregator, ILogger<IBeerFactoryEventHandler> logger) : 
        //    base(eventAggregator, logger) {
            ///Logger = Log.Logger;
        //    Task.Run(() => Connect());
        //

        public SignalRPrismBeerFactoryEventHandler(IEventAggregator eventAggregator, ILoggerFactory loggerFactory) : 
            base(eventAggregator, loggerFactory) {
            ///Logger = Log.Logger;
            Task.Run(() => Connect());
        }

        public virtual async Task Connect() {
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

                _connection.On<string>("InitializationChangeFired", 
                    (jsonEvent) => {
                        base.InitializationChangeFired(jsonEvent.ToEvent<InitializationChange>().LogSignalREvent<InitializationChange>());
                    });
                _connection.On<string>("TemperatureChangeFired", 
                    (jsonEvent) => {
                        base.TemperatureChangeFired(jsonEvent.ToEvent<TemperatureChange>().LogSignalREvent<TemperatureChange>());
                    }); 
                //_connection.On<string>("ThermometerChangeFired", 
                //    (jsonEvent) => {
                //       base.ThermometerChangeFired(jsonEvent.ToEvent<ThermometerChange>().LogSignalREvent<ThermometerChange>());
                //    });
                _connection.On<string>("PumpRequestFired", 
                    (jsonEvent) => {
                        base.PumpRequestFired(jsonEvent.ToEvent<PumpRequest>().LogSignalREvent<PumpRequest>());
                    });
                _connection.On<string>("PumpChangeFired", 
                    (jsonEvent) => {
                        base.PumpChangeFired(jsonEvent.ToEvent<PumpChange>().LogSignalREvent<PumpChange>());
                    });
                _connection.On<string>("PidRequestFired", 
                    (jsonEvent) => {
                        base.PidRequestFired(jsonEvent.ToEvent<PidRequest>().LogSignalREvent<PidRequest>());
                    });
                _connection.On<string>("PidChangeFired", 
                    (jsonEvent) => {
                        base.PidChangeFired(jsonEvent.ToEvent<PidChange>().LogSignalREvent<PidChange>());
                    });
                _connection.On<string>("SsrChangeFired", 
                    (jsonEvent) => {
                        base.SsrChangeFired(jsonEvent.ToEvent<SsrChange>().LogSignalREvent<SsrChange>());
                    });
                _connection.On<string>("ConnectionStatusRequestFired", 
                    (jsonEvent) => {
                        base.ConnectionStatusRequestFired(jsonEvent.ToEvent<ConnectionStatusRequest>().LogSignalREvent<ConnectionStatusRequest>());
                    });
                _connection.On<string>("ConnectionStatusChangeFired", 
                    (jsonEvent) => {
                        base.ConnectionStatusChangeFired(jsonEvent.ToEvent<ConnectionStatusChange>().LogSignalREvent<ConnectionStatusChange>());
                    });

                _connection.Reconnected += OnConnection;

                await _connection.StartAsync();

                //if (_connection.IsConnected())
                //    InitializationChangeFired(new InitializationChange());
            } catch (Exception ex) {
                //Logger.Warning($"Unable to connecto to SignalR server: {ex}");
            }
        }

        private Task OnConnection(string arg) {

            return Task.CompletedTask;
           
        }


        public override void InitializationChangeFired(InitializationChange initializationChange) {
            if (_connection.IsConnected()) _connection.InvokeAsync("InitializationChangeFired", initializationChange.LogSignalREvent<InitializationChange>().ToJson());
            base.InitializationChangeFired(initializationChange);
        }

        public override void TemperatureChangeFired(TemperatureChange temperatureChange) {
            if (_connection.IsConnected()) _connection.InvokeAsync("TemperatureChangeFired", temperatureChange.LogSignalREvent<TemperatureChange>().ToJson());
            base.TemperatureChangeFired(temperatureChange);
        }

        //public override void ThermometerChangeFired(ThermometerChange thermometerChange) {
        //    if (_connection.IsConnected()) _connection.InvokeAsync("ThermometerChangeFired", thermometerChange.LogSignalREvent<ThermometerChange>().ToJson());
        //    base.ThermometerChangeFired(thermometerChange);
        //}
    
        public override void PumpRequestFired(PumpRequest pumpRequest) {
            if (_connection.IsConnected()) _connection.InvokeAsync("PumpRequestFired", pumpRequest.LogSignalREvent<PumpRequest>().ToJson());
            base.PumpRequestFired(pumpRequest);
        }

        public override void PumpChangeFired(PumpChange pumpChange) {
            if (_connection.IsConnected()) _connection.InvokeAsync("PumpChangeFired", pumpChange.LogSignalREvent<PumpChange>().ToJson());
            base.PumpChangeFired(pumpChange);
        }

        public override void PidRequestFired(PidRequest pidRequest) {
            if (_connection.IsConnected()) _connection.InvokeAsync("PidRequestFired", pidRequest.LogSignalREvent<PidRequest>().ToJson());
            base.PidRequestFired(pidRequest);
        }

        public override void PidChangeFired(PidChange pidChange) {
            if (_connection.IsConnected()) if (_connection.IsConnected()) _connection.InvokeAsync("PidChangeFired", pidChange.LogSignalREvent<PidChange>().ToJson());
            base.PidChangeFired(pidChange);
        }

        public override void SsrChangeFired(SsrChange ssrChange) {
            if (_connection.IsConnected()) _connection.InvokeAsync("SsrChangeFired", ssrChange.LogSignalREvent<SsrChange>().ToJson());
            base.SsrChangeFired(ssrChange);
        }

        public override void ConnectionStatusRequestFired(ConnectionStatusRequest connectionStatusRequest) {
            if (_connection.IsConnected()) _connection.InvokeAsync("ConnectionStatusRequestFired", connectionStatusRequest.LogSignalREvent<ConnectionStatusRequest>().ToJson());
            base.ConnectionStatusRequestFired(connectionStatusRequest);
        }

        public override void ConnectionStatusChangeFired(ConnectionStatusChange connectionStatusChange) {
            if (_connection.IsConnected()) _connection.InvokeAsync("ConnectionStatusChangeFired", connectionStatusChange.LogSignalREvent<ConnectionStatusChange>().ToJson());
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

    public static class HubConnectionHelper {

        public static ILogger Logger { get; set; }

        public static bool IsConnected(this HubConnection hubConnection) {
            return hubConnection != null && 
                   hubConnection.State == HubConnectionState.Connected;
        }

        public static bool IsVerbose { get; set; } = true;

        public static string Environment { get; set; }

        public static T LogSignalREvent<T>(this IEventPayload eventPayload) where T : IEventPayload {
            if (IsVerbose) Logger?.LogDebug($"{Environment} : {eventPayload.GetType().Name}");
            return (T)eventPayload;
        }

        public static T LogLocalEvent<T>(this IEventPayload eventPayload) where T : IEventPayload {
            if (IsVerbose) Logger?.LogDebug($"{Environment} : {eventPayload.GetType().Name}");
            return (T)eventPayload;
        }
        
    }
}