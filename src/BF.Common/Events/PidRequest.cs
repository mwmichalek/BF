using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public enum PidMode {
        Temperature,
        Percentage
    }
    
    public class PidRequest {

        public PidControllerId Id { get; set; }

        public bool IsEngaged { get; set; }

        public PidMode PidMode { get; set; }

        public int SetPoint { get; set; }

    }
}
