using System;
using System.Collections.Generic;
using System.Text;
using BF.Common.Components;

namespace BF.Common.Events {

    public class InitializationChange : IEventPayload {

        public ComponentId Id { get; set; }

        public Device Device { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

    }

}
