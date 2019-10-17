using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class PumpRequest : IEventPayload {

        public int Index { get; set; }

        public bool IsEngaged { get; set; }
    }
}
