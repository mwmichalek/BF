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
using SerialArduino;
using BF.Service.Events;
using BF.Common.Events;
using BF.Common.Ids;
using BF.Service.Controllers;
using BF.Common.Components;
using Microsoft.Extensions.Logging;
using BF.Common.States;
using BF.Services.Components;

namespace BF.Service.UWP.Controllers {

    public class SerialUsbArduinoTemperatureControllerService : TemperatureControllerService {

        private ILogger Logger { get; set; }

        private string[] _stringSeparators = new string[] { "\r\n" };

        private bool _isConnected { get; set; }

        // Track Read Operation
        private CancellationTokenSource _readCancellationTokenSource;
        private Object _readCancelLock = new Object();

        private DataReader _dataReaderObject = null;

        // Track Write Operation
        private CancellationTokenSource _writeCancellationTokenSource;
        private Object _writeCancelLock = new Object();

        private DataWriter _dataWriterObject = null;

        private IBeerFactoryEventHandler _eventHandler;

        private Dictionary<ComponentId, ThermocoupleState> _thermocoupleStateLookup = new Dictionary<ComponentId, ThermocoupleState>();

        public SerialUsbArduinoTemperatureControllerService(Thermometer[] thermometers, 
                                                            IBeerFactoryEventHandler eventHandler, 
                                                            ILoggerFactory loggerFactory) {
            _eventHandler = eventHandler;
            Logger = loggerFactory.CreateLogger<SerialUsbArduinoTemperatureControllerService>();
        }

        public void StartFakeness() {

            // Set HLT Pid to 90 degrees
            _eventHandler.ComponentStateRequestFiring(new ComponentStateRequest<PidControllerState> {
                Id = ComponentId.HLT,
                RequestState = new PidControllerState {
                    IsEngaged = true,
                    SetPoint = 90,
                    GainProportional = 18,
                    GainIntegral = 1.5,
                    GainDerivative = 22.5
                }
            });

            _eventHandler.ComponentStateRequestFiring<PumpState>(new ComponentStateRequest<PumpState> {
                Id = ComponentId.HLT,
                RequestState = new PumpState { IsEngaged = true }
            });
        }

        public override async Task Run() {

            while (true) {
                try {
                    var setupResult = await Setup();
                    if (setupResult) {

                        //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                        //_eventHandler.ConnectionStatusChangeFired(new ConnectionStatusChange { ConnectionState = ConnectionState.Disconnected });
                        //});

                        await RequestAllTemperatures();

                        StartFakeness();

                        while (_isConnected) {
                            await ProcessTemperatures();
                        }
                    } else {
                        //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                        //_eventHandler.ConnectionStatusChangeFired(new ConnectionStatusChange { ConnectionState = ConnectionState.NotConnected });
                        //});
                    }
                    
                } catch (Exception ex) {
                    Debug.WriteLine(ex);
                }
                await Task.Delay(1000);
            }
        }

        public async Task<bool> Setup() {

            string deviceSelector = Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelectorFromUsbVidPid(
                                                                    ArduinoDevice.Vid, ArduinoDevice.Pid);

            var devicesInformation = await DeviceInformation.FindAllAsync(deviceSelector);

            if (devicesInformation != null && devicesInformation.Count > 0) {

                var deviceInformation = devicesInformation[0];
                var deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);

                // Allow the EventHandlerForDevice to handle device watcher events that relates or effects our device (i.e. device removal, addition, app suspension/resume)
                var entry = new DeviceListEntry(deviceInformation, deviceSelector);

                EventHandlerForDevice.CreateNewEventHandlerForDevice();

                // Get notified when the device was successfully connected to or about to be closed
                //EventHandlerForDevice.Current.OnDeviceConnected = this.OnDeviceConnected;
                //EventHandlerForDevice.Current.OnDeviceClose = this.OnDeviceClosing;

                // It is important that the FromIdAsync call is made on the UI thread because the consent prompt, when present,
                // can only be displayed on the UI thread. Since this method is invoked by the UI, we are already in the UI thread.
                _isConnected = await EventHandlerForDevice.Current.OpenDeviceAsync(entry.DeviceInformation, entry.DeviceSelector);

                if (_isConnected) {
                    EventHandlerForDevice.Current.Device.BaudRate = 57600;
                    EventHandlerForDevice.Current.Device.StopBits = SerialStopBitCount.One;
                    EventHandlerForDevice.Current.Device.DataBits = 8;
                    EventHandlerForDevice.Current.Device.Parity = SerialParity.None;
                    EventHandlerForDevice.Current.Device.Handshake = SerialHandshake.None;
                    EventHandlerForDevice.Current.Device.WriteTimeout = TimeSpan.FromMilliseconds(500);
                    EventHandlerForDevice.Current.Device.ReadTimeout = TimeSpan.FromMilliseconds(500);

                    ResetReadCancellationTokenSource();
                    ResetWriteCancellationTokenSource();
                }
            }

