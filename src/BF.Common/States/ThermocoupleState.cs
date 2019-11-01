using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {
    public class ThermocoupleState : ComponentState {

        public double Temperature { get; set; }

    }

    //public static class ThermocoupleStateHelper {

    //    public static ThermocoupleState Clone(this ThermocoupleState thermocoupleState) {
    //        return new ThermocoupleState {
    //            Temperature = thermocoupleState.Temperature,
    //            Timestamp = thermocoupleState.Timestamp
    //        };
    //    }

    //    public static ThermocoupleState Update(this ThermocoupleState thermocoupleState, double temperature) {
    //        var clone = thermocoupleState.Clone();
    //        clone.Temperature = temperature;
    //        clone.Timestamp = DateTime.Now;
    //        return clone;
    //    }

    //    public static ThermocoupleState Update(this ThermocoupleState thermocoupleState, ThermocoupleState newThermocoupleState) {
    //        var clone = thermocoupleState.Clone();
    //        clone.Temperature = newThermocoupleState.Temperature;
    //        clone.Timestamp = DateTime.Now;
    //        return clone;
    //    }
    //}

}
