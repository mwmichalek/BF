using BF.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {
    public class ConnectionStatusRequest : IEventPayload {

        public ComponentId Id { get; set; }

        public string ClientId { get; set; }
    }
}
