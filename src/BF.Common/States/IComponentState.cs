using BF.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.States {


    public abstract class ComponentState {

        public ComponentId Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

    }

    public abstract class ConfigurableComponentState : ComponentState {

        public bool IsEngaged { get; set; }

    }

    public abstract class RequestedComponentState : ComponentState {

        public bool? IsEngaged { get; set; }

    }

}
