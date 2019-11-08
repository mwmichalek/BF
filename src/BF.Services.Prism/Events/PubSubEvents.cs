using BF.Common.Events;
using BF.Common.States;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Prism.Events {

    public class ComponentStateChangeEvent<ComponentStateChange> : PubSubEvent<ComponentStateChange> { }

    public class ComponentStateRequestEvent<ComponentStateRequest> : PubSubEvent<ComponentStateRequest> { }

}
