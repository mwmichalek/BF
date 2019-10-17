// 
// 
// 
#include "Pump.h"

Pump::Pump(int pumpPin) {
	pinMode(pumpPin, OUTPUT);
	_pumpPin = pumpPin;
}

void Pump::update() {
	Serial.println("Pump update.");
}

