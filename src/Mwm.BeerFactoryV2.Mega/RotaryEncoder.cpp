// 
// 
// 
#include <ClickEncoder.h>

#include "RotaryEncoder.h"

RotaryEncoder::RotaryEncoder(double min, double max, double step, double dfault, ClickEncoder *encoder) {
	_min = min;
	_max = max;
	_step = step;
	_value = dfault;
	_encoder = encoder;
}

void RotaryEncoder::update() {
	_encodervalue += _encoder->getValue();

	if (_encodervalue != _encoderlast) {
		if (_encodervalue > _encoderlast) {
			if (_value + _step < _max)
				_value = _value + _step;
			else
				_value = _max;
		} else {
			if (_value - _step > _min)
				_value = _value - _step;
			else
				_value = _min;
		}

		_encoderlast = _encodervalue;
	}

	ClickEncoder::Button b = _encoder->getButton();
	if (b != ClickEncoder::Open) {
		Serial.print("Button: ");
		
#define VERBOSECASE(label) case label: Serial.println(#label); break;
		switch (b) {
			VERBOSECASE(ClickEncoder::Pressed);
			VERBOSECASE(ClickEncoder::Held)
				VERBOSECASE(ClickEncoder::Released)
				VERBOSECASE(ClickEncoder::Clicked)
		case ClickEncoder::DoubleClicked:
			Serial.println("ClickEncoder::DoubleClicked");
			_encoder->setAccelerationEnabled(!_encoder->getAccelerationEnabled());
			Serial.print("  Acceleration is ");
			Serial.println((_encoder->getAccelerationEnabled()) ? "enabled" : "disabled");
			break;
		}
	}
}

void RotaryEncoder::setRange(double min, double max, double step, double dfault) {
	_min = min;
	_max = max;
	_step = step;
	_value = dfault;
}

double RotaryEncoder::getValue() {
	return _value;
}

void RotaryEncoder::setValue(double value) {
	_value = value;
}
