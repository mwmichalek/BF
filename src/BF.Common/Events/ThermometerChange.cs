using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class ThermometerChange : IEventPayload {

        public ThermometerId Id { get; set; }

        public decimal Value { get; set; }

        public DateTime Timestamp { get; set; }
    }

    
}
