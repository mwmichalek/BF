﻿using BF.Common.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Common.Events {

    public class PumpRequest : IEventPayload {

        public PumpId Id { get; set; }

        public bool IsEngaged { get; set; }
    }
}
