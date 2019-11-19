using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BF.Server.Components {
    public abstract class BFComponent : ComponentBase {

        protected string Balls { get; set; }


    }
}
