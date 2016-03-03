using GalaSoft.MvvmLight;
using LAB.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;

namespace LAB.ViewModel
{

    public class MainViewModel : ViewModelBase
    {

        #region Properties and Instances

        // Define Commands
        public RelayCommand ConnectionSetupClickCommand{get; private set;}
        public RelayCommand HardwareSetupClickCommand { get; private set; }
        public RelayCommand MainClosingCommand { get; private set; }
        public RelayCommand StartBrewSessionClickCommand { get; private set; }
        public RelayCommand LoadRecipeClickCommand { get; private set; }
        public RelayCommand DebugDesignSessionClickCommand { get; private set; }
        public RelayCommand AutomaticModeClickCommand { get; private set; }
        public RelayCommand SemiAutoModeClickCommand { get; private set; }
        public RelayCommand ManualModeClickCommand { get; private set; }
        public RelayCommand Pump1ClickCommand { get; private set; }
        public RelayCommand Pump2ClickCommand { get; private set; }

        // Define Model instance names
        public BreweryCommands breweryCommand;
        public HardwareSettings hardwareSettings;
        public Brewery brewery;
        public Process process;
        public Probes probes;
        public UserAlarm userAlarm;

        // Define Timers
        DispatcherTimer UpdateTempSensorTimer;
        DispatcherTimer UpdateVolSensorTimer;
        DispatcherTimer AlarmTimer;
        DispatcherTimer PrimingTimer;
        DispatcherTimer MashStepTimer;
        DispatcherTimer BoilTimer;

        // Define Media Player
        MediaPlayer Player;

        // Define State Variable
        BreweryState breweryState;

        // Define State Machine variables
        bool DesignMode;
        bool RecirculationConfirmed;
        bool MashStepComplete;
        bool FirstMashStep;
        bool FirstSparge;
        bool SpargeModeOn;
        bool BoilOverSent;
        bool BoilComplete;
        double HLTStartVolume;
        double MLTStartVolume;
        int step;
        TimeSpan RemainingTime;
        TimeSpan StepStartTime;
        TimeSpan StepEndTime;
        TimeSpan BoilStartTime;
        TimeSpan BoilEndTime;

        // Bindable List
        List<bool> automationModeChecked;

        // Define Property Names for RaiseProperTyChanged Events
        public const string ConnectionStatusPropertyName = "ConnectionStatus";
        public const string StartSessionButtonContentPropertyName = "StartSessionButtonContent";
        public const string HLT_Temp_DisplayPropertyName = "HLT_Temp_Display";
        public const string BreweryStateDisplayPropertyName = "BreweryStateDisplay";
        public const string AutomationModeCheckedPropertyName = "AutomationModeChecked";

        // Define Bindable Properties
        public string ConnectionStatus
        {
            get
            {
                if (brewery.IsConnected)
                {
                    return "Connection Status : Connected";

                }
                return "Connection Status : Disconnected";
            }

        }

        // Brewery State display in the lower status bar
        private string BreweryStatusString = "Brewery Status : ";
        public string BreweryStateDisplay
        { 
            get
            {
                Messenger.Default.Send<BreweryState>(breweryState);
                switch (breweryState)
                {
                    case BreweryState.HLT_Fill:
                        {
                            return BreweryStatusString + "HLT Fill";
                        }
                    case BreweryState.Strike_Heat:
                        {
                            return BreweryStatusString + "Strike Heating";
                        }
                    case BreweryState.Strike_Transfer:
                        {
                            return BreweryStatusString + "Strike Water Transfer";
                        }
                    case BreweryState.DoughIn:
                        {
                            return BreweryStatusString + "Dough In";
                        }
                    case BreweryState.Mash:
                        {
                            return BreweryStatusString + "Mashing";
                        }
                    case BreweryState.Sparge:
                        {
                            return BreweryStatusString + "Sparging";
                        }
                    case BreweryState.Boil:
                        {
                            return BreweryStatusString + "Boil";
                        }
                    case BreweryState.Chill:
                        {
                            return BreweryStatusString + "Chill";
                        }
                    case BreweryState.Fermenter_Transfer:
                        {
                            return BreweryStatusString + "Fermenter Transfer";
                        }
                    case BreweryState.Manual_Override:
                        {
                            if (brewery.AutomationMode == automationMode.Manual)
                            {
                                return BreweryStatusString + "Manual Override";
                            }
                            else
                            {
                                return BreweryStatusString + "Semi-Automatic Control";
                            }
                        }
                }

                return BreweryStatusString + "StandBy";
                
            }
        }

        // Start session menu item content
        public string StartSessionButtonContent
        {
            get
            {
                if(process.Session.IsStarted)
                {
                    return "Stop Session";
                }

                return "Start Session";
            }
        }

        // Automation Mode Menu item Checked
        public List<bool> AutomationModeChecked
        {
            get
            {
                return automationModeChecked;
            }
        }

        #endregion

        #region Constructor

