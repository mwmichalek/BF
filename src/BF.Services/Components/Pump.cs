using BF.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BF.Services.Components {
    public class Pump : IComponent {

        public ComponentId Id { get; set; }

        private bool _isEngaged;

        public bool IsEngaged {
            get { return _isEngaged; }
            set {
                _isEngaged = value;
            }
        }

        
    }
}
