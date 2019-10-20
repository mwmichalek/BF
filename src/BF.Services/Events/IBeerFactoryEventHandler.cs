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

        void TemperatureChangeOccured(Action<TemperatureChange> temperatureChangeHandler);

        void ThermometerChangeOccured(Action<ThermometerChange> thermometerChangeHandler);


        void PumpRequestOccured(Action<PumpRequest> pumpRequestHandler);

        void PumpChangeOccured(Action<PumpChange> pumpChangeHandler);


        void PidRequestOccured(Action<PidRequest> pidRequestHandler);

        void PidChangeOccured(Action<PidChange> pidChangeHandler);


        void SsrChangeOccured(Action<SsrChange> ssrChangeHandler);


        void ConnectionStatusRequestOccured(Action<ConnectionStatusRequest> connectionStatusRequestHandler);

        void ConnectionStatusChangeOccured(Action<ConnectionStatusChange> connectionStatusChangeHandler);




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