            return _isConnected;
        }

        private async Task ProcessTemperatures() {
            String message = String.Empty;
            try {
                _dataReaderObject = new DataReader(EventHandlerForDevice.Current.Device.InputStream);
                message = await ReadAsync(_readCancellationTokenSource.Token, 200);
                //Debug.WriteLine($"Msg:\n{message}");
                foreach (var tempReading in message.Split(_stringSeparators, StringSplitOptions.RemoveEmptyEntries)) {

                    var tempReadingValues = tempReading.Split('|');

                    if (tempReadingValues.Length == 2) {
                        int.TryParse(tempReadingValues[0], out int index);
                        double.TryParse(tempReadingValues[1], out double temperature);

                        var componentId = (ComponentId)Enum.Parse(typeof(ComponentId), (index).ToString());

                        _eventHandler.ComponentStateChangeFiring(new ComponentStateChange<ThermocoupleState> {
                            Id = componentId,
                            CurrentState = new ThermocoupleState {
                                Temperature = temperature
                            }
                        });
                    }
                }
            } catch (OperationCanceledException /*exception*/) {
                NotifyReadTaskCanceled();
            } catch (Exception exception) {
                Debug.WriteLine(exception.Message.ToString());
            } finally {
                _dataReaderObject.DetachStream();
                _dataReaderObject = null;
            }
        }

        private async Task RequestAllTemperatures() {
            try {
                _dataWriterObject = new DataWriter(EventHandlerForDevice.Current.Device.OutputStream);
                _dataWriterObject.WriteString("temps\r");

                await WriteAsync(_writeCancellationTokenSource.Token);
            } catch (OperationCanceledException /*exception*/) {
                NotifyWriteTaskCanceled();
            } catch (Exception exception) {
                Debug.WriteLine(exception.Message.ToString());
            } finally {
                _dataWriterObject.DetachStream();
                _dataWriterObject = null;
            }
        }


