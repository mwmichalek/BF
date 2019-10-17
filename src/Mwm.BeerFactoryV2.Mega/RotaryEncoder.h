// RotaryEncoder_cs.h

#ifndef _ROTARYENCODER_h
#define _ROTARYENCODER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <ClickEncoder.h>

class RotaryEncoder {

public:
	RotaryEncoder(double min, double max, double step, double dfault, ClickEncoder *encoder);
	void update();
	void setRange(double min, double max, double step, double dfault);
	double getValue();
	void setValue(double value);

private:
	double _min = 0;
	double _max = 100;
	double _step = 1;
	double _value = _min;

	ClickEncoder *_encoder;
	
	int16_t _encoderlast = -1;
	int16_t _encodervalue = -1;
};

#endif

