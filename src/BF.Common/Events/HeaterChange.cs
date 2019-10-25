using BF.Common.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class HeaterChange : IEventPayload {

        public ComponentId Id { get; set; }

        public bool IsEngaged { get; set; }
    }

    
}
