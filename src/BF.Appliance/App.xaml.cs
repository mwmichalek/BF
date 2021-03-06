﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using BF.Service.Controllers;
using BF.Service.UWP.Controllers;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Serilog;
using BF.Service.Events;
using BF.Services.Prism.Events;
using Microsoft.Extensions.Logging;
using BF.Common.Components;
using BF.Services.Components;
using BF.Services.Configuration;
using BF.Common.States;
using BF.Common.Events;

namespace BF.Appliance {
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication {
        public App() {
            InitializeComponent();
        }

        protected override void ConfigureContainer() {

            base.ConfigureContainer();

            //var loggerConfiguration = new LoggerConfiguration()
            //                                    .MinimumLevel.Verbose()
            //                                    .MinimumLevel.Override("Microsoft.ApplicationInsights", Serilog.Events.LogEventLevel.Warning)
            //                                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            //                                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            //                                    .Enrich.FromLogContext()
            //                                    .Enrich.WithExceptionDetails()
            //                                    //.WriteTo.Trace()
            //                                    //.WriteTo.Debug()
            //                                    .WriteTo.LiterateConsole();

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace();


            //Log.Logger = loggerConfiguration.CreateLogger();

            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();

            Container.RegisterInstance<ILoggerFactory>(loggerFactory);

            
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

         
            ConfigureBeerFactory(loggerFactory);

            var msLogger = loggerFactory.CreateLogger("HubConnectionHelper");
            HubConnectionHelper.Logger = msLogger;

            //var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("ApplicationConfig.RaspberryPI");
            //var balls = resourceLoader.GetString("BFHub_Credentials_Password");

            Log.Information("Initialization complete.");
        }

        private void ConfigureBeerFactory(ILoggerFactory loggerFactory) {

            Container.RegisterInstance<IApplicationConfig>(ApplicationConfiguration.FromResourceLoader(), new ContainerControlledLifetimeManager());

            Container.RegisterType<IBeerFactoryEventHandler, SignalRPrismBeerFactoryEventHandler>(new ContainerControlledLifetimeManager());
            var eventHandler = Container.Resolve<IBeerFactoryEventHandler>();

            if (DeviceHelper.GetDevice() == Device.RaspberryPi)
                Container.RegisterType<ITemperatureControllerService, SerialUsbArduinoTemperatureControllerService>(new ContainerControlledLifetimeManager());
            else
                Container.RegisterType<ITemperatureControllerService, FakeArduinoTemperatureControllerService>(new ContainerControlledLifetimeManager());


            foreach (var componentId in ComponentHelper.AllComponentIds) {
                Container.RegisterType<Thermometer>($"{componentId}",
                                                    new ContainerControlledLifetimeManager(),
                                                    new InjectionConstructor(new object[] { componentId, eventHandler, loggerFactory }));
                eventHandler.ComponentStateChangeFiring(new ComponentStateChange<ThermometerState> {
                    CurrentState = new ThermometerState {
                        Id = componentId,
                        Temperature = 0
                    }
                });
            }

            foreach (var componentId in ComponentHelper.SsrComponentIds) {
                Container.RegisterType<Ssr>($"{componentId}",
                                            new ContainerControlledLifetimeManager(),
                                            new InjectionConstructor(new object[] { componentId, eventHandler, loggerFactory }));
                eventHandler.ComponentStateChangeFiring(new ComponentStateChange<SsrState> {
                    CurrentState = new SsrState {
                        Id = componentId,
                        Percentage = 0,
                        IsFiring = false,
                        IsEngaged = false
                    }
                }); 
            }

            foreach (var componentId in ComponentHelper.PidComponentIds) {
                Container.RegisterType<PidController>($"{componentId}",
                                                      new ContainerControlledLifetimeManager(),
                                                      new InjectionConstructor(new object[] { componentId, eventHandler, loggerFactory }));

                // TODO: We need to see the PID parameters somehow.
            }

            foreach (var componentId in ComponentHelper.PumpComponentIds) {
                Container.RegisterType<Pump>($"{componentId}",
                                             new ContainerControlledLifetimeManager(),
                                             new InjectionConstructor(new object[] { componentId, eventHandler, loggerFactory }));
                eventHandler.ComponentStateChangeFiring(new ComponentStateChange<PumpState> {
                    CurrentState = new PumpState {
                        Id = componentId,
                        IsEngaged = false
                    }
                });
            }

            Container.RegisterType<BeerFactory>(new ContainerControlledLifetimeManager());

            HubConnectionHelper.Environment = "RaspberryPi";

            //******************************** START ***********************************
            
            // Forcing creation of each instance
            Container.ResolveAll<Thermometer>();
            Container.ResolveAll<Ssr>();
            Container.ResolveAll<PidController>();
            Container.Resolve<BeerFactory>();

            var temperatureControllerService = Container.Resolve<ITemperatureControllerService>();

            Task.Run(() => {
                temperatureControllerService.Run();
            });

            // NOTE: Hardcoding to start the HLT PID
            eventHandler.ComponentStateRequestFiring(new ComponentStateRequest<PidControllerRequestState> {
                RequestState = new PidControllerRequestState {
                    Id = ComponentId.HLT,
                    PidMode = PidMode.Temperature,
                    SetPoint = 90,
                    IsEngaged = false,
                    GainProportional = 18,
                    GainIntegral = 1.5,
                    GainDerivative = 22.5
                }
            });

            eventHandler.ComponentStateRequestFiring(new ComponentStateRequest<PidControllerRequestState> {
                RequestState = new PidControllerRequestState {
                    Id = ComponentId.MT,
                    PidMode = PidMode.Temperature,
                    SetPoint = 80,
                    IsEngaged = false,
                    GainProportional = 18,
                    GainIntegral = 1.5,
                    GainDerivative = 22.5
                }
            });

            eventHandler.ComponentStateRequestFiring(new ComponentStateRequest<PidControllerRequestState> {
                RequestState = new PidControllerRequestState {
                    Id = ComponentId.BK,
                    PidMode = PidMode.Percentage,
                    SetPoint = 70,
                    IsEngaged = false,
                    GainProportional = 18,
                    GainIntegral = 1.5,
                    GainDerivative = 22.5
                }
            });
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args) {
            await LaunchApplicationAsync(PageTokens.MainPage, null);
        }

        private async Task LaunchApplicationAsync(string page, object launchParam) {
            NavigationService.Navigate(page, launchParam);
            Window.Current.Activate();
            await Task.CompletedTask;
        }

        protected override async Task OnActivateApplicationAsync(IActivatedEventArgs args) {
            await Task.CompletedTask;
        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args) {
            await base.OnInitializeAsync(args);

            // We are remapping the default ViewNamePage and ViewNamePageViewModel naming to ViewNamePage and ViewNameViewModel to
            // gain better code reuse with other frameworks and pages within Windows Template Studio
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "BF.Appliance.ViewModels.{0}ViewModel, BF.Appliance", viewType.Name.Substring(0, viewType.Name.Length - 4));
                return Type.GetType(viewModelTypeName);
            });
        }

        protected override IDeviceGestureService OnCreateDeviceGestureService() {
            var service = base.OnCreateDeviceGestureService();
            service.UseTitleBarBackButton = false;
            return service;
        }
    }
}
