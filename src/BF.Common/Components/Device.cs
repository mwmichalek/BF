using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BF.Common.Components {
    public enum Device {
        RaspberryPi,
        RaspberryPi_PC,
        Server,
        Server_PC,
        AndriodPhone,
        AndriodTablet,
        Kindle,
        XBox,
        Unknown
    }

    public static class DeviceHelper {

        private static Device? _device = Device.Unknown;

        public static Device GetDevice() {
            if (!_device.HasValue) {
                var assemblyName = Assembly.GetEntryAssembly().GetName();
                var processorArchitecture = assemblyName.ProcessorArchitecture;

                if (assemblyName.Name == "BF.Server") {
                    if (processorArchitecture == ProcessorArchitecture.X86) 
                        _device = Device.Server;
                    else 
                        _device = Device.Server_PC;

                } else if (assemblyName.Name == "BF.Appliance") {
                    if (processorArchitecture == ProcessorArchitecture.Arm)
                        _device = Device.RaspberryPi;
                    else 
                        _device = Device.RaspberryPi_PC;
                } else
                    _device = Device.Unknown;
            }

            return _device.Value;
        }

    }
}
