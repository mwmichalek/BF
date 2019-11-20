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
        protected ILoggerFactory _loggerFactory { get; set; }

        protected ILogger _logger;

        private bool _isInitialized = false;

        protected override async Task OnInitializedAsync() {
            //_logger = _loggerFactory.CreateLogger("SetPointComponent");
        }

        protected override void OnParametersSet() {
            if (_isInitialized)
                Initialize();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                Initialize();
                _isInitialized = true;
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
