using BF.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class BFState : UpdateableComponentState {

        public IDictionary<ComponentId, SsrState> SsrStates { get; set; }

        public IDictionary<ComponentId, ThermometerState> ThermometerStates { get; set; }

        public IDictionary<ComponentId, PidControllerState> PidControllerStates { get; set; }

        public IDictionary<ComponentId, PumpState> PumpStates { get; set; }

    }
}
