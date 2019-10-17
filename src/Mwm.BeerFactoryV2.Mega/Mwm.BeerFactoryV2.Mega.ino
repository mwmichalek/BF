#include <Wire.h>
#include <Firmata.h>
#include <Boards.h>
#include <Timer.h>
#include <Event.h>
#include <LCD.h>
#include "Events.h"
//#include <CmdMessenger.h>
#include <OneWire.h>
#include <DallasTemperature.h>
#include "Pump.h"
#include "RemoteController.h"
#include "LocalController.h"
#include "HeatingElement.h"
#include "Kettle.h"
#include "Thermometer.h"

//******************************************************************************
//
// Before you wipe way this machine, make sure you have libraries backed up!
//
//	C:\Users\mwmic\Documents\Arduino
//
// C:\Program Files (x86)\Arduino\libraries\NewliquidCrystal
//
//******************************************************************************
#define SLAVE_ADDRESS 0x40

#define SSR_PIN_1 5
#define SSR_PIN_2 6

#define HEATINGELEMENT_PIN_1 1
#define HEATINGELEMENT_PIN_2 2
#define HEATINGELEMENT_PIN_3 3
#define HEATINGELEMENT_PIN_4 4

#define PUMP_PIN_1 10
#define PUMP_PIN_2 11
#define PUMP_PIN_3 12


#define HEATER_CYCLE_IN_MILLIS 5000
#define TEMP_READING_CYCLE_IN_MILLIS 1000
#define PING_CYCLE_IN_MILLIS 5000
#define CONFIG_CYCLE_IN_MILLIS 2000

// Data wire is plugged into pin 2 on the Arduino Blue Pin, Red Pin to 3.3V, Black Pin to GND, Resister between Red and Blue
#define ONE_WIRE_BUS 2   

OneWire oneWire1(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire1);

DeviceAddress probe01 = { 0x28, 0xFF, 0x5E, 0xC0, 0x4E, 0x04, 0x00, 0xB8 };


DeviceAddress probe04 = { 0x28, 0xFF, 0x43, 0x0E, 0x16, 0x15, 0x03, 0x90 };
DeviceAddress probe05 = { 0x28, 0xFF, 0xD2, 0x37, 0x16, 0x15, 0x03, 0x31 };
DeviceAddress probe06 = { 0x28, 0xFF, 0x4A, 0x3A, 0x16, 0x15, 0x03, 0x25 };

DeviceAddress probe09 = { 0x28, 0xFF, 0x81, 0x22, 0x41, 0x18, 0x02, 0x85 };
DeviceAddress probe10 = { 0x28, 0xFF, 0x66, 0x27, 0x41, 0x18, 0x02, 0x8F };
DeviceAddress probe11 = { 0x28, 0xFF, 0xF3, 0x23, 0x41, 0x18, 0x02, 0xE4 };
DeviceAddress probe12 = { 0x28, 0xFF, 0x0C, 0x22, 0x41, 0x18, 0x02, 0xA2 };
DeviceAddress probe13 = { 0x28, 0xFF, 0x85, 0x20, 0x41, 0x18, 0x02, 0x9D };

Thermometer thermometer1;
Thermometer thermometer2;
Thermometer thermometer3;
Thermometer thermometer4;
Thermometer thermometer5;
Thermometer thermometer6;
Thermometer thermometer7;
Thermometer thermometer8;
Thermometer thermometer9;

Kettle hotLiquorTank;
Kettle boilKettle;

//Pump hotWaterPump(PUMP_PIN_1);
//Pump wortPump(PUMP_PIN_2);

unsigned long LastTempUpdate, LastMessageReceived, LastPing, LastConfigRequest;

LocalController localController;

bool isConfigured = false;

//CmdMessenger cmdMessenger = CmdMessenger(Serial);

void setup() {
	Serial.begin(57600);
	sensors.begin();

	Wire.begin(SLAVE_ADDRESS);

	thermometer1 = Thermometer(sensors, probe01, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer2 = Thermometer(sensors, probe04, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer3 = Thermometer(sensors, probe05, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer4 = Thermometer(sensors, probe06, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer5 = Thermometer(sensors, probe09, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer6 = Thermometer(sensors, probe10, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer7 = Thermometer(sensors, probe11, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer8 = Thermometer(sensors, probe12, TEMP_READING_CYCLE_IN_MILLIS);
	thermometer9 = Thermometer(sensors, probe13, TEMP_READING_CYCLE_IN_MILLIS);

	hotLiquorTank = Kettle(1, SSR_PIN_1, "HLT", HEATER_CYCLE_IN_MILLIS, HEATINGELEMENT_PIN_1, HEATINGELEMENT_PIN_2);
	boilKettle = Kettle(2, SSR_PIN_2, "BK", HEATER_CYCLE_IN_MILLIS, HEATINGELEMENT_PIN_3, HEATINGELEMENT_PIN_4);

	Thermometer* thermometers[] = { &thermometer1, &thermometer2, &thermometer3, 
								    &thermometer4, &thermometer5, &thermometer6, 
		                            &thermometer7, &thermometer8, &thermometer9 };

	localController = LocalController(thermometers, &hotLiquorTank, &boilKettle);

	pinMode(PUMP_PIN_1, OUTPUT);
	pinMode(PUMP_PIN_2, OUTPUT);
	pinMode(PUMP_PIN_3, OUTPUT);

	digitalWrite(PUMP_PIN_1, HIGH);
	digitalWrite(PUMP_PIN_2, LOW);
	digitalWrite(PUMP_PIN_3, HIGH);

	//configureCmdMessenger();
}


void loop() {
	localController.update();
	delay(1000);
	//localController.postStatus();
	//cmdMessenger.feedinSerialData();
}

//char* toCharArray(String command) {
//	if (command.length() != 0) {
//		char *p = const_cast<char*>(command.c_str());
//		return p;
//	}
//}

//void configureCmdMessenger() {
//	cmdMessenger.printLfCr();
//	cmdMessenger.attach(onUnknownCommand);
//	cmdMessenger.attach(Events::kStatusRequest, onStatusRequest);
//	cmdMessenger.attach(Events::kPingRequest, onPing);
//	cmdMessenger.attach(Events::kKettleRequest, onKettleRequest);
//	onArduinoReady();
//}

// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
//void onUnknownCommand() {
//	cmdMessenger.sendCmd(Events::kError, "Command without attached callback");
//}
//
//void onStatusRequest() {
//	localController.postStatus();
//}
//
//// Callback function that responds that Arduino is ready (has booted up)
//void onArduinoReady() {
//	cmdMessenger.sendCmd(Events::kAcknowledge, "Arduino ready");
//}
//
//void onPing() {
//	cmdMessenger.sendCmdStart(Events::kPingResult);
//	cmdMessenger.sendCmdEnd();
//}
//
//void onKettleRequest() {
//	localController.handleKettleRequest();
//}

















//DeviceAddress probe07 = { 0x28, 0xFF, 0xA1, 0x1E, 0x16, 0x15, 0x03, 0xF1 };
//DeviceAddress probe08 = { 0x28, 0xFF, 0x51, 0x24, 0x16, 0x15, 0x03, 0x04 };
//DeviceAddress probe02 = { 0x28, 0xFF, 0x14, 0xDB, 0x3C, 0x04, 0x00, 0xE0 };
//DeviceAddress probe03 = { 0x28, 0xFF, 0xAF, 0x3B, 0x4A, 0x04, 0x00, 0xA8 };