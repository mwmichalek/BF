// Events.h

#ifndef _EVENTS_h
#define _EVENTS_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class Events {

public:
	Events();
	enum {
		kAcknowledge,
		kError,
		kPingRequest,
		kPingResult,
		kStatusRequest,
		kStatusResult,
		kKettleRequest,
		kKettleResult,
		kTempChange,
		kSsrChange,
		kHeaterChange,
		kPumpChange,
		kMessage
	};

};


#endif

