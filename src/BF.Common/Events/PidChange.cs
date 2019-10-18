using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class PidChange {

        public PidControllerId Id { get; set; }

        public bool IsEngaged { get; set; }

        public PidMode PidMode { get; set; }

        public double SetPoint { get; set; }

        public double GainDerivative { get; set; }

        public double GainIntegral { get; set; }

        public double GainProportional { get; set; }

    }
}
