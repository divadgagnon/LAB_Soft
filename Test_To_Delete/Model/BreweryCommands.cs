using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using SerialComm;
using SerialComm.PacketEncoder;
using GalaSoft.MvvmLight;
using LAB.Model;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

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

        // Internal variables
        private int Cycles;

        // Timer References
        DispatcherTimer DisconnectTimer;
        DispatcherTimer PrimingTimer1 = new DispatcherTimer();
        DispatcherTimer PrimingTimer2 = new DispatcherTimer();
        DispatcherTimer PWMTimer;

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
            PWMTimer = new DispatcherTimer();
            PWMTimer.Interval = TimeSpan.FromSeconds(hardwareSettings.PIDTimerInterval);
            PWMTimer.Tick += PWMTimer_Tick;
            PWMTimer.Start();
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

        public void UpdateSensors()
        {
            arduino.UpdateSensors(hardwareSettings.HLT_Vol_Pin, hardwareSettings.BK_Vol_Pin);
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
            }
            else if (Vessel.Temp.Value >= Target && Vessel.Burner.IsOn)
            {
                LightBurner(Vessel.Name, RelayState.Off);
            }

            if (Vessel.Temp.Value >= Target - ProcessSettings.TempHoldingRange && Vessel.Temp.Value <= Target + ProcessSettings.TempHoldingRange)
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

        // Hold Temperature of any vessel using a PID loop
        public void HoldTempPID(Brewery.vessel Vessel, double Target)
        {
            // Set the Target Set Point to the Vessel Instance
            if (Target == 0)
            {
                // Turn burner off
                LightBurner(Vessel.Name, RelayState.Off);
                Vessel.Temp.SetPoint = Target;
                Vessel.Temp.SetPointReached = true;
                Vessel.Temp.LastError = 0;
                goto MessageUpdates;
            }
            else
            {
                Vessel.Temp.SetPoint = Target;
            }

            // Calculate Errors
            double DutyCycle = Vessel.Burner.DutyCycle;
            double Error = Vessel.Temp.SetPoint - Vessel.Temp.Value;
            double LastError = Vessel.Temp.LastError;
            double ErrorD = 0;
            double ErrorI = Vessel.Temp.ErrorIntegral;
            if (DutyCycle < 1 && Error < 2) { ErrorI = ErrorI + Error; }
            double DebugErrorDerivative = 0;

            // Activate Derivative term only when error array is populated
            if (Vessel.Temp.ErrorDerivative.Count == hardwareSettings.ErrorDerivativeCount)
            {
                ErrorD = (Error - Vessel.Temp.ErrorDerivative[0]) / hardwareSettings.ErrorDerivativeCount;
                DebugErrorDerivative = Vessel.Temp.ErrorDerivative[0];
            }
            
            // Check if target temp is reached
            if(Vessel.Temp.Value >= Vessel.Temp.SetPoint-ProcessSettings.TempHoldingRange && Vessel.Temp.Value <= Vessel.Temp.SetPoint+ProcessSettings.TempHoldingRange )
            {
                Vessel.Temp.SetPointReached = true;
            }
            else
            {
                Vessel.Temp.SetPointReached = false;
            }

            // Set Burner Timer PWM
            double Proportionnal = Error * hardwareSettings.Kp;
            double Derivative = ErrorD * hardwareSettings.Kd;
            double Integral = ErrorI * hardwareSettings.Ki;

            // Set Saturation limits for integral term
            if (Integral > 1) { Integral = 1; }
            if(Integral < 0) { Integral = 0; }

            DutyCycle = Proportionnal + Integral + Derivative;

            // Controller Saturation
            if(DutyCycle < 0) { DutyCycle = 0; }
            if(DutyCycle > 1) { DutyCycle = 1; }
          
            // Burner control according to actual Duty Cycle
            if (Vessel.Temp.Value <= Vessel.Temp.SetPoint)
            {
                if (Cycles <= Math.Round(DutyCycle * 10))
                {
                    if (!Vessel.Burner.IsOn) { LightBurner(Vessel.Name, RelayState.On); }
                }
                else
                {
                    if (Vessel.Burner.IsOn) { LightBurner(Vessel.Name, RelayState.Off); }
                }
            }
            else if (Vessel.Burner.IsOn)
            {
                LightBurner(Vessel.Name, RelayState.Off);
            }

            // DEBUG
            System.Diagnostics.Debug.WriteLine("P : " + Proportionnal);
            System.Diagnostics.Debug.WriteLine("I : " + Integral);
            System.Diagnostics.Debug.WriteLine("D : " + Derivative);
            System.Diagnostics.Debug.WriteLine("Current Cycle : " + Cycles);

            // Assign LastError values before sending back the brewery instances
            Vessel.Temp.LastError = Error;
            Vessel.Temp.ErrorIntegral = ErrorI;
            Vessel.Burner.DutyCycle = DutyCycle;

            // Copy Back the set point info to the brewery instance and send the corresponding update message
            MessageUpdates:
                if (Vessel.Name == Vessels.HLT)
                {
                    brewery.HLT = Vessel;
                    Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointReachedUpdate");
                    Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointUpdate");
                    Messenger.Default.Send<Brewery>(brewery, "HLTPIDUpdate");
                }
                else if (Vessel.Name == Vessels.MLT)
                {
                    brewery.MLT = Vessel;
                    Messenger.Default.Send<Brewery>(brewery, "MLTTempSetPointReachedUpdate");
                    Messenger.Default.Send<Brewery>(brewery, "MLTTempSetPointUpdate");
                    Messenger.Default.Send<Brewery>(brewery, "MLTPIDUpdate");
            }
                else
                {
                    brewery.BK = Vessel;
                    Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointReachedUpdate");
                    Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointUpdate");
                    Messenger.Default.Send<Brewery>(brewery, "BKPIDUpdate");
            }
            }

        #region Timer Tick Events

        // Duty Cycle Calculation
        private void PWMTimer_Tick(object sender, EventArgs e)
        {
            // Update Cycles Counter
            if (Cycles >= 10)
            {
                Cycles = 0;
            }
            Cycles++;
        }

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
                    brewery.HLT.Volume.Value = (volSensor.Value - 72.575)/ 6.6038;
                    if (brewery.HLT.Volume.Value < 0) { brewery.HLT.Volume.Value = 0; }
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
                    brewery.BK.Volume.Value = (volSensor.Value - 75.833) / 6.281;
                    if(brewery.BK.Volume.Value < 0) { brewery.BK.Volume.Value = 0; }
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
                Messenger.Default.Unregister<AnalogReturn>(this, "VolumeUpdate");
                Messenger.Default.Unregister<Probes>(this, "TemperatureUpdate");
            }
            else
            {
                Messenger.Default.Register<Probes>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
                Messenger.Default.Register<AnalogReturn>(this, "VolumeUpdate", VolumeUpdate_MessageReceived);
                Messenger.Default.Unregister<Brewery>(this, "DebugTemperatureUpdate");
                Messenger.Default.Unregister<Brewery>(this, "DebugTemperatureUpdate");
            }
        }

        private void DesignModeTempUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.Value = _brewery.HLT.Temp.Value;
            brewery.MLT.Temp.Value = _brewery.MLT.Temp.Value;
            brewery.BK.Temp.Value = _brewery.BK.Temp.Value;

            // Send temperature to main view model
            Messenger.Default.Send<Brewery>(brewery, "TemperatureUpdate");
        }

        private void DesignModeVolumeUpdate_MessageReceived(Brewery _brewery)
        {
            if (_brewery.HLT.Volume.Value != 500) { brewery.HLT.Volume.Value = _brewery.HLT.Volume.Value; }
            if (_brewery.BK.Volume.Value != 500) { brewery.BK.Volume.Value = _brewery.BK.Volume.Value; }

            // Send Volumes to Main view model
            Messenger.Default.Send<Brewery>(brewery, "VolumeUpdate");
        }

    }

    #endregion


}
