using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class SsrState : UpdateableComponentState {

        public int Percentage { get; set; }

        public bool IsFiring { get; set; }

        //public override bool Equals(object obj) {
        //    var otherSsr = (SsrState)obj;

        //    return (obj is SsrState otherSsrState) ?
             
        //        Percentage.Equals(otherSsrState.Percentage) &&
        //        IsEngaged.Equals(otherSsrState.IsEngaged) :
        //        false;
        //}

        

    }

    public static class SsrStateHelper {

        public static SsrState Clone(this SsrState ssrState) {
            return new SsrState {
                Percentage = ssrState.Percentage,
                IsFiring = ssrState.IsFiring
            };
        }

        public static SsrState Update(this SsrState ssrState, bool isFiring) {
            var clone = ssrState.Clone();
            clone.IsFiring = isFiring;
            return clone;
        }

        public static SsrState Update(this SsrState ssrState, SsrState newSsrState) {
            var clone = ssrState.Clone();

            // TODO: Add SsrState update logic.

            return clone;
        }
    }

}
