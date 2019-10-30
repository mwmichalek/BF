using BF.Common.Components;
using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public enum PidMode {
        Temperature,
        Percentage,
        Unknown
    }
    
    public class PidRequest : IEventPayload {

        public ComponentId Id { get; set; }

        public bool? IsEngaged { get; set; }

        public PidMode? PidMode { get; set; }

        public double? SetPoint { get; set; }

        public double? GainDerivative { get; set; } 

        public double? GainIntegral { get; set; }

        public double? GainProportional { get; set; } 

    }
}
