using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BF.Server.Components {
    public abstract class BFComponent : ComponentBase {

        protected string Balls { get; set; }


        //public async Task Motherfucker(out int target, int source, Func<int> pred) {
        //    await Task.Run(() => {
        //        do {
        //            target = source;
        //            InvokeAsync(() => StateHasChanged());
        //            if (pred() != source)
        //                Thread.Sleep(50);
        //        } while (pred() != source);
        //    });
        //}
    }
}
