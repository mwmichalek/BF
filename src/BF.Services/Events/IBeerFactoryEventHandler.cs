﻿using BF.Common.Components;
using BF.Common.Events;
using BF.Common.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF.Service.Events {


    //************************************************* CHANGE EVENTS ***********************************************

    public enum ThreadType {
        UIThread,
        BackgroundThread,
        PublisherThread
    }

    public interface IBeerFactoryEventHandler {

        T CurrentComponentState<T>(ComponentId componentId) where T : ComponentState;

        IList<T> CurrentComponentStates<T>() where T : ComponentState;

        void ComponentStateChangeFiring<T>(ComponentStateChange<T> componentStateChange) where T : ComponentState;

        void SubscribeToComponentStateChange<T>(Action<ComponentStateChange<T>> componentStateChangeHandler,
                                            ComponentId componentId = ComponentId.UNDEFINED,
                                            ThreadType threadType = ThreadType.PublisherThread) where T : ComponentState;

        void ComponentStateRequestFiring<T>(ComponentStateRequest<T> componentStateRequest) where T : RequestedComponentState;

        void SubscribeToComponentStateRequest<T>(Action<ComponentStateRequest<T>> componentStateRequestHandler,
                                             ComponentId componentId = ComponentId.UNDEFINED,
                                             ThreadType threadType = ThreadType.PublisherThread) where T : RequestedComponentState;
    }



}
