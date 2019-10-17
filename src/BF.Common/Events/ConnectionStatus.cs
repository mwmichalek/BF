using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public enum ConnectionState {
        Disconnected,
        Connected,
        NotConnected,
        Ready
    }

    public class ConnectionStatus : IEventPayload {

        public ConnectionState ConnectionState { get; set; }
    }


}
