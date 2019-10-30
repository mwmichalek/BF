using BF.Common.Events;
using BF.Common.States;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Prism.Events {

    public class InitializationChangeEvent : PubSubEvent<InitializationChange> { }

    public class ConnectionStatusRequestEvent : PubSubEvent<ConnectionStatusRequest> { }

    public class ConnectionStatusChangeEvent : PubSubEvent<ConnectionStatusChange> { }

    public class HeaterChangeEvent : PubSubEvent<HeaterChange> { }

    public class PidChangeEvent : PubSubEvent<PidChange> { }

    public class PidRequestEvent : PubSubEvent<PidRequest> { }

    public class PumpChangeEvent : PubSubEvent<PumpChange> { }

    public class PumpRequestEvent : PubSubEvent<PumpRequest> { }

    public class SsrChangeEvent : PubSubEvent<SsrChange> { }

    public class TemperatureChangeEvent : PubSubEvent<TemperatureChange> { }

    public class ThermometerChangeEvent : PubSubEvent<ThermometerChange> { }


    public class ComponentStateChangeEvent<ComponentStateChange> : PubSubEvent<ComponentStateChange> { }



}
