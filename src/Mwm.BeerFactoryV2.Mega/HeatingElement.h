// HeatingElement.h

#ifndef _HEATINGELEMENT_h
#define _HEATINGELEMENT_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class HeatingElement {

	public:
		HeatingElement();
		HeatingElement(int relayPin, const String name);
		HeatingElement(int relayPin, const int index);
		void enable(bool isEnabled);
		void engage(bool isEngaged);
		void postStatus(int index, int onOrOff);

	private:
		int _relayPin;
		bool _enabled;
		bool _engaged;
		String _name;
		int _index;
};

#endif

