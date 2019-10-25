using BF.Common.Components;
using BF.Common.Ids;
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

    public class ConnectionStatusChange : IEventPayload {

        public ComponentId Id { get; set; }

        public string ClientId { get; set; }

        public ConnectionState ConnectionState { get; set; }

        public List<ThermometerChange> ThermometerChanges { get; set; }

        public List<SsrChange> SsrChanges { get; set; }

        public List<PumpChange> PumpChanges { get; set; }

        public List<PidChange> PidChanges { get; set; }

    }


}
