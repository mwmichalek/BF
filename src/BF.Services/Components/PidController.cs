using BF.Common.Components;
using BF.Common.Events;
using BF.Common.Ids;
using BF.Common.States;
using BF.Service.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BF.Services.Components {

    /// <summary>
    /// A (P)roportional, (I)ntegral, (D)erivative Controller
    /// </summary>
    /// <remarks>
    /// The controller should be able to control any process with a
    /// measureable value, a known ideal value and an input to the
    /// process that will affect the measured value.
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/PID_controller"/>
    public class PidController : ComponentBase<PidControllerState> {

        private ILogger Logger { get; set; }

        private DateTime lastRun;

        private bool isRunning = false;

        private IBeerFactoryEventHandler _eventHandler;

        private int dutyCycleInMillis = 2000;


        public PidController(ComponentId id, 
                             IBeerFactoryEventHandler eventHandler, 
                             ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<PidController>();
            _eventHandler = eventHandler;
            CurrentState = new PidControllerState { Id = id };
            RegisterEvents();
        }

        private void RegisterEvents() {
            _eventHandler.ComponentStateChangeOccured<ThermometerState>(ThermometerStateChangeOccured);
            _eventHandler.ComponentStateRequestOccured<PidControllerState>(PidControllerStateRequestOccured);
        }

        private void ThermometerStateChangeOccured(ComponentStateChange<ThermometerState> thermometerStateChange) { 
            if (thermometerStateChange.Id == Id) {
                PriorState = CurrentState;
                CurrentState = CurrentState.Update(thermometerStateChange.CurrentState.Temperature);
                Process();
            }
        }

        private void PidControllerStateRequestOccured(ComponentStateRequest<PidControllerState> pidControllerStateRequest) {
            if (pidControllerStateRequest.Id == Id) {
                Logger.LogInformation($"Pid Request Received: {Id} {pidControllerStateRequest.RequestState.SetPoint} {pidControllerStateRequest.RequestState.IsEngaged}");
                PriorState = CurrentState;
                CurrentState = CurrentState.UpdateRequest(pidControllerStateRequest.RequestState);

                _eventHandler.ComponentStateChangeFiring<PidControllerState>(new ComponentStateChange<PidControllerState> {
                    PriorState = PriorState.Clone(),
                    CurrentState = CurrentState.Clone()
                });

                Process();
            }
        }

        /// <summary>
        /// The controller output
        /// </summary>
        /// <param name="timeSinceLastUpdate">timespan of the elapsed time
        /// since the previous time that ControlVariable was called</param>
        /// <returns>Value of the variable that needs to be controlled</returns>
        private void Process() {

            if (!CurrentState.IsEngaged)
                UpdateSsr(0, "Pid Disengaged");

            if (CurrentState.IsEngaged) {
                if (CurrentState.PidMode == PidMode.Temperature && CurrentState.Temperature != 0) {
                    var currentTime = DateTime.Now;
                    if (lastRun == null)
                        lastRun = currentTime;


                    var secondsSinceLastUpdate = (currentTime - lastRun).Seconds;
                    if (secondsSinceLastUpdate == 0) secondsSinceLastUpdate = 1;

                    double error = CurrentState.SetPoint - CurrentState.Temperature;

                    // integral term calculation
                    _integralTerm += (CurrentState.GainIntegral * error * secondsSinceLastUpdate);
                    _integralTerm = Clamp(_integralTerm);

                    // derivative term calculation
                    double dInput = CurrentState.Temperature - PriorState.Temperature;
                    double derivativeTerm = CurrentState.GainDerivative * (dInput / secondsSinceLastUpdate);

                    // proportional term calcullation
                    double proportionalTerm = CurrentState.GainProportional * error;

                    double output = proportionalTerm + _integralTerm - derivativeTerm;
                    output = Clamp(output);

                    lastRun = currentTime;

                    Logger.LogInformation($"PID Temp: {CurrentState.Temperature}, SSR: {output}, SetPoint: {CurrentState.SetPoint}");


                    UpdateSsr((int)output);

                } else if (CurrentState.PidMode == PidMode.Percentage) {
                    // If PidMode is Percentage, SetPoint is a Percentage
                    UpdateSsr((int)CurrentState.SetPoint, "Percentage");
                }
            }
        }

        private void UpdateSsr(int percentage, string msg = "") {
            Logger.LogInformation($"Ssr Request Firing: {Id} {percentage} {msg}");
            _eventHandler.ComponentStateRequestFiring<SsrState>(new ComponentStateRequest<SsrState> {
                RequestState = new SsrState {
                    Id = Id,
                    Percentage = percentage
                }
            });
        }


        /// <summary>
        /// The max output value the control device can accept.
        /// </summary>
        private double _outputMax = 100;

        /// <summary>
        /// The minimum ouput value the control device can accept.
        /// </summary>
        private double _outputMin = 0;

        /// <summary>
        /// Adjustment made by considering the accumulated error over time
        /// </summary>
        /// <remarks>
        /// An alternative formulation of the integral action, is the
        /// proportional-summation-difference used in discrete-time systems
        /// </remarks>
        private double _integralTerm = 0;

        /// <summary>
        /// Limit a variable to the set OutputMax and OutputMin properties
        /// </summary>
        /// <returns>
        /// A value that is between the OutputMax and OutputMin properties
        /// </returns>
        /// <remarks>
        /// Inspiration from http://stackoverflow.com/questions/3176602/how-to-force-a-number-to-be-in-a-range-in-c
        /// </remarks>
        private double Clamp(double variableToClamp) {
            if (variableToClamp <= _outputMin) { return _outputMin; }
            if (variableToClamp >= _outputMax) { return _outputMax; }
            return variableToClamp;
        }
    }

    

}
