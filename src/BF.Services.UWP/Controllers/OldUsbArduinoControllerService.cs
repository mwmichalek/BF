using CommandMessenger;
using CommandMessenger.Transport.Serial;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;
using Windows.Storage.Streams;
using Windows.UI.Core;
using SerialPortLib;
using BF.Service.Controllers;

namespace BF.Service.UWP.Controllers {
    public class OldUsbArduinoControllerService : TemperatureControllerService {
        enum Command {
            Acknowledge,
            Error,
            PingRequest,
            PingResult,
            StatusRequest,
            StatusResult,
            KettleRequest,
            KettleResult,
            TempChange,
            SsrChange,
            HeaterChange,
            PumpChange,
            Message
        };

        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        public bool IsConnected { get; set; }
        public OldUsbArduinoControllerService() {


            //_eventAggregator.GetEvent<KettleCommandEvent>().Subscribe((kettleCommand) => {
            //    ExecuteKettleCommand(kettleCommand);
            //});
        }

        public override async Task Run() {

            while (true) {
                var setupResult = await Setup();
                if (setupResult) {
                    while (IsConnected) {
                        Ping();
                        await Task.Delay(1000);
                    }
                    Exit();

                    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    //    _eventAggregator.GetEvent<ConnectionStatusEvent>().Publish(new ConnectionStatus { Type = ConnectionStatus.EventType.Disconnected });
                    //});
                } else {
                    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    //    _eventAggregator.GetEvent<ConnectionStatusEvent>().Publish(new ConnectionStatus { Type = ConnectionStatus.EventType.NotConnected });
                    //});
                }
                await Task.Delay(1000);
            }
        }

        public async Task<bool> Setup() {




            var serialPort = new SerialPortInput();

            // Listen to Serial Port events

            serialPort.ConnectionStatusChanged += delegate (object sender, ConnectionStatusChangedEventArgs args) {
                Debug.WriteLine("Connected = {0}", args.Connected);
            };

            serialPort.MessageReceived += delegate (object sender, MessageReceivedEventArgs args) {
                var msg = Encoding.UTF8.GetString(args.Data, 0, args.Data.Length);
                Debug.WriteLine($"Received message: {msg}");
            };

            serialPort.SetPort("COM4", 57600);

            // Connect the serial port
            serialPort.Connect();

            UInt32 _pid = 0x0042;
            UInt32 _vid = 0x2341;


            ////USB\VID_2341&PID_0042&REV_0001
            ////USB\VID_2341&PID_0042

            var aqs = UsbDevice.GetDeviceSelector(_vid, _pid);

            //string aqs = SerialDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);
            var myDeviceInfo = dis[0];

            ////SerialPort serialPort = null;

            //foreach (var di in dis) {
            //    if (di.Id.Contains("VID_2341")) {

            //        try {
            //            var serialPort = await SerialDevice.FromIdAsync(di.Id);

            //            if (serialPort != null)
            //                Debug.WriteLine($"serialPort: {di.Id} {serialPort.PortName}");
            //            else
            //                Debug.WriteLine($"serialPort: -");
            //        } catch (Exception ex) {

            //            Debug.WriteLine(ex);
            //        }



            //        //Debug.WriteLine($"Device: {device.Properties["FriendlyName"]}");
            //    }

            //}

            //_serialTransport = new SerialTransport {
            //    CurrentSerialSettings = { PortName = $"COM4", BaudRate = 57600, DtrEnable = false } // object initializer
            //};

            //_cmdMessenger = new CmdMessenger(_serialTransport, BoardType.Bit32);

            //_cmdMessenger.Attach(OnUnknownCommand);
            //_cmdMessenger.Attach((int)Command.Acknowledge, OnAcknowledge);
            //_cmdMessenger.Attach((int)Command.Error, OnError);
            //_cmdMessenger.Attach((int)Command.TempChange, OnTempChange);
            //_cmdMessenger.Attach((int)Command.KettleResult, OnKettleResult);
            //_cmdMessenger.Attach((int)Command.SsrChange, OnSsrChange);
            //_cmdMessenger.Attach((int)Command.Message, OnMessage);
            //// _cmdMessenger.Attach((int)Command.HeaterChange, OnHeaterChange);

            //_cmdMessenger.NewLineReceived += NewLineReceived;
            //_cmdMessenger.NewLineSent += NewLineSent;

            ////IsConnected = _cmdMessenger.Connect();




            //if (IsConnected) {
            //    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
            //        _eventAggregator.GetEvent<ConnectionStatusEvent>().Publish(new ConnectionStatus { Type = ConnectionStatus.EventType.Connected });
            //    });

            //    RequestStatus();
            //}

            return IsConnected;
        }

        public void Ping() {
            var pingCommand = new SendCommand((int)Command.PingRequest, (int)Command.PingResult, 2000);
            var pingResultCommand = _cmdMessenger.SendCommand(pingCommand);
            IsConnected = pingResultCommand.Ok;
        }