        // MainViewModel Class Constructor
        public MainViewModel()
        {
            // Creating new instance of model
            breweryCommand = new BreweryCommands();
            hardwareSettings = new HardwareSettings();
            brewery = new Brewery();
            process = new Process();
            probes = new Probes();
            userAlarm = new UserAlarm();

            // Initializing UI variables
            automationModeChecked = new List<bool>(new bool[] { true, false, false });

            // Initializing RelayCommand Instances
            ConnectionSetupClickCommand = new RelayCommand(connectionSetupClickCommand);
            HardwareSetupClickCommand = new RelayCommand(hardwareSetupClickCommand);
            StartBrewSessionClickCommand = new RelayCommand(startBrewSessionClickCommand);
            MainClosingCommand = new RelayCommand(mainClosing);
            LoadRecipeClickCommand = new RelayCommand(loadRecipeClickCommand);
            DebugDesignSessionClickCommand = new RelayCommand(debugDesignClickCommand);
            AutomaticModeClickCommand = new RelayCommand(automaticModeClickCommand);
            SemiAutoModeClickCommand = new RelayCommand(semiAutomaticModeClickCommand);
            ManualModeClickCommand = new RelayCommand(manualModeClickCommand);

            // Initializing Timers
            UpdateTempSensorTimer = new DispatcherTimer();
            UpdateVolSensorTimer = new DispatcherTimer();
            AlarmTimer = new DispatcherTimer();
            PrimingTimer = new DispatcherTimer();
            MashStepTimer = new DispatcherTimer();
            BoilTimer = new DispatcherTimer();

            // Initializing Sound Player
            Player = new MediaPlayer();

            // Initializing Machine State Variables
            MashStepComplete = false;
            FirstMashStep = true;
            FirstSparge = true;
            SpargeModeOn = false;
            BoilOverSent = false;
            BoilComplete = false;
            StepStartTime = new TimeSpan();
            RemainingTime = new TimeSpan();
            StepEndTime = new TimeSpan();

            // Initializing the brewery State
            breweryState = BreweryState.StandBy;
            RaisePropertyChanged(BreweryStateDisplayPropertyName);

            // Initializing Messenger Registers
            Messenger.Default.Register<NotificationMessage>(this, "BreweryCommand", BreweryCommand_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "ConnectionUpdate" , ConnectionUpdate_MessageReceived);
            Messenger.Default.Register<HardwareSettings>(this, "HardwareSettingsUpdate", HardwareSettings_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "VolumeUpdate", VolumeUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTBurnerUpdate", HLTBurnerUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTBurnerUpdate", MLTBurnerUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKBurnerUpdate", BKBurnerUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "Pump1Update", Pump1Update_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "Pump2Update", Pump2Update_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "AirPump1Update", AirPump1Update_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "AirPump2Update", AirPump2Update_MessageReceived);
            Messenger.Default.Register<string>(this, "SelectedcomPort", SelectedcomPort_MessageReceived);
            Messenger.Default.Register<Process>(this, Process_MessageReceived);
            Messenger.Default.Register<Probes>(this, "GetConnectedProbes", GetConnectedProbes_MessageReceived);
            Messenger.Default.Register<UserAlarm>(this, "UserAlarmReturn", UserAlarmReturn_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTTempSetPointUpdate", HLTTempSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTTempSetPointUpdate", MLTTempSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKTempSetPointUpdate", BKTempSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTTempSetPointReachedUpdate", HLTTempSetPointReachedUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTTempSetPointReachedUpdate", MLTTempSetPointReachedUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKTempSetPointReachedUpdate", BKTempSetPointReachedUpdate_MessageReceived);
            Messenger.Default.Register<Brewery.vessel>(this, "BurnerOverride", BurnerOverride_MessageReceived);
        }

        #endregion

        #region Automated StateMachine

        private void RunAutoStateMachine()
        {
            switch(breweryState)
            {
                case BreweryState.StandBy:
                    {
                        if(process !=null)
                        {
                            // Set the fill and temp set point and send the data to the HLT view
                            brewery.HLT.Volume.SetPoint = process.Session.TotalWaterNeeded;
                            brewery.HLT.Temp.SetPoint = process.Strike.Temp;
                            brewery.HLT.Temp.SetPointReached = false;
                            brewery.HLT.Volume.SetPointReached = false;

                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointUpdate");
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointReachedUpdate");
                            Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointUpdate");
                            Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointReachedUpdate");

                            if(!userAlarm.MessageSent) { PlayAlarm("", false, true, true); }
                            if(userAlarm.ProceedIsPressed)
                            {
                                breweryState = BreweryState.HLT_Fill;
                                RaisePropertyChanged(BreweryStateDisplayPropertyName);
                                userAlarm = new UserAlarm();
                            }
                        }
                        break;
                    }

                case BreweryState.HLT_Fill:
                    {
                        // Send User Alarm message to update the UserAction display text
                        if(!userAlarm.MessageSent) { PlayAlarm("", false, false, true); }

                        // Set the fill set point and send the data to the HLT view
                        brewery.HLT.Volume.SetPoint = process.Session.TotalWaterNeeded;
                        brewery.HLT.Temp.SetPoint = process.Strike.Temp;

                        Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointUpdate");
                        Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointUpdate");

                        // Turn on air pump 1 for Level monitoring
                        if(!brewery.AirPump1.IsOn) { breweryCommand.ActivateAirPump1(RelayState.On); }

                        // Check if HLT volume reached SetPoint
                        if(brewery.HLT.Volume.Value >= brewery.HLT.Volume.SetPoint)
                        {
                            // Set the SetPointReached property and send the data to the HLT view
                            brewery.HLT.Volume.SetPointReached = true;
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointReachedUpdate");

                            // Go to Strike heat state
                            breweryState = BreweryState.Strike_Heat;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                        }

                        break;
                    }

                case BreweryState.Strike_Heat:
                    {
                        // Send Message to light the pilots
                        if (!userAlarm.MessageSent) { PlayAlarm("Pilot", true, true, true); }

                        // Check for user confirmation of pilots
                        if(userAlarm.ProceedIsPressed)
                        {
                            // Get to HLT Set point and Hold
                            breweryCommand.HoldTemp(Vessels.HLT, process.Strike.Temp);
                        }

                        // Check if temperature range is reached
                        if (brewery.HLT.Temp.SetPointReached)
                        {
                            // Change State
                            breweryState = BreweryState.Strike_Transfer;
                            brewery.MLT.Volume.SetPointReached = false;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();

                        }
                        break;
                    }

                case BreweryState.Strike_Transfer:
                    {
                        // Define SetPoint for initial MLT volume (dough in)
                        brewery.MLT.Volume.SetPoint = process.MashSteps[0].Volume;
                        Messenger.Default.Send<Brewery>(brewery, "MLTVolumeSetPointUpdate");
                        Messenger.Default.Send<Brewery>(brewery, "MLTVolumeSetPointReachedUpdate");

                        // Confirm the correct Valves operations are complete before starting the pump
                        if (!userAlarm.MessageSent) { PlayAlarm("Valves", true, true, true); }

                        // Hold Strike Temp
                        breweryCommand.HoldTemp(Vessels.HLT, process.Strike.Temp);

                        // If the user did not confirm, hold temperature and wait
                        if (!userAlarm.ProceedIsPressed)
                        {
                            return;
                        }

                        if (brewery.MLT.Volume.Value < brewery.MLT.Volume.SetPoint)
                        {
                            // Let a delay for pump priming if the pump is off and not primed
                            if (!brewery.Pump1.IsPrimed && !brewery.Pump1.IsOn && !brewery.Pump1.IsPriming)
                            {
                                StartPrimingDelay(1);
                                PlayAlarm("Priming", false, false, false);
                            }

                            // Else if pump is primed but off
                            else if (brewery.Pump1.IsPrimed && !brewery.Pump1.IsOn)
                            {
                                HLTStartVolume = brewery.HLT.Volume.Value;
                                breweryCommand.ActivatePump1(RelayState.On);
                                PlayAlarm("Transfering", false, false, false);
                            }
                            // Update the transfered volume
                            else if(brewery.Pump1.IsOn)
                            {
                                brewery.MLT.Volume.Value = HLTStartVolume - brewery.HLT.Volume.Value;
                                Messenger.Default.Send<Brewery>(brewery, "MLTVolumeUpdate");
                            }
                        }
                        else
                        {
                            // Stop the transfer
                            if (brewery.Pump1.IsOn)
                            {
                                breweryCommand.ActivatePump1(RelayState.Off);
                            }

                            // Set the Volume Set point reached property
                            brewery.MLT.Volume.SetPointReached = true;
                            Messenger.Default.Send<Brewery>(brewery, "MLTVolumeSetPointReachedUpdate");

                            // Go to dough in state
                            breweryState = BreweryState.DoughIn;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                        }
                        break;
                    }

                case BreweryState.DoughIn:
                    {
                        // Ask user to set the valves in recirculation mode and play alarm sound to request user action
                        if (RecirculationConfirmed) { goto SkipRecirculationAlarm; }

                        if (!userAlarm.MessageSent)
                        {
                            PlayAlarm("Recirculation", true, true, true);
                        }
                        else if(userAlarm.ProceedIsPressed)
                        {
                            RecirculationConfirmed = true;
                            userAlarm = new UserAlarm();
                        }

                    SkipRecirculationAlarm:

                        // Hold MLT temp at set point (dough in temp)
                        breweryCommand.HoldTemp(Vessels.MLT, process.Strike.Temp);

                        // Hold HLT temp at set point 
                        breweryCommand.HoldTemp(Vessels.HLT, process.Sparge.Temp);

                        // Check if the temperature is within range and set the SetPointReached property
                        if (brewery.MLT.Temp.SetPointReached)
                        {
                            // Ask user to dough in and confirm BEFORE putting the grains play alarm sound to request user action
                            if (!userAlarm.MessageSent && RecirculationConfirmed)
                            {
                                PlayAlarm("DoughIn", true, true, true);
                            }

                            // If user confirmed go to Mash step #1
                            if(userAlarm.ProceedIsPressed)
                            {
                                // Start the mash process
                                breweryCommand.LightBurner(Vessels.MLT, RelayState.Off);
                                breweryState = BreweryState.Mash;
                                RaisePropertyChanged(BreweryStateDisplayPropertyName);
                                userAlarm = new UserAlarm();
                            }
                        }
                        break;
                    }

                case BreweryState.Mash:
                    {
                        // If first iteration start mash timer
                        if(FirstMashStep)
                        {
                            // Get the step start and end time
                            StepStartTime = DateTime.Now.TimeOfDay;
                            StepEndTime = StepStartTime.Add(TimeSpan.FromMinutes(process.MashSteps[step].Time));

                            // Start the step timer
                            MashStepTimer.Interval = TimeSpan.FromMilliseconds(500);
                            MashStepTimer.Tick += MashStepTimer_Tick;
                            MashStepTimer.Start();
                            FirstMashStep = false;
                        }

                        // If the mash step is completed increment
                        if(MashStepComplete)
                        {
                            // Set the new mash step as incomplete
                            MashStepComplete = false;

                            // Check if all steps are completed
                            if (process.MashSteps.Count <= step+1)
                            {
                                breweryState = BreweryState.Sparge;
                                RaisePropertyChanged(BreweryStateDisplayPropertyName);
                                userAlarm = new UserAlarm();
                                return;
                            }

                            // Get the step start and end time
                            StepStartTime = DateTime.Now.TimeOfDay;
                            StepEndTime = StepStartTime.Add(TimeSpan.FromMinutes(process.MashSteps[step].Time));

                            // Start the step timer
                            MashStepTimer.Interval = TimeSpan.FromMilliseconds(500);
                            MashStepTimer.Tick += MashStepTimer_Tick;
                            MashStepTimer.Start();
                        }

                        // Hold HLT temp set point at mash step temp
                        breweryCommand.HoldTemp(Vessels.MLT, process.MashSteps[step].Temp);

                        // Hold HLT temp at sparge temp
                        breweryCommand.HoldTemp(Vessels.HLT, process.Sparge.Temp);

                        // Start recirculating
                        if(!brewery.Pump2.IsPrimed && !brewery.Pump2.IsPriming)
                        {
                            StartPrimingDelay(2);
                        }
                        else if(brewery.Pump2.IsPrimed && !brewery.Pump2.IsOn)
                        {
                            breweryCommand.ActivatePump2(RelayState.On);
                            breweryCommand.ActivateAirPump2(RelayState.On);
                        }

                        break;
                    }

                case BreweryState.Sparge:
                    {
                        // Send Sparging Message
                        if(!userAlarm.MessageSent && !SpargeModeOn)
                        {
                            PlayAlarm("ValveSpargeMode", false, false, true);
                            breweryCommand.ActivatePump2(RelayState.Off);
                            SpargeModeOn = true;
                            userAlarm = new UserAlarm();
                        }

                        // Hold HLT at Sparge Temp
                        breweryCommand.HoldTemp(Vessels.HLT, process.Sparge.Temp);

                        // If HLT temp is in range then start the pumps
                        if(brewery.HLT.Temp.SetPointReached && userAlarm.ProceedIsPressed && (!brewery.Pump1.IsOn || !brewery.Pump2.IsOn) && FirstSparge)
                        {
                            // Save the HLT Start Volume and start the pumps
                            HLTStartVolume = brewery.HLT.Volume.Value;
                            breweryCommand.ActivatePump1(RelayState.On);
                            breweryCommand.ActivatePump2(RelayState.On);
                            FirstSparge = false;

                            // Turn off MLT Temp monitoring
                            breweryCommand.HoldTemp(Vessels.MLT, 0);

                            // Set The HLT Volume Set Point and save the MLT Start Volume
                            MLTStartVolume = brewery.MLT.Volume.Value;
                            brewery.HLT.Volume.SetPoint = HLTStartVolume - process.Sparge.Volume;
                            if (brewery.HLT.Volume.SetPoint < 0) { brewery.HLT.Volume.SetPoint = 0; }
                            brewery.HLT.Volume.SetPointReached = false;
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointUpdate");
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointReachedUpdate");

                            // Set the BK volume Set Point
                            brewery.BK.Volume.SetPoint = process.Boil.Volume;
                            brewery.BK.Volume.SetPointReached = false;
                            Messenger.Default.Send<Brewery>(brewery, "BKVolumeSetPointUpdate");
                            Messenger.Default.Send<Brewery>(brewery, "BKVolumeSetPointReachedUpdate");
                        }

                        // Update the MLT volume based on the HLT volume pumped out and set the Volume Set Points
                        brewery.MLT.Volume.Value = MLTStartVolume + HLTStartVolume - brewery.HLT.Volume.Value - brewery.BK.Volume.Value;
                        Messenger.Default.Send<Brewery>(brewery, "MLTVolumeUpdate");
                        
                        // Monitor HLT Sparge volume
                        if (brewery.HLT.Volume.Value <= HLTStartVolume-process.Sparge.Volume)
                        {
                            if (brewery.Pump1.IsOn)
                            {
                                breweryCommand.ActivatePump1(RelayState.Off);
                                breweryCommand.ActivateAirPump1(RelayState.Off);
                            }

                            if(brewery.HLT.Volume.Value <= 10 && brewery.HLT.Burner.IsOn)
                            {
                                breweryCommand.LightBurner(Vessels.HLT, RelayState.Off);
                                brewery.HLT.Temp.SetPointReached = false;
                                Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointReachedUpdate");
                            }

                            brewery.HLT.Volume.SetPointReached = true;
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointReachedUpdate");
                        }

                        // Check if sparge water is missing and send alarm
                        else if (brewery.HLT.Volume.SetPoint == 0 && brewery.HLT.Volume.Value == 0 && !userAlarm.MessageSent)
                        {
                            // Send alarm to indicate forced continue and initial water level was low
                            PlayAlarm("MissingSpargeWater", true, true, false, process.Sparge.Volume - HLTStartVolume);

                            if (brewery.Pump1.IsOn)
                            {
                                breweryCommand.ActivatePump1(RelayState.Off);
                            }

                            if (brewery.HLT.Volume.Value <= 10 && brewery.HLT.Burner.IsOn)
                            {
                                breweryCommand.LightBurner(Vessels.HLT, RelayState.Off);
                                brewery.HLT.Temp.SetPointReached = false;
                                Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointReachedUpdate");
                            }

                            brewery.HLT.Volume.SetPointReached = true;
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointReachedUpdate");
                        }

                        // Monitor BK Volume if over 10 start heating
                        if(brewery.BK.Volume.Value>= 10 && !brewery.BK.Burner.IsOn)
                        {
                            breweryCommand.HoldTemp(Vessels.BK, 100);
                        }

                        // Monitor Bk Volume for process target
                        if(brewery.BK.Volume.Value >= process.Boil.Volume)
                        {
                            if (brewery.Pump2.IsOn)
                            {
                                breweryCommand.ActivatePump2(RelayState.Off);
                            }

                            brewery.BK.Volume.SetPointReached = true;

                            // Switch to boil state
                            breweryState = BreweryState.Boil;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                            Messenger.Default.Send<Brewery>(brewery, "BKVolumeSetPointReachedUpdate");
                        }

                        break;
                    }

                case BreweryState.Boil:
                    {
                        // Hold boil temp in BK
                        breweryCommand.HoldTemp(Vessels.BK, 100);

                        // Monitor BK and send an alarm to warn boil over
                        if (brewery.BK.Temp.Value>=97 && !BoilOverSent)
                        {
                            // Send Boil Over Message
                            if (!userAlarm.MessageSent)
                            {
                                PlayAlarm("BoilOver", true, true, false);
                                BoilOverSent = true;
                                userAlarm = new UserAlarm();
                            }
                        }

                        // Boil is reached
                        if (brewery.BK.Temp.Value >= 99.5)
                        {
                            brewery.BK.Temp.BoilReached = true;
                        }

                        // Start Boil Timer if Boil is Reached
                        if(brewery.BK.Temp.BoilReached && !BoilTimer.IsEnabled && !BoilComplete)
                        { 
                            // Get the step start and end time
                            BoilStartTime = DateTime.Now.TimeOfDay;
                            BoilEndTime = StepStartTime.Add(TimeSpan.FromMinutes(process.Boil.Time));

                            // Start the step timer
                            BoilTimer.Interval = TimeSpan.FromMilliseconds(500);
                            BoilTimer.Tick += BoilTimer_Tick;
                            BoilTimer.Start();
                        }

                        // Check if boil is complete
                        if(BoilComplete)
                        {
                            // Turn off BK burner
                            breweryCommand.HoldTemp(Vessels.BK, 0);

                            // Switch to Chill State
                            breweryState = BreweryState.Chill;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                        }

                        break;
                    }

                case BreweryState.Chill:
                    {
                        // Send chill temperature info
                        if (!userAlarm.MessageSent)
                        {
                            PlayAlarm("Chill", true, true, false);
                            breweryState = BreweryState.Fermenter_Transfer;
                            userAlarm = new UserAlarm();
                        }
                        break;
                    }

                case BreweryState.Fermenter_Transfer:
                    { 
                        // Monitor BK Level
                        if(brewery.BK.Volume.Value == 0)
                        {
                            breweryCommand.ActivateAirPump2(RelayState.Off);
                            PlayAlarm("Ferment", false, false, false);
                            UpdateTempSensorTimer.Stop();
                            UpdateVolSensorTimer.Stop();
                            breweryCommand.PreDisconnect();
                        }

                        break;
                    }
            }
        }

        #endregion

        #region Manual Control Methods

        // Burner Control Command
        private void BurnerOverride_MessageReceived(Brewery.vessel Vessel)
        {
            if(brewery.AutomationMode != automationMode.Manual) { return; }
            
            if(Vessel.Burner.IsOn)
            {
                breweryCommand.LightBurner(Vessel.Name, RelayState.Off);
            }
            else
            {
                breweryCommand.LightBurner(Vessel.Name, RelayState.On);
            }
        }

        // Pump Control Commands

        private void Pump1Control()
        {
            if (brewery.AutomationMode != automationMode.Manual) { return; }

            if (brewery.Pump1.IsOn)
            {
                breweryCommand.ActivatePump1(RelayState.Off);
            }
            else
            {
                breweryCommand.ActivatePump1(RelayState.On);
            }
        }

        private void Pump2Control()
        {
            if (brewery.AutomationMode != automationMode.Manual) { return; }

            if (brewery.Pump2.IsOn)
            {
                breweryCommand.ActivatePump2(RelayState.Off);
            }
            else
            {
                breweryCommand.ActivatePump2(RelayState.On);
            }
        }

        // Air pump Control Commands

        private void AirPump1Control()
        {
            if(brewery.AutomationMode != automationMode.Manual) { return; }

            if(brewery.AirPump1.IsOn)
            {
                breweryCommand.ActivateAirPump1(RelayState.Off);
            }
            else
            {
                breweryCommand.ActivateAirPump1(RelayState.On);
            }
        }

        private void AirPump2Control()
        {
            if (brewery.AutomationMode != automationMode.Manual) { return; }

            if (brewery.AirPump2.IsOn)
            {
                breweryCommand.ActivateAirPump2(RelayState.Off);
            }
            else
            {
                breweryCommand.ActivateAirPump2(RelayState.On);
            }
        }

        #endregion

        #region StateMachine Secondary Methods

        private void PlayAlarm(string AlarmType, bool Audible, bool Visual, bool ProceedComfirmationRequired, double NumData = 0)
        {
            // Create a new alarm message
            userAlarm = new UserAlarm();
            userAlarm.CurrentState = breweryState;
            userAlarm.ProceedIsPressed = !ProceedComfirmationRequired;
            userAlarm.AlarmType = AlarmType;
            userAlarm.AudibleAlarm = Audible;
            userAlarm.VisualAlarm = Visual;
            userAlarm.ProcessData = process;
            userAlarm.NumData = NumData;
            userAlarm.MessageSent = true;
            userAlarm.IsActive = true;

            // Send Alarm message to UserAlarmViewModel
            Messenger.Default.Send<UserAlarm>(userAlarm);

            // Start the Audible Alarm Timer
            if (userAlarm.AudibleAlarm)
            {
                AlarmTimer.Tick += AlarmTimer_Tick;
                AlarmTimer.Interval = TimeSpan.FromMilliseconds(1000);
                AlarmTimer.Start();
            }
        }

        private void StopAlarm()
        {
            AlarmTimer.Stop();
        }

        private void StartPrimingDelay(int PumpNum)
        {
            if (PumpNum == 1)
            {
                brewery.Pump1.IsPriming = true;
                PrimingTimer.Tick += PrimingTimer1_Tick;
                PrimingTimer.Interval = TimeSpan.FromMilliseconds(hardwareSettings.PrimingDelay);
                PrimingTimer.Start();
            }
            else if(PumpNum == 2)
            {
                brewery.Pump2.IsPriming = true;
                PrimingTimer.Tick += PrimingTimer2_Tick;
                PrimingTimer.Interval = TimeSpan.FromMilliseconds(hardwareSettings.PrimingDelay);
                PrimingTimer.Start();
            }
            else { throw new Exception("Pump number is out of range, only two pumps in the system (1 and 2)"); }
        }

        #endregion

        #region Timer Tick Events

        private void UpdateTempSensorTimer_Tick(object sender, EventArgs e)
        {
            breweryCommand.UpdateTempSensors();
        }

        private void UpdateVolSensorTimer_Tick(object sender, EventArgs e)
        {
            breweryCommand.UpdateVolSensors();
        }

        int i = 0;
        private void AlarmTimer_Tick(object sender, EventArgs e)
        {
            Player.Open(new Uri(@"../../Media/Bleep.mp3", UriKind.Relative));
            Player.Play();
            i++;
            if (i == 5) { i = 0; StopAlarm(); }
        }

        private void PrimingTimer1_Tick(object sender, EventArgs e)
        {
            PrimingTimer.Stop();
            brewery.Pump1.IsPrimed = true;
            brewery.Pump1.IsPriming = false;
        }

        private void PrimingTimer2_Tick(object sender, EventArgs e)
        {
            PrimingTimer.Stop();
            brewery.Pump2.IsPrimed = true;
            brewery.Pump2.IsPriming = false;
        }

        private void MashStepTimer_Tick(object sender, EventArgs e)
        {
            // Increment ElapsedTime
            RemainingTime = StepEndTime - DateTime.Now.TimeOfDay;

            // Check if mash step is completed
            if (DateTime.Now.TimeOfDay >= StepEndTime)
            {
                MashStepComplete = true;
                RemainingTime = new TimeSpan();
                if(process.MashSteps.Count == step+1) { return; }
                step++;
            }

            // Send Mash step info
            userAlarm = new UserAlarm();
            userAlarm.ProcessData = process;
            userAlarm.VisualAlarm = false;
            userAlarm.CurrentState = breweryState;
            userAlarm.CurrentMashStep = step;
            userAlarm.RemainingTime = RemainingTime;
            userAlarm.IsActive = true;

            Messenger.Default.Send<UserAlarm>(userAlarm);
        }

        private void BoilTimer_Tick(object sender, EventArgs e)
        {
            // Increment ElapsedTime
            RemainingTime = BoilEndTime - DateTime.Now.TimeOfDay;

            // Check if Boil is Completed
            if(BoilEndTime <= DateTime.Now.TimeOfDay)
            {
                BoilComplete = true;
                BoilTimer.Stop();
                return;
            }

            // Send Boil Timer Info
            userAlarm = new UserAlarm();
            userAlarm.AlarmType = "BoilTimer";
            userAlarm.ProcessData = process;
            userAlarm.VisualAlarm = false;
            userAlarm.CurrentState = breweryState;
            userAlarm.RemainingTime = RemainingTime;
            userAlarm.IsActive = true;

            Messenger.Default.Send<UserAlarm>(userAlarm);
        }

        #endregion

        #region UI Methods

        // Starts a Brew Session when the MenuSubItem : Start Session is selected
        private void startBrewSessionClickCommand()
        {
            // Verify brewery connection
            if (!brewery.IsConnected)
            {
                process.Session.StartRequested = true;
                connectionSetupClickCommand();
                return;
            }

            // Verify Session Status
            if (process.Session.IsStarted)
            {
                process.Session.StartRequested = false;
                process.Session.IsStarted = false;
                RaisePropertyChanged(StartSessionButtonContentPropertyName);
                UpdateTempSensorTimer.Stop();
                UpdateVolSensorTimer.Stop();
                return;
            }

            // Get recipe file if not chosen already
            if (process.Session.TotalWaterNeeded == 0)
            {
                process.Session.StartRequested = true;
                loadRecipeClickCommand();
                return;
            }

            // Set Session properties
            process.Session.IsStarted = true;
            process.Session.StartRequested = false;
            RaisePropertyChanged(StartSessionButtonContentPropertyName);

            // Check if Design Session and skip the sensor update timers if true
            if (DesignMode) { goto SkipDAQTimers; }

            // Setting Temperature Timer Properties
            UpdateTempSensorTimer.Interval = TimeSpan.FromMilliseconds(hardwareSettings.TempRefreshRate);
            UpdateTempSensorTimer.Tick += UpdateTempSensorTimer_Tick;
            UpdateTempSensorTimer.Start();

            // Setting Volume Timer Properties
            UpdateVolSensorTimer.Interval = TimeSpan.FromMilliseconds(hardwareSettings.VolResfreshRate);
            UpdateVolSensorTimer.Tick += UpdateVolSensorTimer_Tick;
            UpdateVolSensorTimer.Start();

        SkipDAQTimers:

            // Verify the automation mode selected
            if((brewery.AutomationMode == automationMode.Manual) || (brewery.AutomationMode == automationMode.SemiAuto))
            {
                breweryState = BreweryState.Manual_Override;
                return;
            }

            // Run state Machine
            breweryState = BreweryState.StandBy;
            RunAutoStateMachine();
        }

        // Sends a message to get recipe xml file when the MenuSubItem : Load Recipe is selected
        private void loadRecipeClickCommand()
        {
            RecipeSetup recipe = new RecipeSetup();
            recipe.Load();
        }

        // Sends a message to open Connection Setup Window when the MenuSubItem : Connection Setup is selected
        private void connectionSetupClickCommand()
        {
            Messenger.Default.Send<NotificationMessageAction>(new NotificationMessageAction("OpenConnectionSetup", ConnectionSetup_CallBack), "WindowOperation");
        }

        // Sends a message to open Hardware Setup Window when tu MenuSubItem : Hardware Settings is selected
        private void hardwareSetupClickCommand()
        {
            Messenger.Default.Send<NotificationMessageAction>(new NotificationMessageAction("OpenHardwareSetup", HardwareSetup_CallBack), "WindowOperation");
        }

        // CallBack when Connection Setup is opened to update the button content
        private void ConnectionSetup_CallBack()
        {
            Messenger.Default.Send<bool>(brewery.IsConnected, "ConnectButtonContent");
        }

        // CallBack when Hardware Settings is opened
        private void HardwareSetup_CallBack()
        {
            if(brewery.IsConnected)
            {
                breweryCommand.GetConnectedProbes();
            }
        }

        private void automaticModeClickCommand()
        {
            automationModeChecked.Clear();
            automationModeChecked = new List<bool>(new bool[] { true, false, false });
            brewery.AutomationMode = automationMode.Automatic;
            RaisePropertyChanged(AutomationModeCheckedPropertyName);
        }

        private void semiAutomaticModeClickCommand()
        {
            automationModeChecked.Clear();
            automationModeChecked = new List<bool>(new bool[] { false, true, false });
            brewery.AutomationMode = automationMode.SemiAuto;
            RaisePropertyChanged(AutomationModeCheckedPropertyName);
        }

        private void manualModeClickCommand()
        {
            automationModeChecked.Clear();
            automationModeChecked = new List<bool>(new bool[] { false, false, true });
            brewery.AutomationMode = automationMode.Manual;
            RaisePropertyChanged(AutomationModeCheckedPropertyName);
        }

        // Gets executed before the application shuts down to close the comPort
        private void mainClosing()
        {
            if (brewery.IsConnected) { breweryCommand.PreDisconnect(); }
        }

        #endregion

        #region Received Message Handling

        // Get connected temperature probes
        private void GetConnectedProbes_MessageReceived(Probes _probes)
        {
            probes = _probes;
        }

        // Process update after recipe file is chosen
        private void Process_MessageReceived(Process _process)
        {
            process = _process;

            // Start Session if session start was already requested
            if(process.Session.StartRequested) { startBrewSessionClickCommand(); }
            
        }

        // comPort selected in ConnectionSetupWindow
        private void SelectedcomPort_MessageReceived(string _SelectedcomPort)
        {
            hardwareSettings.comPort = _SelectedcomPort;
        }

        // Hardware settings update
        private void HardwareSettings_MessageReceived(HardwareSettings _hardwareSettings)
        {
            string temp = hardwareSettings.comPort;
            hardwareSettings = _hardwareSettings;
            hardwareSettings.comPort = temp;
        }

        // Temperature Update
        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.Value = _brewery.HLT.Temp.Value;
            brewery.MLT.Temp.Value = _brewery.MLT.Temp.Value;
            brewery.BK.Temp.Value = _brewery.BK.Temp.Value;
            if (brewery.AutomationMode != automationMode.Automatic) { return; }
            RunAutoStateMachine();
        }

