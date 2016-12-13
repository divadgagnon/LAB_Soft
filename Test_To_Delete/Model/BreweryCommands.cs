using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using SerialComm;
using SerialComm.PacketEncoder;

namespace LAB.Model
{
    public class BreweryCommands
    {
        // Internal Model References
        private ArduinoCommands arduino;
        private HardwareSettings hardwareSettings;
        private Brewery brewery;
        private LSPProtocol device;
        private Process.settings ProcessSettings;

        // Timer References
        DispatcherTimer DisconnectTimer;

        // Multiple packet send avoidance variables
        bool BurnerCommandSent;
        bool PumpCommandSent;
        bool Pump2CommandSent;
        bool AirPump1CommandSent;
        bool AirPump2CommandSent;

        public BreweryCommands()
        {
            // Initializing Model References
            arduino = new ArduinoCommands(device = new LSPProtocol(new SimplePacketProtocolPacketEncoder()));
            hardwareSettings = new HardwareSettings();
            brewery = new Brewery();
            ProcessSettings = new Process.settings();

            // Registering to Messenger Notifications
            Messenger.Default.Register<AnalogReturn>(this, "VolumeUpdate", VolumeUpdate_MessageReceived);
            Messenger.Default.Register<DigitalReturn>(this, DigitalReturn_MessageReceived);
            Messenger.Default.Register<HardwareSettings>(this, "HardwareSettingsUpdate", UpdateHardwareSettings);
            Messenger.Default.Register<Probes>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
            Messenger.Default.Register <bool>(this, "DesignMode", DesignMode_MessageReceived);

            // Initializing Timers
            DisconnectTimer = new DispatcherTimer();
        }

        private void UpdateHardwareSettings(HardwareSettings _hardwareSettings)
        {
            // Update the hardware settings
            string temp = hardwareSettings.comPort;
            hardwareSettings = _hardwareSettings;
            hardwareSettings.comPort = temp;

            // Reset pin modes if brewery is connected
            if (brewery.IsConnected)
            {
                SetPinModes();
            }
        }

        // Connect to the brewery system
        public void Connect(string comPort)
        {
            arduino.Open(comPort);
            arduino.Ping();
        }

        // Disconnect and close port command
        public void FinalDisconnect()
        {
            brewery.IsConnected = false;
            arduino.ClosePort();
            Messenger.Default.Send<Brewery>(brewery, "ConnectionUpdate");
        }

        // Turn all relays off before disconnect
        public void PreDisconnect()
        {
            SetPinModes();
            DisconnectTimer.Interval = TimeSpan.FromMilliseconds(500);
            DisconnectTimer.Tick += DisconnectTimer_Tick;
            DisconnectTimer.Start();
        }

        // Set all the pin modes
        public void SetPinModes()
        {
            arduino.SetPinMode(hardwareSettings.AirPump1_Pin, PinModes.Output);
            arduino.SetPinMode(hardwareSettings.AirPump2_Pin, PinModes.Output);
            arduino.SetPinMode(hardwareSettings.BK_Burner_Pin, PinModes.Output);
            arduino.SetPinMode(hardwareSettings.HLT_Burner_Pin, PinModes.Output);
            arduino.SetPinMode(hardwareSettings.MLT_Burner_Pin, PinModes.Output);
            arduino.SetPinMode(hardwareSettings.Pump1_Pin, PinModes.Output);
            arduino.SetPinMode(hardwareSettings.Pump2_Pin, PinModes.Output);
        }

        // Update Temp sensors
        public void UpdateTempSensors()
        {
            arduino.GetTemperatures();
        }

        // Update Vol Sensors
        public void UpdateVolSensors()
        {
            arduino.AnalogRead(hardwareSettings.HLT_Vol_Pin);
            arduino.AnalogRead(hardwareSettings.BK_Vol_Pin); 
        }

        // Get a list of connected sensor probes
        public void GetConnectedProbes()
        {
            arduino.GetProbeAddress();
        }

        // Turn a burner On or Off
        public void LightBurner(Vessels vessel, RelayState State)
        {
            if(BurnerCommandSent) { return; }

            int BurnerPin;

            if (vessel == Vessels.HLT) { BurnerPin = hardwareSettings.HLT_Burner_Pin; }
            else if(vessel == Vessels.MLT) { BurnerPin = hardwareSettings.MLT_Burner_Pin; }
            else { BurnerPin = hardwareSettings.BK_Burner_Pin; }
            BurnerCommandSent = true;

            arduino.DigitalWrite(BurnerPin, State);
        }

        // Turn pump On or Off
        public void ActivatePump1(RelayState State)
        {
            if(PumpCommandSent) { return; }
            PumpCommandSent = true;

            arduino.DigitalWrite(hardwareSettings.Pump1_Pin, State);
        }

        // Turn pump 2 On or Off
        public void ActivatePump2(RelayState State)
        {
            if(Pump2CommandSent) { return; }
            Pump2CommandSent = true;

            arduino.DigitalWrite(hardwareSettings.Pump2_Pin,State);
        }

