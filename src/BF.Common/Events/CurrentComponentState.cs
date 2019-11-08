using BF.Common.Components;
using BF.Common.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {

    public class CurrentComponentState<T> where T : ComponentState {

        public ComponentId Id { get; set; }

        public T CurrentState { get; set; }

    }

}