        public void Exit() {
            _cmdMessenger.Disconnect();
            _cmdMessenger.Dispose();
            _serialTransport.Dispose();
        }

        public void RequestStatus() {
            var statusCommand = new SendCommand((int)Command.StatusRequest, (int)Command.StatusResult, 2000);
            var statusResultCommand = _cmdMessenger.SendCommand(statusCommand);
            var success = statusResultCommand.Ok;
        }

        //public void ExecuteKettleCommand(KettleCommand kettleCommand) {
        //    Debug.WriteLine($"ExecuteKettleCommand: {kettleCommand.Index} {kettleCommand.Percentage}");

        //    Task.Run(() => {
        //        var kettleRequest = new SendCommand((int)Command.KettleRequest, (int)Command.KettleResult, 2000);
        //        kettleRequest.AddArgument(kettleCommand.Index);
        //        kettleRequest.AddArgument(kettleCommand.Percentage);
        //        var kettleResultCommand = _cmdMessenger.SendCommand(kettleRequest);
        //        var success = kettleRequest.Ok;
        //    });

        //}

        // ------------------  C A L L B A C K S ---------------------

        //private async void OnSsrChange(ReceivedCommand receivedCommand) {
        //    int.TryParse(receivedCommand.ReadStringArg(), out int ssrIndex);
        //    int.TryParse(receivedCommand.ReadStringArg(), out int ssrValue);
        //    int.TryParse(receivedCommand.ReadStringArg(), out int percentage);

        //    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //    //    _eventAggregator.GetEvent<SsrChangeEvent>().Publish(new SsrChange { Index = ssrIndex, IsEngaged = ssrValue == 1, Percentage = percentage });
        //    //});
        //}

        //private async void OnHeaterChange(ReceivedCommand receivedCommand) {
        //    int.TryParse(receivedCommand.ReadStringArg(), out int heaterIndex);
        //    int.TryParse(receivedCommand.ReadStringArg(), out int heaterValue);

        //    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //        _eventAggregator.GetEvent<HeaterResultEvent>().Publish(new HeaterResult { Index = heaterIndex, IsEngaged = heaterIndex == 1 });
        //    });
        //}

        //private async void OnTempChange(ReceivedCommand receivedCommand) {
        //    int.TryParse(receivedCommand.ReadStringArg(), out int probeIndex);
        //    decimal.TryParse(receivedCommand.ReadStringArg(), out decimal temp);

        //    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //    //    _eventAggregator.GetEvent<TemperatureChangeEvent>().Publish(new TemperatureChange { Index = probeIndex, Value = temp });
        //    //});
        //}

        //private async void OnKettleResult(ReceivedCommand receivedCommand) {
        //    try {
        //        int.TryParse(receivedCommand.ReadStringArg(), out int kettleIndex);
        //        //int.TryParse(receivedCommand.ReadStringArg(), out int percentage);
        //        var percentageString = receivedCommand.ReadStringArg();
        //        int.TryParse(percentageString.Split(':')[1], out int percentage);
        //        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //            _eventAggregator.GetEvent<KettleResultEvent>().Publish(new KettleResult { Index = kettleIndex, Percentage = percentage });
        //        });
        //    } catch (Exception ex) {

        //        throw;
        //    }
        //}

        //private async void OnMessage(ReceivedCommand receivedCommand) {
        //    try {
        //        int.TryParse(receivedCommand.ReadStringArg(), out int kettleIndex);
        //        var message = ""; // receivedCommand.ReadStringArg();
        //        int.TryParse(receivedCommand.ReadStringArg(), out int percentage);

        //        //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //        //    _eventAggregator.GetEvent<MessageEvent>().Publish(new Message { Index = kettleIndex, Body = message, Percentage = percentage });
        //        //});
        //    } catch (Exception ex) {

        //        throw;
        //    }
        //}

        private void OnUnknownCommand(ReceivedCommand arguments) {
            Console.WriteLine("Command without attached callback received");
        }

        //private async void OnAcknowledge(ReceivedCommand arguments) {
        //    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
        //    //    _eventAggregator.GetEvent<ConnectionStatusEvent>().Publish(new ConnectionStatus { Type = ConnectionStatus.EventType.Ready });
        //    //});
        //}

        // Callback function that prints that the Arduino has experienced an error
        private void OnError(ReceivedCommand arguments) {
            Console.WriteLine(" Arduino has experienced an error");
        }

        // Log received line to console
        private void NewLineReceived(object sender, CommandEventArgs e) {
            //Console.WriteLine(@"Received > " + e.Command.CommandString());
            //foreach (var arg in e.Command.Arguments) {
            //    Console.WriteLine($"Arg: {arg}");
            //}
        }

        // Log sent line to console
        private void NewLineSent(object sender, CommandEventArgs e) {
            //Console.WriteLine(@"Sent > " + e.Command.CommandString());
        }

    }
}

