using BF.Common.Events;
using BF.Common.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF.Common.Components {

    public enum ComponentId {
        UNDEFINED = 0,
        HLT = 1,
        MT_IN = 2,
        MT = 3,
        MT_OUT = 4,
        BK = 5,
        HEX_IN = 6,
        HEX_OUT = 7,
        FERM = 8
    }

    public abstract class ComponentBase<TState> : IEventPayload where TState : ComponentState {

        public ComponentId Id => CurrentState.Id;

        public TState CurrentState { get; protected set; }

        public TState PriorState { get; protected set; }

    }

    public abstract class ConfirgurableComponentBase<TState> : IEventPayload where TState : ConfigurableComponentState {

        public ComponentId Id => CurrentState.Id;

        public TState CurrentState { get; protected set; }

        public TState PriorState { get; protected set; }

    }

    public static class ComponentHelper {

        public static IList<ComponentId> AllComponentIds = new List<ComponentId>(new[] { 
            ComponentId.HLT,
            ComponentId.MT_IN,
            ComponentId.MT,
            ComponentId.MT_OUT,
            ComponentId.BK,
            ComponentId.HEX_IN,
            ComponentId.HEX_OUT,
            ComponentId.FERM
        });

        public static IList<ComponentId> PidComponentIds = new List<ComponentId>(new[] {
            ComponentId.HLT,
            ComponentId.MT,
            ComponentId.BK,
        });

        public static IList<ComponentId> SsrComponentIds = new List<ComponentId>(new []{ 
            ComponentId.HLT, 
            ComponentId.BK 
        });

        public static IList<ComponentId> PumpComponentIds = new List<ComponentId>(new[] {
            ComponentId.HLT,
            ComponentId.MT,
            ComponentId.BK,
        });

        public static ComponentId ToComponentId(this string componentIdStr) {
            return (ComponentId)Enum.Parse(typeof(ComponentId), componentIdStr);
        }

        public static ComponentId ToComponentId(this int componentIdInt) {
            return ToComponentId(componentIdInt.ToString());
        }

        //public static ComponentBase<T> GetById<ComponentBase<T>>(this List<ComponentBase<T>> components, ComponentId componentId) where T : ComponentState {
        //    return (T)components.SingleOrDefault(s => s.Id == componentId);
        //}

        //public static T GetById<T>(this T[] components, ComponentId componentId) where T : ComponentBase {
        //    return (T)components.SingleOrDefault(s => s.Id == componentId);
        //}

    }

}
