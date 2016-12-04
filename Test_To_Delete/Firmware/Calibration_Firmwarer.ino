  // Include libraries
  #include <Wire.h>
  #include <OneWire.h>
  #include <DallasTemperature.h>
  
  // Define variables for digital temperature 
  #define ONE_WIRE_BUS 2
  #define TEMPERATURE_PRECISION 11
  
  OneWire oneWire(ONE_WIRE_BUS);
  DallasTemperature sensors(&oneWire);
  int NumberOfDevices;
  int WaterAmount;
  char UserInfo;
  DeviceAddress TempAddress;
  
  void setup() 
  {
    // Digital temperature probes initialisation
     Serial.begin(9600);
     sensors.begin();
     NumberOfDevices = sensors.getDeviceCount(); 
     
     for(int i=0;i<NumberOfDevices; i++)
     {
       // Search the wire for address
       if(sensors.getAddress(TempAddress, i))
       {
  			sensors.setResolution(TempAddress, TEMPERATURE_PRECISION);
       }
     }  

	 // Start Air Pump
	 PinMode(1, OUTPUT);
	 DigitalWrite(1, LOW);
  }
  
  void loop() 
  {
	// Ask to add the correct amount of water and wait confirmation
	UserInfo = "Add" + WaterAmount + "liters and press enter to confirm";
	Serial.Print(UserInfo);
	WaitConfirmation()

    // Take reading
  }
  
  void WaitConfirmation()
  {
	  while (Serial.Available() <= 0)
	  {
		  Delay(100);
	  }
  }
    
   
