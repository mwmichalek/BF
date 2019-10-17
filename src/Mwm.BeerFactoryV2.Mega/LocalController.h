// LocalController.h

#ifndef _LOCALCONTROLLER_h
#define _LOCALCONTROLLER_h
#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include <LiquidCrystal_I2C.h>
#include "Thermometer.h"
#include "Kettle.h"

#define DISPLAY_REFRESH_MILLIS 1000

class LocalController {

public:
	LocalController();

	LocalController::LocalController(Thermometer* thermometers[],
		Kettle* hotLiquorTank, Kettle* boilKettle);


	LocalController(Thermometer* thermometer1, Thermometer* thermometer2, Thermometer* thermometer3,
		            Kettle* hotLiquorTank, Kettle* boilKettle);
	void update();
	
	void displayStatus();
	
	void postStatus();
	void postMsg(String msg);
	void connectionStatus(bool isConnected);
	void handleKettleRequest();



private:
	Thermometer* _thermometer1;
	Thermometer* _thermometer2;
	Thermometer* _thermometer3;
	Thermometer* _thermometer4;
	Thermometer* _thermometer5;
	Thermometer* _thermometer6;
	Thermometer* _thermometer7;
	Thermometer* _thermometer8;
	Thermometer* _thermometer9;

	int test = 0;
	
	Kettle* _hotLiquorTank;
	Kettle* _boilKettle;

	double _temperature1;
	double _temperature2;
	double _temperature3;
	double _temperature4;
	double _temperature5;
	double _temperature6;
	double _temperature7;
	double _temperature8;
	double _temperature9;

	int _nextDisplayUpdate;
	String _lastCmd;
	String _lastVal;
	int _isConnected;

	LiquidCrystal_I2C _lcd = LiquidCrystal_I2C(0x27, 2, 1, 0, 4, 5, 6, 7, 3, POSITIVE);

	void postTemperature(int index, double temperature);
	void processCommands();
	String printableTemp(double temperature);
};

#endif

