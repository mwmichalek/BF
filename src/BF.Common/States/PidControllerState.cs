using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class PidControllerState : ConfigurableComponentState {

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
                Id = pidControllerState.Id,
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

        public static PidControllerRequestState ToPidControllerRequestState(this ComponentStateChange<ThermometerState> thermometerStateChange) {
            return new PidControllerRequestState { Temperature = thermometerStateChange.CurrentState.Temperature };
        }

        public static PidControllerState Update(this PidControllerState pidControllerState, PidControllerRequestState requestPidControllerState) {
            if (pidControllerState == null) return null;
            var clone = pidControllerState.Clone();

            if (requestPidControllerState.IsEngaged.HasValue)
                clone.IsEngaged = requestPidControllerState.IsEngaged.Value;

            if (requestPidControllerState.PidMode.HasValue)
                clone.PidMode = requestPidControllerState.PidMode.Value;

            if (requestPidControllerState.SetPoint.HasValue)
                clone.SetPoint = requestPidControllerState.SetPoint.Value;

            // NOTE: Temperature can't be set, dawg.

            if (requestPidControllerState.GainDerivative.HasValue)
                clone.GainDerivative = requestPidControllerState.GainDerivative.Value;

            if (requestPidControllerState.GainIntegral.HasValue)
                clone.GainIntegral = requestPidControllerState.GainIntegral.Value;

            if (requestPidControllerState.GainIntegral.HasValue)
                clone.GainProportional = requestPidControllerState.GainProportional.Value;

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
