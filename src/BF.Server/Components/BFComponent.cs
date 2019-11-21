using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BF.Common.Components;
using BF.Service.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BF.Server.Components {
    public abstract class BFComponent : ComponentBase {

        [Parameter]
        public ComponentId ComponentId { get; set; }

        [Inject]
        protected IBeerFactoryEventHandler EventHandler { get; set; }

        [Inject]
        protected ILoggerFactory LoggerFactory { get; set; }

        protected ILogger Logger;

        protected bool IsInitialized = false;

        protected override async Task OnInitializedAsync() {

        }

        protected override void OnParametersSet() {
            if (IsInitialized)
                Initialize();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                Initialize();
                IsInitialized = true;
            }
        }

        public virtual void Initialize() { }

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
