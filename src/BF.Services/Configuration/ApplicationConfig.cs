﻿using BF.Common.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Services.Configuration {


    public interface IApplicationConfig {

        string SignalRUrl { get; }

        string UserName { get; }

        string Password { get; }

        bool IsLocal { get; }

        Device Device { get; set; }

        bool IsServer();

        bool IsAppliance();

    }

    public class ApplicationConfig : IApplicationConfig {

        public string SignalRUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsLocal { get; set;}

        public Device Device { get; set; }

        public bool IsServer() => Device == Device.Server_PC || Device == Device.Server;

        public bool IsAppliance() => Device == Device.RaspberryPi_PC || Device == Device.RaspberryPi;

    }

    public static class ApplicationConfiguration {

        public static ApplicationConfig FromIConfiguration(this IConfiguration configuration) {
            return new ApplicationConfig {
                SignalRUrl = configuration["BFHub:Url"],
                UserName = configuration["BFHub:Credentials:UserName"],
                Password = configuration["BFHub:Credentials:Password"],
                IsLocal = bool.Parse(configuration["BFHub:Local"]),
                Device = DeviceHelper.GetDevice()
            };
        }

        public static ApplicationConfig FromBullshit() {
            return new ApplicationConfig {
                SignalRUrl = "https://localhost:44355/bfHub",
                UserName = "raspberrypi",
                Password = "BlaBlah",
                IsLocal = true,
                Device = DeviceHelper.GetDevice()
            };
        }

        public static ApplicationConfig FromResourceLoader() {
            var resourceLoader = (DeviceHelper.GetDevice() == Device.RaspberryPi) ?
                Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("ApplicationConfig.RaspberryPI") :
                Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("ApplicationConfig.RaspberryPI-PC");

            return new ApplicationConfig {
                SignalRUrl = resourceLoader.GetString("BFHub_Url"),
                UserName = resourceLoader.GetString("BFHub_Credentials_UserName"),
                Password = resourceLoader.GetString("BFHub_Credentials_Password"),
                IsLocal = bool.Parse(resourceLoader.GetString("BFHub_Local")),
                Device = DeviceHelper.GetDevice()
            };
        }
    }
}