        // Turn air pump 1 On or Off
        public void ActivateAirPump1(RelayState State)
        {
            if(AirPump1CommandSent) { return; }
            AirPump1CommandSent = true;

            arduino.DigitalWrite(hardwareSettings.AirPump1_Pin, State);
        }

        public void ActivateAirPump2(RelayState State)
        {
            if(AirPump2CommandSent) { return; }
            AirPump2CommandSent = true;

            arduino.DigitalWrite(hardwareSettings.AirPump2_Pin, State);
        }

        // Hold Temperature Set Point
        public void HoldTemp(Vessels vesselName, double Target)
        {
            // Creating a new vessel instance
            Brewery.vessel Vessel = new Brewery.vessel(vesselName);

            // Get the selected vessel instance
            if(vesselName == Vessels.HLT) { Vessel = brewery.HLT; }
            else if(vesselName == Vessels.MLT) { Vessel = brewery.MLT; }
            else { Vessel = brewery.BK; }

            // Set the Target Set Point to the Vessel Instance
            if (Target == 0)
            {
                // Turn burner off
                LightBurner(Vessel.Name, RelayState.Off);
                Vessel.Temp.SetPoint = Target;
                Vessel.Temp.SetPointReached = true;
                goto MessageUpdates;
            }
            else
            {
                Vessel.Temp.SetPoint = Target;
            }

            // Check if temp is within range and process burner action
            if (Vessel.Temp.Value <= Target - ProcessSettings.TempHoldingRange && !Vessel.Burner.IsOn)
            {
                LightBurner(Vessel.Name, RelayState.On);
                Vessel.Temp.SetPointReached = false;
            }
            else if (Vessel.Temp.Value >= Target + ProcessSettings.TempHoldingRange && Vessel.Burner.IsOn)
            {
                LightBurner(Vessel.Name, RelayState.Off);
                Vessel.Temp.SetPointReached = false;
            }
            else if(Vessel.Temp.Value >= Target - ProcessSettings.TempHoldingRange && Vessel.Temp.Value <= Target + ProcessSettings.TempHoldingRange)
            {
                Vessel.Temp.SetPointReached = true;
            }
            else
            {
                Vessel.Temp.SetPointReached = false;
            }

            // Copy Back the set point info to the brewery instance and send the corresponding update message
            MessageUpdates:
            if (vesselName == Vessels.HLT)
            {
                brewery.HLT = Vessel;
                Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointReachedUpdate");
                Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointUpdate");
            }
            else if (vesselName == Vessels.MLT)
            {
                brewery.MLT = Vessel;
                Messenger.Default.Send<Brewery>(brewery, "MLTTempSetPointReachedUpdate");
                Messenger.Default.Send<Brewery>(brewery, "MLTTempSetPointUpdate");
            }
            else
            {
                brewery.BK = Vessel;
                Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointReachedUpdate");
                Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointUpdate");
            }
            
        }

        #region Timer Tick Events

        // Disconnect Timer Definition
        private void DisconnectTimer_Tick(object sender, EventArgs e)
        {
            FinalDisconnect();
        }

        #endregion

        #region Message Received Handling

        // Temperature update received
        private void TemperatureUpdate_MessageReceived(Probes probes)
        {
            // Set HLT temperature
            if(hardwareSettings.HLT_Temp_Probe.Address == probes.Yellow.Address) { brewery.HLT.Temp.Value = probes.Yellow.Temp; }
            if(hardwareSettings.HLT_Temp_Probe.Address == probes.Pink.Address) { brewery.HLT.Temp.Value = probes.Pink.Temp; }
            if(hardwareSettings.HLT_Temp_Probe.Address == probes.Orange.Address) { brewery.HLT.Temp.Value = probes.Orange.Temp; }
            if(hardwareSettings.HLT_Temp_Probe.Address == probes.YellowOrange.Address) { brewery.HLT.Temp.Value = probes.YellowOrange.Temp; }
            if(hardwareSettings.HLT_Temp_Probe.Address == probes.YellowPink.Address) { brewery.HLT.Temp.Value = probes.YellowPink.Temp; }

            // Set MLT temperature
            if (hardwareSettings.MLT_Temp_Probe.Address == probes.Yellow.Address) { brewery.MLT.Temp.Value = probes.Yellow.Temp; }
            if (hardwareSettings.MLT_Temp_Probe.Address == probes.Pink.Address) { brewery.MLT.Temp.Value = probes.Pink.Temp; }
            if (hardwareSettings.MLT_Temp_Probe.Address == probes.Orange.Address) { brewery.MLT.Temp.Value = probes.Orange.Temp; }
            if (hardwareSettings.MLT_Temp_Probe.Address == probes.YellowOrange.Address) { brewery.MLT.Temp.Value = probes.YellowOrange.Temp; }
            if (hardwareSettings.MLT_Temp_Probe.Address == probes.YellowPink.Address) { brewery.MLT.Temp.Value = probes.YellowPink.Temp; }

            // Set BK temperature
            if (hardwareSettings.BK_Temp_Probe.Address == probes.Yellow.Address) { brewery.BK.Temp.Value = probes.Yellow.Temp; }
            if (hardwareSettings.BK_Temp_Probe.Address == probes.Pink.Address) { brewery.BK.Temp.Value = probes.Pink.Temp; }
            if (hardwareSettings.BK_Temp_Probe.Address == probes.Orange.Address) { brewery.BK.Temp.Value = probes.Orange.Temp; }
            if (hardwareSettings.BK_Temp_Probe.Address == probes.YellowOrange.Address) { brewery.BK.Temp.Value = probes.YellowOrange.Temp; }
            if (hardwareSettings.BK_Temp_Probe.Address == probes.YellowPink.Address) { brewery.BK.Temp.Value = probes.YellowPink.Temp; }

            // Send temperature to main view model
            Messenger.Default.Send<Brewery>(brewery, "TemperatureUpdate");
        }

