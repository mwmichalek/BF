﻿using BF.Common.Components;
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

            // TODO: Add PidControllerState update logic

            //TODO: Move this to the clone method
            //CurrentState.IsEngaged = pidControllerStateRequest.RequestState.IsEngaged;
            //CurrentState.PidMode = (pidControllerStateRequest.RequestState.PidMode != PidMode.Unknown) ?
            //    pidControllerStateRequest.RequestState.PidMode :
            //    CurrentState.PidMode;
            //CurrentState.SetPoint = pidControllerStateRequest.RequestState.SetPoint;

            //CurrentState.GainDerivative = pidControllerStateRequest.RequestState.GainDerivative != double.MinValue ?
            //    pidControllerStateRequest.RequestState.GainDerivative :
            //    CurrentState.GainDerivative;
            //CurrentState.GainIntegral = pidControllerStateRequest.RequestState.GainIntegral != double.MinValue ?
            //    pidControllerStateRequest.RequestState.GainIntegral :
            //    CurrentState.GainIntegral;
            //CurrentState.GainProportional = pidControllerStateRequest.RequestState.GainProportional != double.MinValue ?
            //    pidControllerStateRequest.RequestState.GainProportional :
            //    CurrentState.GainProportional;


            return clone;
        }
    }
}
