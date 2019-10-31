using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class ThermometerState : ComponentState {

        public double Temperature { get; set; }

    }

    public static class ThermometerStateHelper {

        public static ThermometerState Clone(this ThermometerState thermometerState) {
            return new ThermometerState {
                Temperature = thermometerState.Temperature
            };
        }

        public static ThermometerState Update(this ThermometerState thermometerState, double temperature) {
            var clone = thermometerState.Clone();
            clone.Temperature = temperature;
            return clone;
        }
    }

}