        // Volume Update
        private void VolumeUpdate_MessageReceived(Brewery _brewery)
        {
            if (_brewery.HLT.Volume.Value != 500) { brewery.HLT.Volume.Value = _brewery.HLT.Volume.Value; }
            if (_brewery.BK.Volume.Value != 500) { brewery.BK.Volume.Value = _brewery.BK.Volume.Value; }
            if (brewery.AutomationMode != automationMode.Automatic) { return; }
            RunAutoStateMachine();
        }

        // Connection State
        private void ConnectionUpdate_MessageReceived(Brewery _brewery)
        {
            if(_brewery.IsConnected != brewery.IsConnected)
            {
                brewery.IsConnected = _brewery.IsConnected;
                Messenger.Default.Send<bool>(brewery.IsConnected, "ConnectButtonContent");
                RaisePropertyChanged(ConnectionStatusPropertyName);
                if (process.Session.StartRequested) { startBrewSessionClickCommand(); }
                if(brewery.IsConnected)
                {
                    Messenger.Default.Send<NotificationMessageAction>(new NotificationMessageAction("CloseConnectionSetup",ConnectionSetupCloseCallback), "WindowOperation");
                    breweryCommand.SetPinModes();
                }
            }
        }

        // Connection Setup Automatic Close Callback
        private void ConnectionSetupCloseCallback()
        {
            // No code to execute
        }

