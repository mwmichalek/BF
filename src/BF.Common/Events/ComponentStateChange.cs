using BF.Common.Components;
using BF.Common.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF.Common.Events {

    public class ComponentStateChange<T> where T : ComponentState {

        public ComponentId Id { get; set; }

        public T PriorState { get; set; }

        public T CurrentState { get; set; }

        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

    }

    public static class ComponentStateHelper {

        public static IDictionary<ComponentId, T> ToDictionary<T>(this Dictionary<Tuple<Type, ComponentId>, ComponentState> componentStates) where T : ComponentState {
            return componentStates.Where(cs => cs.Value.GetType() == typeof(T)).ToDictionary(cs => cs.Key.Item2, cs => (T)cs.Value);
        }

        public static ComponentStateChange<T> ToComponentStateChange<T>(this KeyValuePair<ComponentId, T> kv) where T : ComponentState {
            return new ComponentStateChange<T> {
                Id = kv.Key,
                CurrentState = kv.Value
            };
        }

        public static ComponentStateChange ToComponentStateChange<ComponentStateChange>(this string eventJson) {
            return JsonConvert.DeserializeObject<ComponentStateChange>(eventJson);
        }

        public static string ToJson<T>(this ComponentStateChange<T> componentStateChange) where T : ComponentState {
            return JsonConvert.SerializeObject(componentStateChange);
        }
    }

}
