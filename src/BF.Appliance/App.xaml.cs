using System;
using System.Globalization;
using System.Threading.Tasks;
using BF.Service;
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
using BF.Service.Prism.Events;
using BF.Services.Prism.Events;
using Microsoft.Extensions.Logging;
using Serilog.Exceptions;
using BF.Common.Components;

namespace BF.Appliance {
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication {
        public App() {
            InitializeComponent();
        }

        protected override void ConfigureContainer() {
            // register a singleton using Container.RegisterType<IInterface, Type>(new ContainerControlledLifetimeManager());
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


            Log.Logger = loggerConfiguration.CreateLogger();

            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);

            Container.RegisterInstance<ILoggerFactory>(loggerFactory);

            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

            Container.RegisterType<IBeerFactoryEventHandler, SignalRPrismBeerFactoryEventHandler>(new ContainerControlledLifetimeManager());

            if (DeviceHelper.GetDevice() == Device.RaspberryPi)
                Container.RegisterType<ITemperatureControllerService, SerialUsbArduinoTemperatureControllerService>(new ContainerControlledLifetimeManager());
           else
                Container.RegisterType<ITemperatureControllerService, FakeArduinoTemperatureControllerService>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IBeerFactory, BeerFactory>(new ContainerControlledLifetimeManager());

            var temperatureControllerService = Container.Resolve<ITemperatureControllerService>();
            var beerFactory = Container.Resolve<IBeerFactory>();

            Task.Run(() => {
                temperatureControllerService.Run();
            });

            HubConnectionHelper.Environment = "RaspberryPi";
            var msLogger = loggerFactory.CreateLogger("HubConnectionHelper");
            HubConnectionHelper.Logger = msLogger;

            Log.Information("Initialization complete.");
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
