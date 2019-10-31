using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class PidControllerState : UpdateableComponentState {

        public PidMode PidMode { get; set; }

        public double SetPoint { get; set; }

        public double Temperature { get; set; }

        public double GainProportional { get; set; }

        public double GainIntegral { get; set; }

        public double GainDerivative { get; set; }

    }

    public static class PidControllerStateHelper {

        public static PidControllerState Clone(this PidControllerState pidControllerState) {
            return new PidControllerState {
                PidMode = pidControllerState.PidMode,
                SetPoint = pidControllerState.SetPoint,
                Temperature = pidControllerState.Temperature,
                GainProportional = pidControllerState.GainProportional,
                GainIntegral = pidControllerState.GainIntegral,
                GainDerivative = pidControllerState.GainDerivative
            };
        }

        public static PidControllerState Update(this PidControllerState pidControllerState, double newTemperature) {
            var clone = pidControllerState.Clone();
            clone.Temperature = newTemperature;
            return clone;
        }

        public static PidControllerState Update(this PidControllerState pidControllerState, PidControllerState newPidControllerState) {
            var clone = pidControllerState.Clone();

            clone.IsEngaged = newPidControllerState.IsEngaged;
            clone.PidMode = (newPidControllerState.PidMode != PidMode.Unknown) ?
                newPidControllerState.PidMode :
                clone.PidMode;

            clone.SetPoint = newPidControllerState.SetPoint;

            clone.GainDerivative = newPidControllerState.GainDerivative != double.MinValue ?
                newPidControllerState.GainDerivative :
                clone.GainDerivative;

            clone.GainIntegral = newPidControllerState.GainIntegral != double.MinValue ?
                newPidControllerState.GainIntegral :
                clone.GainIntegral;

            clone.GainProportional = newPidControllerState.GainProportional != double.MinValue ?
                newPidControllerState.GainProportional :
                clone.GainProportional;

            return clone;
        }
    }
}
