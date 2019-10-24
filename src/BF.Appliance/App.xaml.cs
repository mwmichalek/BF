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

namespace BF.Appliance {
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App : PrismUnityApplication {
        public App() {
            InitializeComponent();
        }

        protected override void ConfigureContainer() {
            // register a singleton using Container.RegisterType<IInterface, Type>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();


            //Container.RegisterType<IBackgroundTaskService, BackgroundTaskService>(new ContainerControlledLifetimeManager());
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

            Container.RegisterType<IBeerFactoryEventHandler, SignalRPrismBeerFactoryEventHandler>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<ITemperatureControllerService, SerialUsbArduinoTemperatureControllerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITemperatureControllerService, FakeArduinoTemperatureControllerService>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IBeerFactory, BeerFactory>(new ContainerControlledLifetimeManager());

            var temperatureControllerService = Container.Resolve<ITemperatureControllerService>();
            var beerFactory = Container.Resolve<IBeerFactory>();

            Task.Run(() => {
                temperatureControllerService.Run();
            });



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
