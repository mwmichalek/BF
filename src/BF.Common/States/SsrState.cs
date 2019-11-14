using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class SsrState : ConfigurableComponentState {

        public int Percentage { get; set; } = 0;

        public bool IsFiring { get; set; } = false;

    }

    public static class SsrStateHelper {

        public static SsrState Clone(this SsrState ssrState) {
            return new SsrState {
                Id = ssrState.Id,
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

        public static SsrState UpdateRequest(this SsrState ssrState, SsrRequestState requestSsrState) {
            var clone = ssrState.Clone();

            if (requestSsrState.IsEngaged.HasValue)
                clone.IsEngaged = requestSsrState.IsEngaged.Value;

            if (requestSsrState.Percentage.HasValue)
                clone.Percentage = requestSsrState.Percentage.Value;

            clone.Timestamp = DateTime.Now;

            return clone;
        }

        public static bool IsDifferent(this SsrState ssrState, SsrRequestState ssrRequestState) {
            return ssrState.Percentage != ssrRequestState.Percentage;
        }
    }

}
