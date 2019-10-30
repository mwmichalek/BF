using BF.Common.Components;
using BF.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public class PidControllerState : UpdateableComponentState {

        public PidMode PidMode { get; set; }

        public double SetPoint { get; set; }

        public double GainProportional { get; set; }

        public double GainIntegral { get; set; }

        public double GainDerivative { get; set; }

    }
}
