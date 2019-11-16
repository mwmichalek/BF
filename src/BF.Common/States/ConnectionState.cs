using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {

    public enum ConnectionStatus {

        Connected,
        Disconnected

    }

    public class ConnectionState : ComponentState {

        public ConnectionStatus Status { get; set; }

        public override string ToString() {
            return $"Connection - Id: {Id}, Status: {Status} Time: {Timestamp.Second}:{Timestamp.Millisecond}";
        }

    }

}
