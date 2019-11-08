using BF.Common.Components;
using BF.Common.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {

    public class ComponentStateChange<T> where T : ComponentState {

        public ComponentId Id { get; set; }

        public T PriorState { get; set; }

        public T CurrentState { get; set; }

        public string UserName { get; set; }

    }

    public static class ComponentStateChangeHelper {

        public static ComponentStateChange ToComponentStateChange<ComponentStateChange>(this string eventJson) {
            return JsonConvert.DeserializeObject<ComponentStateChange>(eventJson);
        }

        public static string ToJson<T>(this ComponentStateChange<T> componentStateChange) where T : ComponentState {
            return JsonConvert.SerializeObject(componentStateChange);
        }
    }

}
