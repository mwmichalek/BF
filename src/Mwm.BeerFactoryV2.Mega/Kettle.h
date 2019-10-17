// Kettle.h

#ifndef _KETTLE_h
#define _KETTLE_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
	#include "HeatingElement.h"
#else
	#include "WProgram.h"
#endif

class Kettle {

public:
	Kettle();
	Kettle(int index, int ssrPin, String name, int cycleLengthInMillis, int heatingElementPin1, int heatingElementPin2);
	

	void update();
	//bool isEnabled();
	//bool isEngaged();
	//void enable(bool isEnabled);
	void engage(bool isEngaged);
	void disengage();
	int currentPercentage();
	void setPercentage(int percentage);
	void postSsrStatus();
	void postKettleStatus();
	
private:
	int _index;
	int _ssrPin;
	String _name;
	HeatingElement _heatingElement1;
	HeatingElement _heatingElement2;
	int _cycleLengthInMillis;
	int _percentage;
	int _millisOfOn;
	int _millisOfOff;
	unsigned long _timeToToggle;
	//bool _enabled;
	bool _engaged;
};

#endif

