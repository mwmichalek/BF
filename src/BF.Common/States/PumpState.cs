﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class PumpState : ConfigurableComponentState {

        public override string ToString() {
            return $"Pump - Id: {Id}, Time: {Timestamp.Second}:{Timestamp.Millisecond}";
        }

    }

    public static class PumpStateHelper {

        public static PumpState Clone(this PumpState pumpState) {
            return new PumpState {
                IsEngaged = pumpState.IsEngaged,
                Timestamp = pumpState.Timestamp
            };
        }

        public static bool IsDifferent(this PumpState pumpState, PumpRequestState pumpRequestState) {
            return pumpState.IsEngaged != pumpRequestState.IsEngaged;
        }

        public static PumpState Update(this PumpState pumpState, PumpRequestState pumpRequestState) {
            var clone = pumpState.Clone();

            if (pumpRequestState.IsEngaged.HasValue)
                clone.IsEngaged = pumpRequestState.IsEngaged.Value;

            return clone;
        }

    }
}
