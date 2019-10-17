#include <CmdMessenger.h>
#include <LiquidCrystal_I2C.h>
#include "LocalController.h"
#include "Kettle.h"
#include "Events.h"

LocalController::LocalController() {
}

LocalController::LocalController(Thermometer* thermometers[], 
	Kettle* hotLiquorTank, Kettle* boilKettle) {
	_thermometer1 = thermometers[0];
	_thermometer2 = thermometers[1];
	_thermometer3 = thermometers[2];
	_thermometer4 = thermometers[3];
	_thermometer5 = thermometers[4];
	_thermometer6 = thermometers[5];
	_thermometer7 = thermometers[6];
	_thermometer8 = thermometers[7];
	_thermometer9 = thermometers[8];

	_hotLiquorTank = hotLiquorTank;
	_boilKettle = boilKettle;

	_lcd.begin(20, 4);
	_lcd.setBacklight(HIGH);

	_lcd.setCursor(0, 0);            // go to the top left corner
	_lcd.print("BF v3.0");

	//TODO: (Michalek) HARDCODED!!!!
	//_hotLiquorTank->setPercentage(0);
	//_hotLiquorTank->enable(false);

	//_boilKettle->setPercentage(0);
	//_boilKettle->enable(false);
}

void LocalController::update() {
	_thermometer1->update();
	_thermometer2->update();
	_thermometer3->update();
	_thermometer4->update();
	_thermometer5->update();
	_thermometer6->update();
	_thermometer7->update();
	_thermometer8->update();
	_thermometer9->update();
	   
	double newTemperature1 = _thermometer1->currentTemp();
	if (newTemperature1 != _temperature1) {
		_temperature1 = newTemperature1;
		postTemperature(1, _temperature1);
	}

	double newTemperature2 = _thermometer2->currentTemp();
	if (newTemperature2 != _temperature2) {
		_temperature2 = newTemperature2;
		postTemperature(2, _temperature2);
	}
	
	double newTemperature3 = _thermometer3->currentTemp();
	if (newTemperature3 != _temperature3) {
		_temperature3 = newTemperature3;
		postTemperature(3, _temperature3);
	}

	double newTemperature4 = _thermometer4->currentTemp();
	if (newTemperature4 != _temperature4) {
		_temperature4 = newTemperature4;
		postTemperature(4, _temperature4);
	}

	double newTemperature5 = _thermometer5->currentTemp();
	if (newTemperature5 != _temperature5) {
		_temperature5 = newTemperature5;
		postTemperature(5, _temperature5);
	}

	double newTemperature6 = _thermometer6->currentTemp();
	if (newTemperature6 != _temperature6) {
		_temperature6 = newTemperature6;
		postTemperature(6, _temperature6);
	}

	double newTemperature7 = _thermometer7->currentTemp();
	if (newTemperature7 != _temperature7) {
		_temperature7 = newTemperature7;
		postTemperature(7, _temperature7);
	}

	double newTemperature8 = _thermometer8->currentTemp();
	if (newTemperature8 != _temperature8) {
		_temperature8 = newTemperature8;
		postTemperature(8, _temperature8);
	}

	double newTemperature9 = _thermometer9->currentTemp();
	if (newTemperature9 != _temperature9) {
		_temperature9 = newTemperature9;
		postTemperature(9, _temperature9);
	}

	_hotLiquorTank->update();
	_boilKettle->update();

	processCommands();

	displayStatus();
}

void LocalController::processCommands() {

	if (Serial.available() > 0) {
		String funcName = Serial.readString();
		if (funcName.startsWith("temps"))
			postStatus();
	}
}

void LocalController::handleKettleRequest() {

}

void LocalController::postTemperature(int index, double temperature) {
	if (temperature != 185) {
		Serial.println(String(index) + "|" + String(temperature));
	}
}


void LocalController::postStatus() {
	_isConnected = true;
	Serial.println("1|" + printableTemp(_temperature1));
	Serial.println("2|" + printableTemp(_temperature2));
	Serial.println("3|" + printableTemp(_temperature3));
	Serial.println("4|" + printableTemp(_temperature4));
	Serial.println("5|" + printableTemp(_temperature5));
	Serial.println("6|" + printableTemp(_temperature6));
	Serial.println("7|" + printableTemp(_temperature7));
	Serial.println("8|" + printableTemp(_temperature8));
	Serial.println("9|" + printableTemp(_temperature9));

	//_hotLiquorTank->postKettleStatus();
	//_boilKettle->postKettleStatus();
}


void LocalController::connectionStatus(bool isConnected) {
	if (isConnected)
		_isConnected = 1;
	else
		_isConnected = 0;
}

void LocalController::postMsg(String msg) {
	_lcd.setCursor(0, 3);
	_lcd.print(String(msg));
}

String LocalController::printableTemp(double temperature) {
	if (temperature < 0)
		return "-----";
	return String(temperature);
}

void LocalController::displayStatus() {
	_lcd.setCursor(0, 0);
	_lcd.print(printableTemp(_temperature1) + " " + printableTemp(_temperature2) + " " + printableTemp(_temperature3));
	_lcd.setCursor(0, 1);
	_lcd.print(printableTemp(_temperature4) + " " + printableTemp(_temperature5) + " " + printableTemp(_temperature6));
	_lcd.setCursor(0, 2);
	_lcd.print(printableTemp(_temperature7) + " " + printableTemp(_temperature8) + " " + printableTemp(_temperature9));
	//_lcd.setCursor(0, 3);
	//_lcd.print("HLT:" + String(_hotLiquorTank->currentPercentage()) + " BK:" + String(_boilKettle->currentPercentage()) );
}
