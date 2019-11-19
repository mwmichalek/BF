using BF.Common.Components;
using BF.Common.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF.Common.Events {

    public class ComponentStateChange<T> where T : ComponentState {

        public ComponentId Id => CurrentState.Id;

        public T PriorState { get; set; }

        public T CurrentState { get; set; }

        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

    }

    public static class ComponentStateHelper {

        public static ComponentStateChange ToComponentStateChange<ComponentStateChange>(this string eventJson) {
            return JsonConvert.DeserializeObject<ComponentStateChange>(eventJson);
        }

        public static ComponentStateChange<T> ToComponentStateChange<T>(this T state) where T : ComponentState {
            return new ComponentStateChange<T> { CurrentState = state };
        }

        public static string ToJson<T>(this ComponentStateChange<T> componentStateChange) where T : ComponentState {
            return JsonConvert.SerializeObject(componentStateChange);
        }
    }

}
