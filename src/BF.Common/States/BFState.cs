using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BF.Common.States {

    public class BFRequestState : RequestedComponentState {

    }

    public class BFState : ConfigurableComponentState {

        public override string ToString() {
            return $"BF - Id: {Id}, Therms: {ThermometerStates?.Count}, Ssrs: {SsrStates?.Count}, PIDs: {PidControllerStates?.Count}, Pumps: {PumpStates?.Count} Time: {Timestamp.Second}:{Timestamp.Millisecond}";
        }

        public IList<ThermometerState> ThermometerStates { get; set; }

        public IList<SsrState> SsrStates { get; set; }

        public IList<PidControllerState> PidControllerStates { get; set; }

        public IList<PumpState> PumpStates { get; set; } 

    }
}