        // Volume Update received
        private void VolumeUpdate_MessageReceived(AnalogReturn volSensor)
        {

            if(volSensor.OverByte) { volSensor.Value = volSensor.Value + 255; }

            if (hardwareSettings.HLT_Vol_Pin == volSensor.Pin)
            {
                if (brewery.CalibrationModeOn) { brewery.HLT.Volume.Value = volSensor.Value; }
                else
                {
                    brewery.HLT.Volume.Value = (volSensor.Value - 63.809)/6.4779;

                }
            }
            else
            {
                brewery.HLT.Volume.Value = 500;
            }

            if(hardwareSettings.BK_Vol_Pin == volSensor.Pin)
            {
                if (brewery.CalibrationModeOn) { brewery.BK.Volume.Value = volSensor.Value; }
                else
                {
                    brewery.BK.Volume.Value = (volSensor.Value - 63.809) / 6.4779;
                }
            }
            else
            {
                brewery.BK.Volume.Value = 500;
            }

            // Send Volumes to Main view model
            Messenger.Default.Send<Brewery>(brewery, "VolumeUpdate");
        }

        private void DigitalReturn_MessageReceived(DigitalReturn _returnPinState)
        {
            // Update the corresponding brewery property
            
            if(_returnPinState.Pin == hardwareSettings.HLT_Burner_Pin)
            {
                brewery.HLT.Burner.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "HLTBurnerUpdate");
                BurnerCommandSent = false;
                return;
            }

            if(_returnPinState.Pin == hardwareSettings.MLT_Burner_Pin)
            {
                brewery.MLT.Burner.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "MLTBurnerUpdate");
                BurnerCommandSent = false;
                return;
            }

            if(_returnPinState.Pin == hardwareSettings.BK_Burner_Pin)
            {
                brewery.BK.Burner.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "BKBurnerUpdate");
                BurnerCommandSent = false;
                return;
            }

            if(_returnPinState.Pin == hardwareSettings.Pump1_Pin)
            {
                brewery.Pump1.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "Pump1Update");
                PumpCommandSent = false;
                return;
            }

            if(_returnPinState.Pin == hardwareSettings.Pump2_Pin)
            {
                brewery.Pump2.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "Pump2Update");
                Pump2CommandSent = false;
            }

            if(_returnPinState.Pin == hardwareSettings.AirPump1_Pin)
            {
                brewery.AirPump1.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "AirPump1Update");
                AirPump1CommandSent = false;
            }

            if(_returnPinState.Pin == hardwareSettings.AirPump2_Pin)
            {
                brewery.AirPump2.IsOn = !_returnPinState.Value;
                Messenger.Default.Send<Brewery>(brewery, "AirPump2Update");
                AirPump2CommandSent = false;
            }
            
        }

        private void DesignMode_MessageReceived(bool _designMode)
        {
            if (_designMode)
            {
                Messenger.Default.Register<Brewery>(this, "DebugTemperatureUpdate", DesignModeTempUpdate_MessageReceived);
                Messenger.Default.Register<Brewery>(this, "DebugVolumeUpdate", DesignModeVolumeUpdate_MessageReceived);
            }
        }

        private void DesignModeTempUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.Value = _brewery.HLT.Temp.Value;
            brewery.MLT.Temp.Value = _brewery.MLT.Temp.Value;
            brewery.BK.Temp.Value = _brewery.BK.Temp.Value;
        }

        private void DesignModeVolumeUpdate_MessageReceived(Brewery _brewery)
        {
            if (_brewery.HLT.Volume.Value != 500) { brewery.HLT.Volume.Value = _brewery.HLT.Volume.Value; }
            if (_brewery.BK.Volume.Value != 500) { brewery.BK.Volume.Value = _brewery.BK.Volume.Value; }
        }

    }

    #endregion


}
