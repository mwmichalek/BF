using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class TemperatureChange : IEventPayload {

        public ThermometerId Id { get; set; }

        public decimal Value { get; set; }

        public decimal Change { get; set; }

        public decimal PercentChange { get; set; }

        public DateTime Timestamp { get; set; }

    }

    
}
