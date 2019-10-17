//// 
//// 
//// 
//#include <SoftwareSerial.h>
//#include <Boards.h>
//#include <Firmata.h>
//
//#include "RemoteController.h"
//
//RemoteController::RemoteController() {
//	/*Firmata.setFirmwareVersion(FIRMATA_FIRMWARE_MAJOR_VERSION, FIRMATA_FIRMWARE_MINOR_VERSION);
//	Firmata.attach(STRING_DATA, receiveSettings);
//	Firmata.begin(115200);*/
//}
//
//RemoteController::RemoteController(Thermometer &thermometer1, Thermometer &thermometer2, Thermometer &thermometer3) {
//	_thermometer1 = thermometer1;
//	_thermometer2 = thermometer2;
//	_thermometer3 = thermometer3;
//}
//
//void RemoteController::update() {
//	_thermometer1.update();
//	_thermometer2.update();
//	_thermometer3.update();
//
//	_temperature1 = _thermometer1.currentTemp();
//	_temperature2 = _thermometer2.currentTemp();
//	_temperature3 = _thermometer3.currentTemp();
//	//LOG: RemoteController - T1: {_temperature1}, T2 : {_temperature2}, T3 : {_temperature3}
//}
//
//void receiveSettings(char *msg) {
//	//isConfigured = true;
//
//	//LastMessageReceived = millis();
//
//	//String msgString = String(msg);
//
//	//TODO: Rename these.
//	/*int heater1Index = msgString.indexOf(':');
//	int heater2Index = msgString.indexOf(':', heater1Index + 1);
//	int pump1Index = msgString.indexOf(':', heater2Index + 1);
//	int pump2Index = msgString.indexOf(':', pump1Index + 1);
//
//	int heater1value = msgString.substring(0, heater1Index).toInt();
//	int heater2value = msgString.substring(heater1Index + 1, heater2Index).toInt();
//	int pump1value = msgString.substring(heater2Index + 1, pump1Index).toInt();
//	int pump2value = msgString.substring(pump1Index + 1).toInt();
//	String timestamp = msgString.substring(pump2Index + 1);*/
//
//	/*clearDisplay(9);
//
//	lcd.setCursor(0, 0);
//	lcd.print("H1: ");
//	lcd.print(heater1value);
//	lcd.setCursor(0, 1);
//	lcd.print("H2: ");
//	lcd.print(heater2value);
//	lcd.setCursor(0, 2);
//	lcd.print("P1: ");
//	lcd.print(pump1value);
//	lcd.setCursor(0, 3);
//	lcd.print("P2: ");
//	lcd.print(pump2value);
//
//	lcd.setCursor(9, 3);
//	lcd.print("T:");
//	lcd.print(timestamp);
//
//	hotLiquorTank.setPercentage(heater1value);
//	boilKettle.setPercentage(heater2value);*/
//}
//
//void sendSetting(String settingName, double settingValue) {
//	//String settingStr = settingName + "=" + String(settingValue, 2);
//	//Firmata.sendString(settingStr.c_str());
//}
//
//void processInput() {
//	//if (Firmata.available())
//	//	Firmata.processInput();
//}
//
