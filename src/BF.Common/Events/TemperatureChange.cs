using BF.Common.Components;
using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class TemperatureChange : IEventPayload {

        public ComponentId Id { get; set; }

        public double Value { get; set; }

        public double Change { get; set; }

        public double PercentChange { get; set; }

        public DateTime Timestamp { get; set; }

    }

    
}
