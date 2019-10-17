#include "Thermometer.h"
#include <DallasTemperature.h>
#include <OneWire.h>

Thermometer::Thermometer() {}

Thermometer::Thermometer(const DallasTemperature &sensor, const DeviceAddress &probe, int cycleLengthInMillis) {
	_cycleLengthInMillis = cycleLengthInMillis;
	_timeToUpdate = 0;
	_temperature = 0;
	_probe = probe;
	_sensor = sensor;
	_enabled = true;
}

Thermometer::Thermometer(const DallasTemperature &sensor, const DeviceAddress &probe, int cycleLengthInMillis, bool enabled) {
	_cycleLengthInMillis = cycleLengthInMillis;
	_timeToUpdate = 0;
	_temperature = 0;
	_probe = probe;
	_sensor = sensor;
	_enabled = enabled;
}

void Thermometer::update() {
	unsigned long currentMillis = millis();

	if (_enabled && currentMillis > _timeToUpdate) {
		_timeToUpdate = currentMillis + _cycleLengthInMillis;
		_sensor.requestTemperaturesByAddress(_probe);
		_temperature = _sensor.getTempF(_probe);
	}
}

double Thermometer::currentTemp() {
	return _temperature;
}


void Thermometer::setEnabled(bool isEnabled) {
	_enabled = isEnabled;
}