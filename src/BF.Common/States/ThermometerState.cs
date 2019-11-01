using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class ThermometerState : ComponentState {

        public double Temperature { get; set; }

    }

    public static class ThermometerStateHelper {

        public static ThermometerState Clone(this ThermometerState thermometerState) {
            if (thermometerState == null) return null;
            return new ThermometerState {
                Temperature = thermometerState.Temperature,
                Timestamp = thermometerState.Timestamp
            };
        }

        public static ThermometerState Update(this ThermometerState thermometerState, double temperature) {
            if (thermometerState == null) return null;
            var clone = thermometerState.Clone();
            clone.Temperature = temperature;
            clone.Timestamp = DateTime.Now;
            return clone;
        }

        public static ThermometerState Update(this ThermometerState thermometerState, ThermometerState newThermometerState) {
            if (thermometerState == null) return null;
            var clone = thermometerState.Clone();
            clone.Temperature = newThermometerState.Temperature;
            clone.Timestamp = DateTime.Now;
            return clone;
        }

        public static ThermometerState Update(this ThermometerState thermometerState, ThermocoupleState newThermocoupleState) {
            if (thermometerState == null) return null;
            var clone = thermometerState.Clone();
            clone.Temperature = newThermocoupleState.Temperature;
            clone.Timestamp = DateTime.Now;
            return clone;
        }
    }

}
