using BF.Common.Components;
using BF.Common.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {

    public class ComponentStateRequest<T> where T : UpdateableComponentState {

        public ComponentId Id { get; set; }

        public T RequestState { get; set; }

    }
}
