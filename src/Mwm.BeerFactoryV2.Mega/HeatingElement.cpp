#include <CmdMessenger.h>
#include "Arduino.h"
#include "HeatingElement.h"
#include "Events.h"

///
/// Used to allow power to the element.  Percentage is controlled by the SSR (Kettle)
///
HeatingElement::HeatingElement() {

}

HeatingElement::HeatingElement(int relayPin, const int index) {
	pinMode(relayPin, OUTPUT);
	_relayPin = relayPin;
	_index = index;
}

HeatingElement::HeatingElement(int relayPin, const String name) {
	pinMode(relayPin, OUTPUT);
	_relayPin = relayPin;
	_name = name;
}

void HeatingElement::enable(bool isEnabled) {
	_enabled = isEnabled;
}

void HeatingElement::engage(bool isEngaged) {
	//TODO: This is still notifying when the value doesn't change.
	bool valueChanged = false;
	bool trueEngaged = _enabled && isEngaged;

	if (_engaged != trueEngaged) {
		_engaged = trueEngaged;
		valueChanged = true;
	}

	// This doesn't do much but launch events when the parent SSR is engaged.
	if (valueChanged) {
		if (_engaged)
			postStatus(_relayPin, 1);
		
		else 	
			postStatus(_relayPin, 0);
	}
}

void HeatingElement::postStatus(int index, int onOrOff) {
	/*_cmdMessenger->sendCmdStart(Events::kHeaterChange);
	_cmdMessenger->sendCmdArg(index);
	_cmdMessenger->sendCmdArg(onOrOff);
	_cmdMessenger->sendCmdEnd();*/
}


