﻿using BF.Common.Components;
using BF.Common.Events;
using BF.Common.Ids;
using BF.Service.Components;
using BF.Service.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BF.Service.Components {

    /// <summary>
    /// A (P)roportional, (I)ntegral, (D)erivative Controller
    /// </summary>
    /// <remarks>
    /// The controller should be able to control any process with a
    /// measureable value, a known ideal value and an input to the
    /// process that will affect the measured value.
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/PID_controller"/>
    public class PidController : IComponent {

        private ILogger Logger { get; set; }

        public ComponentId Id { get; private set; }

        public Thermometer Thermometer { get; private set; }

        public Ssr Ssr { get; private set; }

        private double processVariable = 0;
        private DateTime lastRun;
        private bool isRunning = false;
        private IBeerFactoryEventHandler _eventHandler;

        private int dutyCycleInMillis = 2000;

        public PidController(IBeerFactoryEventHandler eventHandler, ComponentId id, Ssr ssr, Thermometer thermometer, ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<PidController>();
            _eventHandler = eventHandler;
            Id = id;
            Ssr = ssr;
            Thermometer = thermometer;
            RegisterEvents();
        }

        public PidController(IBeerFactoryEventHandler eventHandler, ComponentId id, Ssr ssr, Thermometer thermometer, double setPoint, ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<PidController>();
            _eventHandler = eventHandler;
            Id = id;
            Ssr = ssr;
            Thermometer = thermometer;
            SetPoint = setPoint;
            RegisterEvents();
        }

        public PidController(IBeerFactoryEventHandler eventHandler, ComponentId id, Ssr ssr, Thermometer thermometer, double gainProportional, double gainIntegral, double gainDerivative, double outputMin, double outputMax, double setPoint, ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<PidController>();
            _eventHandler = eventHandler;
            if (OutputMax < OutputMin)
                throw new FormatException("OutputMax is less than OutputMin");
            Id = id;
            Ssr = ssr;
            Thermometer = thermometer;
            GainDerivative = gainDerivative;
            GainIntegral = gainIntegral;
            GainProportional = gainProportional;
            OutputMax = outputMax;
            OutputMin = outputMin;
            SetPoint = setPoint;
            RegisterEvents();
        }

        private void RegisterEvents() {
            _eventHandler.TemperatureChangeOccured(TemperatureChangeOccured);
            _eventHandler.PidRequestOccured(PidRequestOccured);
        }

        private bool isEngaged = false;

        public bool IsEngaged {
            get { return isEngaged; }
            set { isEngaged = value; }
        }


        public void TemperatureChangeOccured(TemperatureChange temperatureChange) {
            if (temperatureChange.Id == Thermometer.Id) {
                Process();
            }
        }

        public void PidRequestOccured(PidRequest pidRequest) {
            if (pidRequest.Id == Id) {
                IsEngaged = pidRequest.IsEngaged.HasValue ? pidRequest.IsEngaged.Value : IsEngaged;
                PidMode = pidRequest.PidMode.HasValue ? pidRequest.PidMode.Value : PidMode;
                SetPoint = pidRequest.SetPoint.HasValue ? pidRequest.SetPoint.Value : SetPoint;

                GainDerivative = pidRequest.GainDerivative.HasValue ? pidRequest.GainDerivative.Value : GainDerivative;
                GainIntegral = pidRequest.GainIntegral.HasValue ? pidRequest.GainIntegral.Value : GainIntegral;
                GainProportional = pidRequest.GainProportional.HasValue ? pidRequest.GainProportional.Value : GainProportional;

                _eventHandler.PidChangeFired(new PidChange {
                    Id = Id,
                    IsEngaged = IsEngaged,
                    PidMode = PidMode,
                    SetPoint = SetPoint,
                    GainDerivative = GainDerivative,
                    GainIntegral = GainIntegral,
                    GainProportional = GainProportional
                });
            }

            //if (pidRequest.Id != Id && pidRequest.IsEngaged) {
                // Disengage all other PIDs 
            //    IsEngaged = false;
            //}

            Process();
        }

        /// <summary>
        /// The controller output
        /// </summary>
        /// <param name="timeSinceLastUpdate">timespan of the elapsed time
        /// since the previous time that ControlVariable was called</param>
        /// <returns>Value of the variable that needs to be controlled</returns>
        public void Process() {

            ProcessVariable = (double)Thermometer.Temperature;

            if (!isEngaged)
                Ssr.Percentage = 0;

            if (isEngaged) {
                if (PidMode == PidMode.Temperature && ProcessVariable != 0) {
                    var currentTime = DateTime.Now;
                    if (lastRun == null)
                        lastRun = currentTime;


                    var secondsSinceLastUpdate = (currentTime - lastRun).Seconds;
                    if (secondsSinceLastUpdate == 0) secondsSinceLastUpdate = 1;

                    double error = SetPoint - ProcessVariable;

                    // integral term calculation
                    IntegralTerm += (GainIntegral * error * secondsSinceLastUpdate);
                    IntegralTerm = Clamp(IntegralTerm);

                    // derivative term calculation
                    double dInput = processVariable - ProcessVariableLast;
                    double derivativeTerm = GainDerivative * (dInput / secondsSinceLastUpdate);

                    // proportional term calcullation
                    double proportionalTerm = GainProportional * error;

                    double output = proportionalTerm + IntegralTerm - derivativeTerm;

                    output = Clamp(output);

                    lastRun = currentTime;

                    Logger.LogInformation($"PID Temp: {ProcessVariable}, SSR: {output}, SetPoint: {SetPoint}");

                    //Debug.WriteLine($"Temperature: {ProcessVariable}  SSR: {output}");

                    Ssr.Percentage = (int)output;

                } else if (PidMode == PidMode.Percentage) {
                    // If PidMode is Percentage, SetPoint is a Percentage
                    Ssr.Percentage = (int)SetPoint;
                }
            }
        }



        /// <summary>
        /// The derivative term is proportional to the rate of
        /// change of the error
        /// </summary>
        public double GainDerivative { get; set; } = 1;

        /// <summary>
        /// The integral term is proportional to both the magnitude
        /// of the error and the duration of the error
        /// </summary>
        public double GainIntegral { get; set; } = 1;

        /// <summary>
        /// The proportional term produces an output value that
        /// is proportional to the current error value
        /// </summary>
        /// <remarks>
        /// Tuning theory and industrial practice indicate that the
        /// proportional term should contribute the bulk of the output change.
        /// </remarks>
        public double GainProportional { get; set; } = 1;

        /// <summary>
        /// The max output value the control device can accept.
        /// </summary>
        public double OutputMax { get; private set; } = 100;

        /// <summary>
        /// The minimum ouput value the control device can accept.
        /// </summary>
        public double OutputMin { get; private set; } = 0;

        /// <summary>
        /// Adjustment made by considering the accumulated error over time
        /// </summary>
        /// <remarks>
        /// An alternative formulation of the integral action, is the
        /// proportional-summation-difference used in discrete-time systems
        /// </remarks>
        public double IntegralTerm { get; private set; } = 0;


        /// <summary>
        /// The current value
        /// </summary>
        public double ProcessVariable {
            get { return processVariable; }
            set {
                ProcessVariableLast = processVariable;
                processVariable = value;
            }
        }

        /// <summary>
        /// The last reported value (used to calculate the rate of change)
        /// </summary>
        public double ProcessVariableLast { get; private set; } = 0;

        /// <summary>
        /// The desired value
        /// </summary>
        public double SetPoint { get; set; } = 0;

        public PidMode PidMode { get; set; } = PidMode.Temperature;

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
            if (variableToClamp <= OutputMin) { return OutputMin; }
            if (variableToClamp >= OutputMax) { return OutputMax; }
            return variableToClamp;
        }
    }

  
}
