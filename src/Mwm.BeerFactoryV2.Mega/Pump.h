// Pump.h

#ifndef _PUMP_h
#define _PUMP_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class Pump {

public:
	Pump(int pumpPin);
	void update();

private:
	int _pumpPin;

};

#endif

