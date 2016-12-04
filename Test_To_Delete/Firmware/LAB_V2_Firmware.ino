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
  }
  
  void loop() 
  {
    // put your main code here, to run repeatedly:
    CheckForCommand();
    delay(50);
  }
  
  void CheckForCommand()
  {
    // Define buffer variable as a byte array
    byte Buffer[256];
    
    if (Serial.available()>0)
    {
      delay(100);
      // Look for start of command Byte
      if (Serial.read()==0x55)
      {
        //Check how long the command is
        byte NumOfBytes = Serial.read();
        byte bcc = 0;
        
        // ReadBuffer
        for (int i=0x0;i<NumOfBytes;i++)
        {
          Buffer[i] = Serial.read();
          bcc ^= Buffer[i];
          delay(20);
        }
        
        //Save the end of command byte
        byte bccTest = Serial.read();
        // Test for command validity
        if (bccTest==bcc+1)
        {
          // Process the command received
          ProcessCommand(Buffer);
        }
        else
        {
          // Send communication error while reading packet
          byte ErrorBuffer[1] = {0xFF};
          SendPacket(ErrorBuffer, sizeof(ErrorBuffer));
        }
      }
    }
  }
  
  void ProcessCommand(byte Inbyte[])
  {
    byte OutBuffer[256];
    byte CommandNo = Inbyte[0];
    switch(CommandNo)
    {
      //----------------------------------------------
      //
      //
      //
      case 0x00:
      {

      }
      
      //----------------------------------------------
      //
      // Ping test command
      //
      case 0x01:
      {
        byte OutBuffer[2] = {0x01, 0x67};
        SendPacket(OutBuffer, sizeof(OutBuffer));
        break;
      }
      
      //----------------------------------------------
      //
      // digital pinmode configuration
      //
      case 0x02:
      {
        if(Inbyte[2]==0x00) 
        {
          pinMode(Inbyte[1],OUTPUT);
		  digitalWrite(Inbyte[1], HIGH);
        }
        if(Inbyte[2]==0x01) 
        {
          pinMode (Inbyte[1],INPUT);
        }
        else 
        {
          byte OutBuffer[2] = {0x02, 0xFF};
          SendPacket(OutBuffer, sizeof(OutBuffer));
          break;
        }
        byte OutBuffer[1] = {0x02};
        SendPacket(OutBuffer, sizeof(OutBuffer)); 
        break;
      }
      
      //---------------------------------------------
      //
      // Digital Read
      //
      case 0x03:
      {
        bool Val = digitalRead(Inbyte[1]);
        byte OutBuffer[3] = {0x03, Inbyte[1], (byte)Val};
        SendPacket(OutBuffer, sizeof(OutBuffer));
        break;
      }
      
      //-------------------------------------------
      //
      // Digital Write
      //
      case 0x04:
      {
        byte Val;
        if(Inbyte[2] == 0x00)
        {
            digitalWrite(Inbyte[1], LOW);
            Val = 0x00;
        }
        else
        {
            digitalWrite(Inbyte[1],HIGH);
            Val = 0x01;
        }
        
        byte OutBuffer[3] = {0x04, Inbyte[1], Val};
        SendPacket(OutBuffer, sizeof(OutBuffer));
        break;
      }
      
      //-------------------------------------------
      //
      // Analog Read
      //
      case 0x05:
      {
        int Val = analogRead(Inbyte[1])/2;
		bool Over = Val > 255;

		if (Over)
		{
			Val = Val - 255;
		}

        byte OutBuffer[4] = {0x05, Inbyte[1], Val, Over};
        SendPacket(OutBuffer, sizeof(OutBuffer));
		
        break;
      }
      
      //-------------------------------------------
      //
      //  DS18B20 get probe addresses
      //
      case 0x06:
      {
        int TempData;
        byte OutBuffer[NumberOfDevices+1];
        
        OutBuffer[0] = 0x06;
        
        for(int i=0;i<NumberOfDevices;i++)
        {
          if(sensors.getAddress(TempAddress,i))
          {
            OutBuffer[i+1]= TempAddress[7];
          }
        }
        SendPacket(OutBuffer, sizeof(OutBuffer));
        break;
      }
            
      //-------------------------------------------
      //
      //  Get temperatures of all probes
      //
      case 0x07:
      {
        sensors.requestTemperatures();
        delay(50);
        byte OutBuffer[NumberOfDevices*2+1];
        OutBuffer[0] = 0x07;
		int j = 0;
        
        for(int i=0;i<NumberOfDevices*2; i+=2)
        {
            // Search the wire for address
            if(sensors.getAddress(TempAddress, j))
   			{
               byte TempData = round(sensors.getTempC(TempAddress)/0.46875);
               OutBuffer[i+1] = TempAddress[7];
               OutBuffer[i+2] = TempData;
			   j+=1;
            }
         }
         
         SendPacket(OutBuffer, sizeof(OutBuffer));
         break;
      }
    }
    
    // ------------- Maintenance -------------
    
    // Clear Buffer
    for(int i=0;i<255;i++)
    {
      Inbyte[i]=0;
    }
  }
  
  // Command error method
  void CommandError()
  {
    byte ErrorBuffer[1] = {0xFF}; 
    Serial.write(ErrorBuffer, 4);  
  }
  
  // Create a packet start of command byte, length of command byte 
  // and bcc end of command byte and send packet
  void SendPacket(byte PacketData[256], int PacketDataSize)
  {
    byte Packet[256];
    Packet[0] = 0x55;
    byte bcc = 0;
    Packet[1] = (byte)PacketDataSize;

    for(int i=0;i<PacketDataSize;i++)
    {
      Packet[i+2] = PacketData[i];
      bcc ^= PacketData[i];
    }

    bcc = bcc+1;
    Packet[PacketDataSize+2] = bcc;
    Serial.write(Packet, PacketDataSize+3);
  }