        public void Dispose() {
            if (_readCancellationTokenSource != null) {
                _readCancellationTokenSource.Dispose();
                _readCancellationTokenSource = null;
            }

            if (_writeCancellationTokenSource != null) {
                _writeCancellationTokenSource.Dispose();
                _writeCancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Write to the output stream using a task 
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async Task WriteAsync(CancellationToken cancellationToken) {
            Task<UInt32> storeAsyncTask;

            // Don't start any IO if we canceled the task
            lock (_writeCancelLock) {
                cancellationToken.ThrowIfCancellationRequested();

                // Cancellation Token will be used so we can stop the task operation explicitly
                // The completion function should still be called so that we can properly handle a canceled task
                storeAsyncTask = _dataWriterObject.StoreAsync().AsTask(cancellationToken);
            }

            UInt32 bytesWritten = await storeAsyncTask;
            //rootPage.NotifyUser("Write completed - " + bytesWritten.ToString() + " bytes written", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Read from the input output stream using a task 
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async Task<string> ReadAsync(CancellationToken cancellationToken, uint ReadBufferLength) {
            Task<UInt32> loadAsyncTask;

            // Don't start any IO if we canceled the task
            lock (_readCancelLock) {
                // Cancellation Token will be used so we can stop the task operation explicitly
                // The completion function should still be called so that we can properly handle a canceled task
                cancellationToken.ThrowIfCancellationRequested();

                // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
                _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = _dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);
            }

            // Launch the task and wait until the read timeout expires - the bytes returned is the number of bytes were read
            UInt32 bytesRead = await loadAsyncTask;

            String output = String.Empty;
            if (bytesRead > 0) {
                try {
                    output = _dataReaderObject.ReadString(bytesRead);
                    //byte[] rawdata = new byte[ReadBufferLength];
                    //DataReaderObject.ReadBytes(rawdata);
                    //output = Encoding.Unicode.GetString(rawdata, 0, rawdata.Length);


                } catch (Exception) {
                    
                }
            }

            //rootPage.NotifyUser("Read completed - " + bytesRead.ToString() + " bytes were read", NotifyType.StatusMessage);
            return output;
        }


        /// <summary>
        /// It is important to be able to cancel tasks that may take a while to complete. Cancelling tasks is the only way to stop any pending IO
        /// operations asynchronously. If the Serial Device is closed/deleted while there are pending IOs, the destructor will cancel all pending IO 
        /// operations.
        /// </summary>
        /// 

        private void CancelReadTask() {
            lock (_readCancelLock) {
                if (_readCancellationTokenSource != null) {
                    if (!_readCancellationTokenSource.IsCancellationRequested) {
                        _readCancellationTokenSource.Cancel();

                        // Existing IO already has a local copy of the old cancellation token so this reset won't affect it
                        ResetReadCancellationTokenSource();
                    }
                }
            }
        }

        private void CancelWriteTask() {
            lock (_writeCancelLock) {
                if (_writeCancellationTokenSource != null) {
                    if (!_writeCancellationTokenSource.IsCancellationRequested) {
                        _writeCancellationTokenSource.Cancel();

                        // Existing IO already has a local copy of the old cancellation token so this reset won't affect it
                        ResetWriteCancellationTokenSource();
                    }
                }
            }
        }
        private void CancelAllIoTasks() {
            CancelReadTask();
            CancelWriteTask();
        }

        private void ResetReadCancellationTokenSource() {
            // Create a new cancellation token source so that can cancel all the tokens again
            _readCancellationTokenSource = new CancellationTokenSource();

            // Hook the cancellation callback (called whenever Task.cancel is called)
            _readCancellationTokenSource.Token.Register(() => NotifyReadCancelingTask());
        }

        private void ResetWriteCancellationTokenSource() {
            // Create a new cancellation token source so that can cancel all the tokens again
            _writeCancellationTokenSource = new CancellationTokenSource();

            // Hook the cancellation callback (called whenever Task.cancel is called)
            _writeCancellationTokenSource.Token.Register(() => NotifyWriteCancelingTask());
        }

        /// <summary>
        /// Print a status message saying we are canceling a task and disable all buttons to prevent multiple cancel requests.
        /// <summary>
        private async void NotifyReadCancelingTask() {
            // Setting the dispatcher priority to high allows the UI to handle disabling of all the buttons
            // before any of the IO completion callbacks get a chance to modify the UI; that way this method
            // will never get the opportunity to overwrite UI changes made by IO callbacks
            //await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.High,
            //    new DispatchedHandler(() => {
            //        if (!IsNavigatedAway) {
            //            rootPage.NotifyUser("Canceling Read... Please wait...", NotifyType.StatusMessage);
            //        }
            //    }));
        }

        private async void NotifyWriteCancelingTask() {
            // Setting the dispatcher priority to high allows the UI to handle disabling of all the buttons
            // before any of the IO completion callbacks get a chance to modify the UI; that way this method
            // will never get the opportunity to overwrite UI changes made by IO callbacks
            //await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.High,
            //    new DispatchedHandler(() => {
            //        if (!IsNavigatedAway) {
            //            rootPage.NotifyUser("Canceling Write... Please wait...", NotifyType.StatusMessage);
            //        }
            //    }));
        }


        /// <summary>
        /// Notifies the UI that the operation has been cancelled
        /// </summary>
        private async void NotifyReadTaskCanceled() {
            //await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //    new DispatchedHandler(() => {
            //        if (!IsNavigatedAway) {
            //            rootPage.NotifyUser("Read request has been cancelled", NotifyType.StatusMessage);
            //        }
            //    }));
        }

        private async void NotifyWriteTaskCanceled() {
            //await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //    new DispatchedHandler(() => {
            //        if (!IsNavigatedAway) {
            //            rootPage.NotifyUser("Write request has been cancelled", NotifyType.StatusMessage);
            //        }
            //    }));
        }

    }

}

