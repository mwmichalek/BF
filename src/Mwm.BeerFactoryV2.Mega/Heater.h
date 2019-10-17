// Heater.h

#ifndef _HEATER_h
#define _HEATER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif


class Heater {

	public:
		Heater(int pin);
		void engage(bool isEngaged);

	private:
		int _pin;
		bool _isEngaged;
		bool _isPowered;
};

#endif

