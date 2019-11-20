using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BF.Server.Components {
    public abstract class BFComponent : ComponentBase {

        protected void RepeatUntilComplete(Func<bool> func) {
            Task.Run(() => {
                func();
                do {
                    InvokeAsync(() => StateHasChanged());
                    if (!func())
                        Thread.Sleep(50);
                } while (!func());
            });
        }

    }
}
