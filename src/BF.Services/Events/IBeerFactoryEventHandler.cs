using BF.Common.Events;
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

        void InitializationChangeOccured(Action<InitializationChange> initializationHandler, ThreadType threadType = ThreadType.PublisherThread);

        void TemperatureChangeOccured(Action<TemperatureChange> temperatureChangeHandler, ThreadType threadType = ThreadType.PublisherThread);

        void ThermometerChangeOccured(Action<ThermometerChange> thermometerChangeHandler, ThreadType threadType = ThreadType.PublisherThread);


        void PumpRequestOccured(Action<PumpRequest> pumpRequestHandler, ThreadType threadType = ThreadType.PublisherThread);

        void PumpChangeOccured(Action<PumpChange> pumpChangeHandler, ThreadType threadType = ThreadType.PublisherThread);


        void PidRequestOccured(Action<PidRequest> pidRequestHandler, ThreadType threadType = ThreadType.PublisherThread);

        void PidChangeOccured(Action<PidChange> pidChangeHandler, ThreadType threadType = ThreadType.PublisherThread);


        void SsrChangeOccured(Action<SsrChange> ssrChangeHandler, ThreadType threadType = ThreadType.PublisherThread);


        void ConnectionStatusRequestOccured(Action<ConnectionStatusRequest> connectionStatusRequestHandler, ThreadType threadType = ThreadType.PublisherThread);

        void ConnectionStatusChangeOccured(Action<ConnectionStatusChange> connectionStatusChangeHandler, ThreadType threadType = ThreadType.PublisherThread);


        void InitializationChangeFired(InitializationChange initializationChange);

        void TemperatureChangeFired(TemperatureChange temperatureChange);

        void ThermometerChangeFired(ThermometerChange thermometerChange);


        void PumpRequestFired(PumpRequest pumpRequest); 
        
        void PumpChangeFired(PumpChange pumpChange);


        void PidRequestFired(PidRequest pidRequest);

        void PidChangeFired(PidChange pidChange);

        
        void SsrChangeFired(SsrChange ssrChange);

        void ConnectionStatusRequestFired(ConnectionStatusRequest connectionStatusRequest);

        void ConnectionStatusChangeFired(ConnectionStatusChange connectionStatusChange);
    }



}
