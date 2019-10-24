using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public enum PidMode {
        Temperature,
        Percentage
    }
    
    public class PidRequest : IEventPayload {

        public PidControllerId Id { get; set; }

        public bool IsEngaged { get; set; }

        public PidMode PidMode { get; set; }

        public double SetPoint { get; set; }

        public double GainDerivative { get; set; } = double.MinValue;

        public double GainIntegral { get; set; } = double.MinValue;

        public double GainProportional { get; set; } = double.MinValue;

    }
}
