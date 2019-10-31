using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF.Common.Components {

    public enum ComponentId {
        HLT = 1,
        MT_IN = 2,
        MT = 3,
        MT_OUT = 4,
        BK = 5,
        HEX_IN = 6,
        HEX_OUT = 7,
        FERM = 8
    }

    public interface IComponent {

        ComponentId Id { get; }
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
            ComponentId.BK });

        public static T GetById<T>(this List<T> components, ComponentId componentId) where T : IComponent {
            return (T)components.SingleOrDefault(s => s.Id == componentId);
        }

        public static T GetById<T>(this T[] components, ComponentId componentId) where T : IComponent {
            return (T)components.SingleOrDefault(s => s.Id == componentId);
        }

    }

}
