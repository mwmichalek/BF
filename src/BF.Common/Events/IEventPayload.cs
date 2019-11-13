using BF.Common.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public interface IEventPayload {

    }

    public static class EventPayloadHelper {



        public static T ToEvent<T>(this string eventJson) where T : IEventPayload {
            return JsonConvert.DeserializeObject<T>(eventJson); 
        }

        public static string ToJson(this IEventPayload eventPayload) {
            return JsonConvert.SerializeObject(eventPayload);
        }
            
    }
}
