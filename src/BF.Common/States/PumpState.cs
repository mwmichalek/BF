using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class PumpState : UpdateableComponentState {

    }

    public static class PumpStateHelper {

        public static PumpState Clone(this PumpState pumpState) {
            return new PumpState {
                IsEngaged = pumpState.IsEngaged,
                Timestamp = pumpState.Timestamp
            };
        }

        public static bool IsDifferent(this PumpState pumpState, PumpState requestPumpState) {
            return pumpState.IsEngaged != requestPumpState.IsEngaged;
        }
    }
}
