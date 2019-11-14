using BF.Common.Components;
using BF.Common.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Common.Events {

    public class ComponentStateRequest<T> where T : RequestedComponentState {

        public ComponentId Id => RequestState.Id;

        public T RequestState { get; set; }

        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

    }

    public static class ComponentStateRequestHelper {

        public static ComponentStateRequest ToComponentStateRequest<ComponentStateRequest>(this string eventJson) {
            return JsonConvert.DeserializeObject<ComponentStateRequest>(eventJson);
        }

        public static string ToJson<T>(this ComponentStateRequest<T> componentStateRequest) where T : RequestedComponentState {
            return JsonConvert.SerializeObject(componentStateRequest);
        }

    }
}
