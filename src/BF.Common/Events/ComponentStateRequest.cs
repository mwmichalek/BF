using BF.Common.Components;
using BF.Common.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {

    public class ComponentStateRequest<T> where T : UpdateableComponentState {

        public ComponentId Id { get; set; }

        public T RequestState { get; set; }

        public string UserName { get; set; }

    }

    public static class ComponentStateRequestHelper {

        public static ComponentStateRequest ToComponentStateRequest<ComponentStateRequest>(this string eventJson) {
            return JsonConvert.DeserializeObject<ComponentStateRequest>(eventJson);
        }

        public static string ToJson<T>(this ComponentStateRequest<T> componentStateRequest) where T : UpdateableComponentState {
            return JsonConvert.SerializeObject(componentStateRequest);
        }

    }
}
