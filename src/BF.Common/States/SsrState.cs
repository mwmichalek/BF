using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class SsrState : UpdateableComponentState {

        public int Percentage { get; set; } = 0;

        public bool IsFiring { get; set; } = false;

    }

    public static class SsrStateHelper {

        public static SsrState Clone(this SsrState ssrState) {
            return new SsrState {
                Percentage = ssrState.Percentage,
                IsFiring = ssrState.IsFiring,
                Timestamp = ssrState.Timestamp
            };
        }

        public static SsrState Fire(this SsrState ssrState, bool isFiring) {
            var clone = ssrState.Clone();
            clone.IsFiring = isFiring;
            clone.Timestamp = DateTime.Now;
            return clone;
        }

        public static SsrState Engage(this SsrState ssrState, bool isEngaged) {
            var clone = ssrState.Clone();
            clone.IsEngaged = isEngaged;
            clone.Timestamp = DateTime.Now;
            return clone;
        }

        public static SsrState UpdateRequest(this SsrState ssrState, SsrState requestSsrState) {
            var clone = ssrState.Clone();
            //clone.IsEngaged = requestSsrState.IsEngaged;
            clone.Percentage = requestSsrState.Percentage;
            clone.Timestamp = DateTime.Now;

            return clone;
        }

        public static bool IsDifferent(this SsrState ssrState, SsrState requestSsrState) {
            return //ssrState.IsEngaged != requestSsrState.IsEngaged ||
                ssrState.Percentage != requestSsrState.Percentage;
        }
    }

}
