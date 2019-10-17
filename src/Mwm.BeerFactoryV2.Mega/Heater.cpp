// 
// 
// 
#include "Arduino.h"
#include "Heater.h"

///
/// Used to allow power to the element.  Percentage is controlled by the SSR (Kettle)
///
Heater::Heater(int pin) {
	pinMode(pin, OUTPUT);
	_pin = pin;
	engage(false);
}

void Heater::engage(bool isEngaged) {
	_isEngaged = isEngaged;
	if (isEngaged)
		digitalWrite(_pin, HIGH);
	else
		digitalWrite(_pin, LOW);
}

