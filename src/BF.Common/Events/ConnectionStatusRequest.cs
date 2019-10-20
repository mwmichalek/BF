using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {
    public class ConnectionStatusRequest : IEventPayload {

        public string ClientId { get; set; }
    }
}
