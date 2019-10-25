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

        public static T GetById<T>(this List<T> components, ComponentId componentId) where T : IComponent {
            return (T)components.SingleOrDefault(s => s.Id == componentId);
        }

    }

}