        // Selected comPort
        private void BreweryCommand_MessageReceived(NotificationMessage msg)
        {
            if (hardwareSettings.comPort != null)
            {
                if (!brewery.IsConnected) { breweryCommand.Connect(hardwareSettings.comPort); return; }
                breweryCommand.PreDisconnect(); return;
            }

            MessageBox.Show("No Com Port was selected");

        }

        // HLT Burner Update
        private void HLTBurnerUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Burner.IsOn = _brewery.HLT.Burner.IsOn;
        }

        // MLT Burner Update
        private void MLTBurnerUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Burner.IsOn = _brewery.MLT.Burner.IsOn;
        }

        // BK Burner Update
        private void BKBurnerUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Burner.IsOn = _brewery.BK.Burner.IsOn;
        }

        // Pump 1 Update
        private void Pump1Update_MessageReceived(Brewery _brewery)
        {
            brewery.Pump1.IsOn = _brewery.Pump1.IsOn;
        }

        // Pump 2 Update
        private void Pump2Update_MessageReceived(Brewery _brewery)
        {
            brewery.Pump2.IsOn = _brewery.Pump2.IsOn;
        }

        // Air Pump 1 Update
        private void AirPump1Update_MessageReceived(Brewery _brewery)
        {
            brewery.AirPump1.IsOn = _brewery.AirPump1.IsOn;
        }

        // Air Pump 2 Update
        private void AirPump2Update_MessageReceived(Brewery _brewery)
        {
            brewery.AirPump2.IsOn = _brewery.AirPump2.IsOn;
        }

        // User Alarm Return
        private void UserAlarmReturn_MessageReceived(UserAlarm _userAlarm)
        {
            userAlarm.ProceedIsPressed = true;
        }

        // HLT Set Point Update
        private void HLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.SetPoint = _brewery.HLT.Temp.SetPoint;
        }

        // MLT Set Point Update
        private void MLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Temp.SetPoint = _brewery.MLT.Temp.SetPoint;
        }

        // BK Set Point Update
        private void BKTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Temp.SetPoint = _brewery.BK.Temp.SetPoint;
        }

        // HLT Set Point Reached Update
        private void HLTTempSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.SetPointReached = _brewery.HLT.Temp.SetPointReached;
        }

        // MLT Set Point Reached Update
        private void MLTTempSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Temp.SetPointReached = _brewery.MLT.Temp.SetPointReached;
        }

        // BK Set Point Reached Update
        private void BKTempSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Temp.SetPointReached = _brewery.BK.Temp.SetPointReached;
        }

        #endregion

        #region Debug

        private void debugDesignClickCommand()
        {
            // Open Debug Tool
            Messenger.Default.Send<NotificationMessageAction>(new NotificationMessageAction("OpenDebugTool", StartDebugSession), "WindowOperation");
        }

        private void StartDebugSession()
        {
            DesignMode = true;
            Messenger.Default.Send<bool>(DesignMode, "DesignMode");
            startBrewSessionClickCommand();
        }

        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}