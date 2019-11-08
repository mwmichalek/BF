using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class PidControllerState : UpdateableComponentState {

        public PidMode PidMode { get; set; } = PidMode.Temperature;

        public double SetPoint { get; set; } = double.MinValue;

        public double Temperature { get; set; } = double.MinValue;

        public double GainProportional { get; set; } = 1;

        public double GainIntegral { get; set; } = 1;

        public double GainDerivative { get; set; } = 1;

    }

    public static class PidControllerStateHelper {

        public static PidControllerState Clone(this PidControllerState pidControllerState) {
            if (pidControllerState == null) return null;
            return new PidControllerState {
                IsEngaged = pidControllerState.IsEngaged,
                PidMode = pidControllerState.PidMode,
                SetPoint = pidControllerState.SetPoint,
                Temperature = pidControllerState.Temperature,
                GainProportional = pidControllerState.GainProportional,
                GainIntegral = pidControllerState.GainIntegral,
                GainDerivative = pidControllerState.GainDerivative,
                Timestamp = pidControllerState.Timestamp
            };
        }

        public static PidControllerState Update(this PidControllerState pidControllerState, double newTemperature) {
            if (pidControllerState == null) return null;
            var clone = pidControllerState.Clone();
            clone.Temperature = newTemperature;
            clone.Timestamp = DateTime.Now;
            return clone;
        }

        public static PidControllerState UpdateRequest(this PidControllerState pidControllerState, PidControllerState requestPidControllerState) {
            if (pidControllerState == null) return null;
            var clone = pidControllerState.Clone();

            clone.IsEngaged = requestPidControllerState.IsEngaged;

            clone.PidMode = (requestPidControllerState.PidMode != PidMode.Unknown) ?
                requestPidControllerState.PidMode :
                clone.PidMode;

            clone.SetPoint = requestPidControllerState.SetPoint;

            clone.GainDerivative = requestPidControllerState.GainDerivative != double.MinValue ?
                requestPidControllerState.GainDerivative :
                clone.GainDerivative;

            clone.GainIntegral = requestPidControllerState.GainIntegral != double.MinValue ?
                requestPidControllerState.GainIntegral :
                clone.GainIntegral;

            clone.GainProportional = requestPidControllerState.GainProportional != double.MinValue ?
                requestPidControllerState.GainProportional :
                clone.GainProportional;

            clone.Timestamp = DateTime.Now;

            return clone;
        }
    }

    public enum PidMode {
        Temperature,
        Percentage,
        Unknown
    }
}
