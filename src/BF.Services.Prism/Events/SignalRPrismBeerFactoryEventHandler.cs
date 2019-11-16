using BF.Common.Components;
using BF.Common.Events;
using BF.Common.States;
using BF.Service.Events;
using BF.Service.Prism.Events;
using BF.Services.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace BF.Services.Prism.Events {
    public class SignalRPrismBeerFactoryEventHandler : PrismBeerFactoryEventHandler {

        private ILogger Logger { get; set; }

       
        private HubConnection _connection;

        public SignalRPrismBeerFactoryEventHandler(IEventAggregator eventAggregator, ILoggerFactory loggerFactory, IApplicationConfig applicationConfig) :
            base(eventAggregator, loggerFactory, applicationConfig) {
            Logger = loggerFactory.CreateLogger<SignalRPrismBeerFactoryEventHandler>();

            Task.Run(() => Connect());
        }

        public virtual async Task Connect() {
           
        try {
            //var pw = Config["BFHub:Credentials:Password"];
            var credentailString = $"{_applicationConfig.UserName}:{_applicationConfig.Password}";
            var credential = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(credentailString));

            if (DeviceHelper.GetDevice() == Device.Server || DeviceHelper.GetDevice() == Device.RaspberryPi) {
                _connection = new HubConnectionBuilder()
                    .WithUrl("https://emrsd-ws-bf.azurewebsites.net/bfHub",
                    options => {
                        options.Headers.Add("Authorization", $"Basic {credential}");

                    })
                    .WithAutomaticReconnect()
                    .AddJsonProtocol()
                    .Build();
            } else {
                _connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:44355/bfHub",
                    options => {
                        options.Headers.Add("Authorization", $"Basic {credential}");
                        options.HttpMessageHandlerFactory = (handler) => {
                            if (handler is HttpClientHandler clientHandler) {
                                clientHandler.ServerCertificateCustomValidationCallback = ValidateCertificate;
                            }
                            return handler;
                        };
                    })
                    .WithAutomaticReconnect()
                    .AddJsonProtocol()
                    .Build();
            }

            _connection.On<string, string, string>("ComponentStateChangeReceived",
                (string userName, string componentStateTypeStr, string componentStateChangeJson) => {
                    var asm = typeof(ComponentState).Assembly;
                    Type componentStateType = asm.GetType(componentStateTypeStr);
                    var componentStateChange = JsonConvert.DeserializeObject(componentStateChangeJson, componentStateType);

                    //Logger.LogInformation($"SignalR-Change: {userName} {_applicationConfig.Device} {componentStateChange.}");

                    // TODO: There has to be a better way.
                    if (componentStateType == typeof(ComponentStateChange<ThermometerState>)) 
                        BaseComponentStateChangeFiring<ThermometerState>(componentStateChange, userName);
                    if (componentStateType == typeof(ComponentStateChange<PidControllerState>))
                        base.ComponentStateChangeFiring((ComponentStateChange<PidControllerState>)componentStateChange);
                    if (componentStateType == typeof(ComponentStateChange<PumpState>))
                        base.ComponentStateChangeFiring((ComponentStateChange<PumpState>)componentStateChange);
                    if (componentStateType == typeof(ComponentStateChange<SsrState>))
                        base.ComponentStateChangeFiring((ComponentStateChange<SsrState>)componentStateChange);
                    if (componentStateType == typeof(ComponentStateChange<ConnectionState>)) 
                        base.ComponentStateChangeFiring((ComponentStateChange<ConnectionState>)componentStateChange);
                    if (componentStateType == typeof(ComponentStateChange<BFState>))
                        base.ComponentStateChangeFiring((ComponentStateChange<BFState>)componentStateChange);
                });

            _connection.On<string, string, string>("ComponentStateRequestReceived",
                (string userName, string componentStateTypeStr, string componentStateRequestJson) => {
                    var asm = typeof(ComponentState).Assembly;
                    Type componentStateType = asm.GetType(componentStateTypeStr);
                    var componentStateRequest = JsonConvert.DeserializeObject(componentStateRequestJson, componentStateType);

                    Logger.LogInformation($"SignalR-Request: {userName} {_applicationConfig.Device} {componentStateType}");

                    // TODO: There has to be a better way.
                    if (componentStateType == typeof(ComponentStateRequest<PidControllerRequestState>))
                        base.ComponentStateRequestFiring((ComponentStateRequest<PidControllerRequestState>)componentStateRequest);
                    if (componentStateType == typeof(ComponentStateRequest<PumpRequestState>))
                        base.ComponentStateRequestFiring((ComponentStateRequest<PumpRequestState>)componentStateRequest);
                    if (componentStateType == typeof(ComponentStateRequest<SsrRequestState>))
                        base.ComponentStateRequestFiring((ComponentStateRequest<SsrRequestState>)componentStateRequest);
                    if (componentStateType == typeof(ComponentStateRequest<BFRequestState>))
                        base.ComponentStateRequestFiring((ComponentStateRequest<BFRequestState>)componentStateRequest);
                });

            _connection.Reconnected += OnConnection;

            await _connection.StartAsync();

        } catch (Exception ex) {
            Logger.LogWarning($"Unable to connecto to SignalR server: {ex.Message}");
        }
                
        }

        private void BaseComponentStateChangeFiring<T>(object componentStateChangeObj, string userName) where T : ComponentState{
            var componentStateChange = (ComponentStateChange<T>)componentStateChangeObj;
            Logger.LogInformation($"SignalR-Change: {userName} {_applicationConfig.Device} {componentStateChange.CurrentState}");
            base.ComponentStateChangeFiring(componentStateChange);
        }

        private Task OnConnection(string arg) {
            return Task.CompletedTask;
        }

        public override void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) {
            var isApplianceState = typeof(T) != typeof(ThermocoupleState);
            var isServerState = typeof(T) == typeof(ConnectionState);
            componentStateChange.FromUserName = _applicationConfig.UserName;

            if (_connection.IsConnected() && 
                ((isApplianceState && _applicationConfig.IsAppliance()) || (isServerState && _applicationConfig.IsServer()))
                ) { 
                _connection.InvokeAsync("ComponentStateChangeBroadcasted",
                                        componentStateChange.FromUserName,
                                        componentStateChange.GetType().ToString(),
                                        componentStateChange.ToJson());
            }
            base.ComponentStateChangeFiring(componentStateChange);
        }

        public override void ComponentStateRequestFiring<T>(ComponentStateRequest<T> componentStateRequest) {
            componentStateRequest.FromUserName = _applicationConfig.UserName;


            if (_connection.IsConnected() &&
                _applicationConfig.IsServer()
                ) {
                _connection.InvokeAsync("ComponentStateRequestBroadcasted",
                                        componentStateRequest.FromUserName,
                                        componentStateRequest.GetType().ToString(),
                                        componentStateRequest.ToJson());
            }
            base.ComponentStateRequestFiring(componentStateRequest);
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

        //public static T LogSignalREvent<T>(this IEventPayload eventPayload) where T : IEventPayload {
        //    if (IsVerbose) Logger?.LogDebug($"{Environment} : {eventPayload.GetType().Name}");
        //    return (T)eventPayload;
        //}


        //        public virtual void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState {
        //    _eventAggregator.GetEvent<ComponentStateChangeEvent<ComponentStateChange<T>>>().Publish(componentStateChange);
        //}

        public static T LogLocalEvent<T>(this IEventPayload eventPayload) where T : IEventPayload {
            if (IsVerbose) Logger?.LogDebug($"{Environment} : {eventPayload.GetType().Name}");
            return (T)eventPayload;
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}