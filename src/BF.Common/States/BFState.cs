using BF.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class BFState : UpdateableComponentState {

        public Dictionary<ComponentId, SsrState> SsrStates { get; set; }

        public Dictionary<ComponentId, ThermometerState> ThermometerStates { get; set; }

        public Dictionary<ComponentId, PidControllerState> PidControllerStates { get; set; }

        public Dictionary<ComponentId, PumpState> PumpStates { get; set; }

    }
}
