﻿using GalaSoft.MvvmLight;
using LAB.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
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
        public RelayCommand<Brewery.valve> ValveClickCommand { get; private set; }
        public RelayCommand AirPump1ClickCommand { get; private set; }
        public RelayCommand AirPump2ClickCommand { get; private set; }
        public RelayCommand HLTPlotButtonClickCommand { get; private set; }
        public RelayCommand MLTPlotButtonClickCommand { get; private set; }
        public RelayCommand BKPLotButtonClickCommand { get; private set; }

        // Define Model instance names
        public BreweryCommands breweryCommand;
        public HardwareSettings hardwareSettings;
        public Brewery brewery;
        public Process process;
        public Probes probes;
        public UserAlarm userAlarm;
        public Ingredients ingredients;

        // Define Timers
        DispatcherTimer UpdateTempSensorTimer;
        DispatcherTimer UpdateVolSensorTimer;
        DispatcherTimer UpdateSensorsTimer;
        DispatcherTimer AlarmTimer;
        DispatcherTimer PrimingTimer1;
        DispatcherTimer PrimingTimer2;
        DispatcherTimer MashStepTimer;
        DispatcherTimer BoilTimer;
        DispatcherTimer HopScheduleTimer;

        // Define Media Player
        MediaPlayer Player;

        // Define Brushes
        private SolidColorBrush OnColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF218512"));
        private SolidColorBrush OffColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8F0000"));

        // Define State Variable
        BreweryState breweryState;

        // Define State Machine variables
        bool DesignMode;
        bool HopAdditionComplete;
        bool FirstMashStep;
        bool FirstSparge;
        bool BoilOverSent;
        bool BoilComplete;
        double HLTStartVolume;
        double MLTStartVolume;
        int step;
        int CurrentHopAddition;
        int PrimingCount;
        TimeSpan RemainingTime;
        TimeSpan BoilRemainingTime;
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
        public const string AirPump1StatusPropertyName = "AirPump1Status";
        public const string AirPump2StatusPropertyName = "AirPump2Status";
        public const string MashHopTimerDisplayPropertyName = "MashHopTimerDisplay";
        public const string MashHopTimerDisplayTitlePropertyName = "MashHopTimerDisplayTitle";

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
                RaisePropertyChanged(MashHopTimerDisplayTitlePropertyName);
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
                    case BreweryState.CalibrationMode:
                        {
                            return BreweryStatusString + "Volume Sensor Calibration";
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
                if(brewery.SafeModeOn) { return "Resume Session"; }
                if(process.Session.IsStarted) { return "Stop Session"; }
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

        // Valve List property
        public ObservableCollection<Brewery.valve> ValveList
        {
            get
            {
                return brewery.Valves;
            }
        }

        // Air Pump 1 Status Indicator property
        public Brush AirPump1Status
        {
            get
            {
                if (brewery.AirPump1.IsOn) { return OnColor; }
                else { return OffColor; }
            }
        }

        // Air Pump 2 Status Indicator property
        public Brush AirPump2Status
        {
            get
            {
                if (brewery.AirPump2.IsOn) { return OnColor; }
                else { return OffColor; }
            }
        }

        // Mash and Hop Timer Title
        public string MashHopTimerDisplayTitle
        {
            get
            {
                if (breweryState == BreweryState.Sparge || 
                    breweryState == BreweryState.Boil || 
                    breweryState == BreweryState.Chill || 
                    breweryState == BreweryState.Fermenter_Transfer)
                {
                    return "Hop Schedule";
                }

                else { return "Mash Steps"; }
            }
        }

        // Mash steps timer display text property
        ObservableCollection<MashTimerDisplayItem> MashHopStepDisplayList = new ObservableCollection<MashTimerDisplayItem>();

        public ObservableCollection<MashTimerDisplayItem> MashHopTimerDisplay
        {
            get
            {
                MashHopStepDisplayList.Clear();

                if(breweryState == BreweryState.Mash)
                {
                    
                    foreach( Process.MashStep Step in process.MashSteps)
                    {
                        TimeSpan StepTimeSpan = TimeSpan.FromMinutes(Step.Time);

                        if (process.MashSteps.IndexOf(Step) < step) { }

                        else if (process.MashSteps[step] == Step)
                        {
                            MashHopStepDisplayList.Add(new MashTimerDisplayItem() { StepName = Step.Name, Time = RemainingTime.Minutes + ":" + String.Format("{0:00}", RemainingTime.Seconds) });
                        }
                        else
                        {
                            MashHopStepDisplayList.Add(new MashTimerDisplayItem() { StepName = Step.Name, Time = StepTimeSpan.Minutes + ":" + String.Format("{0:00}", StepTimeSpan.Seconds) });
                        }
                    }
                }

                else if(breweryState == BreweryState.Sparge || breweryState == BreweryState.Chill)
                {
                    foreach (Ingredients.Hop hop in ingredients.Hops)
                    {
                        TimeSpan hopBoilTime = TimeSpan.FromMinutes(hop.BoilTime);
                        MashHopStepDisplayList.Add(new MashTimerDisplayItem() { StepName = hop.Name, Time = hopBoilTime.Minutes + ":" + String.Format("{0:00}", hopBoilTime.Seconds) });
                    }
                }

                else if (breweryState == BreweryState.Boil && BoilTimer.IsEnabled && !BoilComplete)
                {
                    foreach(Ingredients.Hop hop in ingredients.Hops)
                    {
                        if (ingredients.Hops.IndexOf(hop) < CurrentHopAddition) { }

                        else
                        {
                            TimeSpan hopRemainingTime = BoilEndTime.Add(-TimeSpan.FromMinutes(hop.BoilTime)-DateTime.Now.TimeOfDay);
                            MashHopStepDisplayList.Add(new MashTimerDisplayItem() { StepName = hop.Name, Time = hopRemainingTime.Minutes + ":" + String.Format("{0:00}", hopRemainingTime.Seconds) });
                        }
                    }
                }

                else
                {
                    foreach (Process.MashStep Step in process.MashSteps)
                    {
                            TimeSpan StepTimeSpan = TimeSpan.FromMinutes(Step.Time);

                            MashHopStepDisplayList.Add(new MashTimerDisplayItem() { StepName = Step.Name, Time = StepTimeSpan.Minutes + ":" + String.Format("{0:00}", StepTimeSpan.Seconds) }); 
                    }
                }

                return MashHopStepDisplayList;
            }
        }

        public List<bool> PlotVisibility { get; set; } = new List<bool>(new bool[3] { true, false, false });

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
            ingredients = new Ingredients();

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
            ValveClickCommand = new RelayCommand<Brewery.valve>(valveClickCommand);
            AirPump1ClickCommand = new RelayCommand(airPump1ClickCommand);
            AirPump2ClickCommand = new RelayCommand(airPump2ClickCommand);
            HLTPlotButtonClickCommand = new RelayCommand(hltPlotButtonClickCommand);
            MLTPlotButtonClickCommand = new RelayCommand(mltPlotButtonClickCommand);
            BKPLotButtonClickCommand = new RelayCommand(bkPlotButtonClickCommand);

            // Initializing Timers
            UpdateTempSensorTimer = new DispatcherTimer();
            UpdateVolSensorTimer = new DispatcherTimer();
            UpdateSensorsTimer = new DispatcherTimer();
            AlarmTimer = new DispatcherTimer();
            PrimingTimer1 = new DispatcherTimer();
            PrimingTimer2 = new DispatcherTimer();
            MashStepTimer = new DispatcherTimer();
            BoilTimer = new DispatcherTimer();
            HopScheduleTimer = new DispatcherTimer();

            // Initializing Sound Player
            Player = new MediaPlayer();

            // Initializing Machine State Variables
            HopAdditionComplete = false;
            FirstMashStep = true;
            FirstSparge = true;
            BoilOverSent = false;
            BoilComplete = false;
            StepStartTime = new TimeSpan();
            RemainingTime = new TimeSpan();
            BoilRemainingTime = new TimeSpan();
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
            Messenger.Default.Register<Brewery>(this, "HLTPIDUpdate", HLTPIDUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTPIDUpdate", MLTPIDUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKPIDUpdate", BKPIDUpdate_MessageReceived);
            Messenger.Default.Register<Brewery.vessel>(this, "BurnerOverride", BurnerOverride_MessageReceived);
            Messenger.Default.Register<Brewery.pump>(this, "PumpOverride", PumpOverride_MessageReceived);
            Messenger.Default.Register<Brewery.valve>(this, ValveUpdate_MessageReceived);
            Messenger.Default.Register<Ingredients>(this, (Ingredients _ingredients) => { ingredients = _ingredients; });
            Messenger.Default.Register<bool>(this, "SafeModeUpdate", SafeModeUpdate_MessageReceived);

            // Get and set the saved hardware settings on startup
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this,""), "SetDefaultHardwareSettings");
        }

        #endregion

        #region Automated StateMachine

        private void RunAutoStateMachine()
        {

            if (brewery.SafeModeOn) { return; }

            switch(breweryState)
            {
                #region StandBy
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

                            breweryState = BreweryState.HLT_Fill;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                        }
                        break;
                    }
                #endregion

                #region HLT Fill
                case BreweryState.HLT_Fill:
                    {
                        // Set the fill set point and send the data to the HLT view
                        brewery.HLT.Volume.SetPoint = process.Session.TotalWaterNeeded;
                        brewery.HLT.Temp.SetPoint = process.Strike.Temp;

                        Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointUpdate");
                        Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointUpdate");

                        // Turn on air pump 1 for Level monitoring
                        if(!brewery.AirPump1.IsOn)
                        {
                            breweryCommand.ActivateAirPump1(RelayState.On);
                        }

                        // Send Message to light the pilots
                        if (!userAlarm.MessageSent) { PlayAlarm("Pilot", true, true, true); }

                        // Check for user confirmation of pilots
                        if (userAlarm.ProceedIsPressed && brewery.HLT.Volume.Value > 10)
                        {
                            // Get to HLT Set point and Hold
                            breweryCommand.HoldTempPID(brewery.HLT, process.Strike.Temp);
                        }

                        // Check if HLT volume reached SetPoint
                        if (brewery.HLT.Volume.Value >= brewery.HLT.Volume.SetPoint && userAlarm.ProceedIsPressed)
                        {
                            // Set the SetPointReached property and send the data to the HLT view
                            brewery.HLT.Volume.SetPointReached = true;
                            Messenger.Default.Send<Brewery>(brewery, "HLTVolumeSetPointReachedUpdate");

                            // Go to Strike heat state
                            breweryState = BreweryState.Strike_Heat;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                        }

                        break;
                    }
                #endregion

                #region Strike Heat
                case BreweryState.Strike_Heat:
                    {
                        // Get to HLT Set point and Hold
                        breweryCommand.HoldTempPID(brewery.HLT, process.Strike.Temp);

                        //Check if temperature range is reached
                        if (brewery.HLT.Temp.SetPointReached || brewery.HLT.Temp.Value > process.Strike.Temp)
                        {
                            // Set HLT Start Volume for MLT Volume calculation
                            HLTStartVolume = brewery.HLT.Volume.Value;

                            // Change State
                            breweryState = BreweryState.Strike_Transfer;
                            brewery.MLT.Volume.SetPointReached = false;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                        }
                        break;
                    }
                #endregion

                #region Strike Transfer
                case BreweryState.Strike_Transfer:
                    {
                        // Hold Strike Temp
                        breweryCommand.HoldTempPID(brewery.HLT, process.Strike.Temp);

                        // Define SetPoint for initial MLT volume (dough in)
                        brewery.MLT.Volume.SetPoint = process.MashSteps[0].Volume;
                        Messenger.Default.Send<Brewery>(brewery, "MLTVolumeSetPointUpdate");
                        Messenger.Default.Send<Brewery>(brewery, "MLTVolumeSetPointReachedUpdate");

                        //Confirm the correct Valves operations are complete before starting the pump
                        if (!(brewery.ValveConfig.ConfigSet == ValveConfigs.Strike_Transfer))
                        {
                            brewery.ValveConfig.Set(ValveConfigs.Strike_Transfer, brewery.Valves);
                            brewery.ValveConfig.ConfigSet = ValveConfigs.Strike_Transfer;
                        }

                        // If the user did not set the correct valve configuration, hold temperature and wait
                        if (!brewery.ValveConfig.Check(ValveConfigs.Strike_Transfer, brewery.Valves))
                        {
                            return;
                        }

                        if (brewery.MLT.Volume.Value < brewery.MLT.Volume.SetPoint)
                        {
                            // Execute pump priming sequence if the pump is off and not primed
                            if (!brewery.Pump1.IsPrimed && !brewery.Pump1.IsOn && !brewery.Pump1.IsPriming)
                            {
                                PrimingTimer1.Interval = TimeSpan.FromSeconds(1.5);
                                PrimingTimer1.Tick += PrimingTimer1_Tick;
                                PrimingTimer1.Start();

                                breweryCommand.ActivatePump1(RelayState.On);

                                brewery.Pump1.IsPriming = true;
                                Messenger.Default.Send<Brewery.pump>(brewery.Pump1, "PrimingUpdate");
                            }

                            // Else if pump is primed but off
                            else if (brewery.Pump1.IsPrimed && !brewery.Pump1.IsOn)
                            {
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
                #endregion

                #region Dough In
                case BreweryState.DoughIn:
                    {
                        // Set the Valve configuration request to Mash Recirculation
                        if (!(brewery.ValveConfig.ConfigSet == ValveConfigs.Mash_Recirc))
                        {
                            brewery.ValveConfig.Set(ValveConfigs.Mash_Recirc, brewery.Valves);
                            brewery.ValveConfig.ConfigSet = ValveConfigs.Mash_Recirc;
                        }

                        // Hold MLT temp at set point (dough in temp)
                        breweryCommand.HoldTempPID(brewery.MLT, process.Strike.Temp);

                        // Hold HLT temp at set point 
                        breweryCommand.HoldTempPID(brewery.HLT, process.Sparge.Temp);

                        // Check if the valves are in the requested position
                        if (!(brewery.ValveConfig.Check(ValveConfigs.Mash_Recirc, brewery.Valves))) { return; }

                        // Check if the temperature is within range and set the SetPointReached property
                        if (brewery.MLT.Temp.SetPointReached)
                        {
                            // Stop recirculating if needed
                            if(brewery.Pump2.IsOn) { breweryCommand.ActivatePump2(RelayState.Off); }
                            
                            // Ask user to dough in and confirm BEFORE putting the grains play alarm sound to request user action
                            if (!userAlarm.MessageSent)
                            {
                                PlayAlarm("DoughIn", true, true, true);
                            }
                        }
                        else if (brewery.MLT.Temp.Value > brewery.MLT.Temp.SetPoint)
                        {
                            // Start recirculating
                            if (!brewery.Pump2.IsPrimed && !brewery.Pump2.IsOn && !brewery.Pump2.IsPriming)
                            {
                                PrimingTimer2.Interval = TimeSpan.FromSeconds(1.5);
                                PrimingTimer2.Tick += PrimingTimer2_Tick;
                                PrimingTimer2.Start();

                                breweryCommand.ActivatePump2(RelayState.On);

                                brewery.Pump2.IsPriming = true;
                                Messenger.Default.Send<Brewery.pump>(brewery.Pump2, "PrimingUpdate");
                            }
                        }

                        // If user confirmed go to Mash step #1
                        if (userAlarm.ProceedIsPressed)
                        {
                            // Start the mash process
                            breweryCommand.LightBurner(Vessels.MLT, RelayState.Off);
                            breweryState = BreweryState.Mash;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                        }

                        break;
                    }
                #endregion

                #region Mash
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
                        if(HopAdditionComplete)
                        {
                            // Set the new mash step as incomplete
                            HopAdditionComplete = false;

                            // Check if all steps are completed
                            if (process.MashSteps.Count < step+1)
                            {
                                breweryCommand.ActivatePump2(RelayState.Off);
                                breweryState = BreweryState.Sparge;
                                RaisePropertyChanged(BreweryStateDisplayPropertyName);
                                userAlarm = new UserAlarm();
                                return;
                            }

                            // Get the step start and end time
                            StepStartTime = DateTime.Now.TimeOfDay;
                            StepEndTime = StepStartTime.Add(TimeSpan.FromMinutes(process.MashSteps[step].Time));

                            // Start the step timer
                            MashStepTimer.Interval = TimeSpan.FromMilliseconds(250);
                            MashStepTimer.Tick += MashStepTimer_Tick;
                            MashStepTimer.Start();
                        }

                        // Hold HLT temp set point at mash step temp
                        breweryCommand.HoldTempPID(brewery.MLT, process.MashSteps[step].Temp);

                        // Hold HLT temp at sparge temp
                        breweryCommand.HoldTempPID(brewery.HLT, process.Sparge.Temp);

                        // Start recirculating
                        if (!brewery.Pump2.IsPrimed && !brewery.Pump2.IsOn && !brewery.Pump2.IsPriming)
                        {
                            PrimingTimer2.Interval = TimeSpan.FromSeconds(1.5);
                            PrimingTimer2.Tick += PrimingTimer2_Tick;
                            PrimingTimer2.Start();

                            breweryCommand.ActivatePump2(RelayState.On);

                            brewery.Pump2.IsPriming = true;
                            Messenger.Default.Send<Brewery.pump>(brewery.Pump2, "PrimingUpdate");
                        }

                        break;
                    }
                #endregion

                #region Sparge
                case BreweryState.Sparge:
                    {

                        // Activate AirPump 2 if not activated

                        if (!brewery.AirPump2.IsOn)
                        {
                            breweryCommand.ActivateAirPump2(RelayState.On);
                        }

                        // Send Sparging Message
                        if (brewery.ValveConfig.ConfigSet != ValveConfigs.Fly_Sparge)
                        {
                            brewery.ValveConfig.Set(ValveConfigs.Fly_Sparge, brewery.Valves);
                            brewery.ValveConfig.ConfigSet = ValveConfigs.Fly_Sparge;
                            breweryCommand.ActivatePump2(RelayState.On);
                            userAlarm = new UserAlarm();
                        }

                        // Hold HLT at Sparge Temp
                        if (brewery.HLT.Volume.Value > 10)
                        {
                            breweryCommand.HoldTempPID(brewery.HLT, process.Sparge.Temp);
                        }
                        if (brewery.HLT.Volume.Value <= 10 && brewery.HLT.Burner.IsOn)
                        {
                            // Turn off HLT temp monitoring
                            breweryCommand.HoldTempPID(brewery.HLT, 0);
                        }

                        // Check if valve config is confirmed
                        if (!brewery.ValveConfig.Check(ValveConfigs.Fly_Sparge, brewery.Valves)) { return; }

                        // If HLT temp is in range then start the pumps
                        if(brewery.HLT.Temp.SetPointReached && FirstSparge)
                        {
                            // Save the HLT Start Volume and start the pumps
                            HLTStartVolume = brewery.HLT.Volume.Value;
                            breweryCommand.ActivatePump1(RelayState.On);
                            breweryCommand.ActivatePump2(RelayState.On);
                            FirstSparge = false;

                            // Turn off MLT Temp monitoring
                            breweryCommand.HoldTempPID(brewery.MLT, 0);

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
                            breweryCommand.ActivateAirPump1(RelayState.Off);

                            if (brewery.Pump1.IsOn)
                            {
                                breweryCommand.ActivatePump1(RelayState.Off);
                            }

                            if (brewery.HLT.Volume.Value <= 10)
                            {
                                breweryCommand.HoldTemp(Vessels.HLT, 0);
                                //brewery.HLT.Temp.SetPointReached = true;
                                //Messenger.Default.Send<Brewery>(brewery, "HLTTempSetPointReachedUpdate");
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
                            breweryCommand.HoldTempPID(brewery.BK, 100);
                        }

                        // Monitor Bk Volume for process target
                        if(brewery.BK.Volume.Value >= process.Boil.Volume)
                        {
                            breweryCommand.ActivatePump2(RelayState.Off);
                            brewery.BK.Volume.SetPointReached = true;

                            // Switch to boil state
                            breweryState = BreweryState.Boil;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                            Messenger.Default.Send<Brewery>(brewery, "BKVolumeSetPointReachedUpdate");
                        }

                        break;
                    }
                #endregion

                #region Boil
                case BreweryState.Boil:
                    {
                        // Hold boil temp in BK
                        breweryCommand.HoldTempPID(brewery.BK, 100);

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
                        if (brewery.BK.Temp.Value >= 99)
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
                            BoilTimer.Interval = TimeSpan.FromMilliseconds(250);
                            BoilTimer.Tick += BoilTimer_Tick;
                            BoilTimer.Start();

                            // Set the Hop schedule timer variables
                            StepStartTime = DateTime.Now.TimeOfDay;
                            StepEndTime = BoilEndTime.Add(-TimeSpan.FromMinutes(ingredients.Hops[CurrentHopAddition].BoilTime));

                            // Start the Hop Schedule Timer
                            HopScheduleTimer.Interval = TimeSpan.FromMilliseconds(250);
                            HopScheduleTimer.Tick += HopScheduleTimer_Tick;
                            HopScheduleTimer.Start();
                        }

                        // If the current hop addition is completed increment
                        if (HopAdditionComplete)
                        {
                            // Set the new hop schedule as incomplete
                            HopAdditionComplete = false;

                            // Check if all steps are completed
                            if (ingredients.Hops.Count < CurrentHopAddition + 1)
                            {
                                HopScheduleTimer.Stop();
                                return;
                            }

                            // Get the step start and end time
                            StepStartTime = DateTime.Now.TimeOfDay;
                            StepEndTime = BoilEndTime.Add(-TimeSpan.FromMinutes(ingredients.Hops[CurrentHopAddition].BoilTime));

                            // Start the step timer
                            MashStepTimer.Interval = TimeSpan.FromMilliseconds(250);
                            MashStepTimer.Tick += MashStepTimer_Tick;
                            MashStepTimer.Start();
                        }

                        // Check if boil is complete
                        if (BoilComplete)
                        {
                            // Turn off BK burner
                            breweryCommand.HoldTempPID(brewery.BK, 0);

                            // Switch to Chill State
                            breweryState = BreweryState.Chill;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                            userAlarm = new UserAlarm();
                        }

                        break;
                    }
                #endregion

                #region Chill
                case BreweryState.Chill:
                    {
                        // Send chill temperature info
                        if (!userAlarm.MessageSent)
                        {
                            PlayAlarm("Chill", true, true, false);
                            breweryState = BreweryState.Fermenter_Transfer;

                            brewery.BK.Temp.SetPoint = process.Fermentation.Temp;
                            brewery.BK.Temp.SetPointReached = false;

                            Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointUpdate");
                            Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointReachedUpdate");

                            userAlarm = new UserAlarm();
                        }

                        // Monitor Wort temperature
                        if(brewery.BK.Temp.Value <= brewery.BK.Temp.SetPoint)
                        {
                            brewery.BK.Temp.SetPointReached = true;
                            Messenger.Default.Send<Brewery>(brewery, "BKTempSetPointReachedUpdate");

                            // Set the BK Volume set point to 0
                            brewery.BK.Volume.SetPoint = 0;
                            brewery.BK.Volume.SetPointReached = false;

                            // Update brewery state
                            breweryState = BreweryState.Fermenter_Transfer;
                            RaisePropertyChanged(BreweryStateDisplayPropertyName);
                        }

                        break;
                    }
                #endregion

                #region Fermenter Transfer
                case BreweryState.Fermenter_Transfer:
                    {
                        // Request Valve Config
                        if (brewery.ValveConfig.ConfigSet != ValveConfigs.Fermenter_Transfer)
                        {
                            brewery.ValveConfig.Set(ValveConfigs.Fermenter_Transfer, brewery.Valves);
                            brewery.ValveConfig.ConfigSet = ValveConfigs.Fermenter_Transfer;
                        }

                        // Check the valve config confirmation
                        if (!brewery.ValveConfig.Check(ValveConfigs.Fermenter_Transfer, brewery.Valves)) { };

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
                    #endregion
            }
        }

        #endregion

        #region Manual Control Methods

        // Burner Control Command
        private void BurnerOverride_MessageReceived(Brewery.vessel Vessel)
        {
            if(brewery.AutomationMode != automationMode.Manual) { return; }
            if(!process.Session.IsStarted) { return; }
            
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

        private void PumpOverride_MessageReceived(Brewery.pump pump)
        {
            if (brewery.AutomationMode != automationMode.Manual) { return; }
            if (!process.Session.IsStarted) { return; }

            if (pump.IsOn)
            {
                if(pump.Number == 1) { breweryCommand.ActivatePump1(RelayState.Off); }
                if(pump.Number == 2) { breweryCommand.ActivatePump2(RelayState.Off); }
            }
            else
            {
                if (pump.Number == 1) { breweryCommand.ActivatePump1(RelayState.On); }
                if (pump.Number == 2) { breweryCommand.ActivatePump2(RelayState.On); }
            }
        }

        // Air pump Control Commands

        private void airPump1ClickCommand()
        {
            if(brewery.AutomationMode != automationMode.Manual) { return; }
            if (!process.Session.IsStarted) { return; }

            if (brewery.AirPump1.IsOn)
            {
                breweryCommand.ActivateAirPump1(RelayState.Off);
            }
            else
            {
                breweryCommand.ActivateAirPump1(RelayState.On);
            }

            RaisePropertyChanged(AirPump1StatusPropertyName);
        }

        private void airPump2ClickCommand()
        {
            if (brewery.AutomationMode != automationMode.Manual) { return; }
            if (!process.Session.IsStarted) { return; }

            if (brewery.AirPump2.IsOn)
            {
                breweryCommand.ActivateAirPump2(RelayState.Off);
            }
            else
            {
                breweryCommand.ActivateAirPump2(RelayState.On);
            }

            RaisePropertyChanged(AirPump2StatusPropertyName);
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

        private void UpdateSensorsTimer_Tick(object sender, EventArgs e)
        {
            breweryCommand.UpdateSensors();
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
            if (PrimingCount >= 4)
            {
                PrimingCount = 0;
                PrimingTimer1.Stop();
                brewery.Pump1.IsPriming = false;
                brewery.Pump1.IsPrimed = true;
                Messenger.Default.Send<Brewery.pump>(brewery.Pump1, "PrimingUpdate");
                return;
            }

            if(PrimingCount == 0) { brewery.Pump1.IsPriming = true; }
            if (brewery.Pump1.IsOn) { breweryCommand.ActivatePump1(RelayState.Off); }
            else { breweryCommand.ActivatePump1(RelayState.On); }
            PrimingCount++;
            
            Messenger.Default.Send<Brewery.pump>(brewery.Pump1);
        }

        private void PrimingTimer2_Tick(object sender, EventArgs e)
        {
            if (PrimingCount >= 4)
            {
                PrimingCount = 0;
                PrimingTimer2.Stop();
                brewery.Pump2.IsPriming = false;
                brewery.Pump2.IsPrimed = true;
                Messenger.Default.Send<Brewery.pump>(brewery.Pump2, "PrimingUpdate");
                return;
            }

            if (PrimingCount == 0) { brewery.Pump2.IsPriming = true; }
            if (brewery.Pump2.IsOn) { breweryCommand.ActivatePump2(RelayState.Off); }
            else { breweryCommand.ActivatePump2(RelayState.On); }
            PrimingCount++;

            Messenger.Default.Send<Brewery.pump>(brewery.Pump2);
        }

        private void MashStepTimer_Tick(object sender, EventArgs e)
        {
            // Increment ElapsedTime
            RemainingTime = StepEndTime - DateTime.Now.TimeOfDay;
            RaisePropertyChanged(MashHopTimerDisplayPropertyName);

            // Check if mash step is completed
            if (DateTime.Now.TimeOfDay >= StepEndTime)
            {
                HopAdditionComplete = true;
                RemainingTime = new TimeSpan();
                step++;
            }
        }

        private void HopScheduleTimer_Tick(object sender, EventArgs e)
        {
            // Increment ElapsedTime
            RemainingTime = StepEndTime - DateTime.Now.TimeOfDay;
            RaisePropertyChanged(MashHopTimerDisplayPropertyName);

            // Check if mash step is completed
            if (RemainingTime <= TimeSpan.FromSeconds(0))
            {
                HopAdditionComplete = true;
                RemainingTime = new TimeSpan();
                CurrentHopAddition++;
            }
        }

        private void BoilTimer_Tick(object sender, EventArgs e)
        {
            // Increment ElapsedTime
            BoilRemainingTime = BoilEndTime - DateTime.Now.TimeOfDay;

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
            userAlarm.RemainingTime = BoilRemainingTime;
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

            // Verify Session Status if started and safemode is off then stop session
            if (process.Session.IsStarted && !brewery.SafeModeOn)
            {
                // Stop data acquisition
                process.Session.StartRequested = false;
                process.Session.IsStarted = false;
                RaisePropertyChanged(StartSessionButtonContentPropertyName);
                UpdateTempSensorTimer.Stop();
                UpdateVolSensorTimer.Stop();

                // Reset all relays to off state
                breweryCommand.PreDisconnect();
                return;
            }

            // Check if Current Session is in safe mode. If true, Resume
            if(brewery.SafeModeOn)
            {
                brewery.SafeModeOn = false;
                RaisePropertyChanged(StartSessionButtonContentPropertyName);
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

            // Setting Sensors Timer Properties
            UpdateSensorsTimer.Interval = TimeSpan.FromMilliseconds(hardwareSettings.SensorsRefreshRate);
            UpdateSensorsTimer.Tick += UpdateSensorsTimer_Tick;
            UpdateSensorsTimer.Start();

            // Verify the automation mode selected
            if((brewery.AutomationMode == automationMode.Manual))
            {
                breweryState = BreweryState.Manual_Override;
                RaisePropertyChanged(BreweryStateDisplayPropertyName);
                return;
            }
            else if (brewery.AutomationMode == automationMode.SemiAuto)
            {
                breweryState = BreweryState.SemiAuto;
                RaisePropertyChanged(BreweryStateDisplayPropertyName);
                return;
            }

            // Run state Machine
            breweryState = BreweryState.StandBy;
            RaisePropertyChanged(BreweryStateDisplayPropertyName);
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

        private void valveClickCommand(Brewery.valve _Valve)
        {
            if(_Valve == null) { throw new Exception("Valve Paramater was null"); }
            brewery.Valves[_Valve.Number].IsOpen = !_Valve.IsOpen;
            Messenger.Default.Send(brewery.Valves[_Valve.Number]);
            RaisePropertyChanged("ValveList");
        }

        // Plot Buttons
        private void hltPlotButtonClickCommand()
        {
            List<Visibility> _PlotVisibility = new List<Visibility>(new Visibility[] { Visibility.Visible, Visibility.Hidden, Visibility.Hidden});
            Messenger.Default.Send<List<Visibility>>(_PlotVisibility, "PlotVisiblityUpdate");
            _PlotVisibility.Clear();
            _PlotVisibility = null;

            PlotVisibility = new List<bool>( new bool[3] { true, false, false });
            RaisePropertyChanged("PlotVisibility");
        }

        private void mltPlotButtonClickCommand()
        {
            List<Visibility> _PlotVisibility = new List<Visibility>(new Visibility[] { Visibility.Hidden, Visibility.Visible, Visibility.Hidden });
            Messenger.Default.Send<List<Visibility>>(_PlotVisibility, "PlotVisiblityUpdate");
            _PlotVisibility.Clear();
            _PlotVisibility = null;

            PlotVisibility = new List<bool>(new bool[3] { false, true, false });
            RaisePropertyChanged("PlotVisibility");
        }

        private void bkPlotButtonClickCommand()
        {
            List<Visibility> _PlotVisibility = new List<Visibility>(new Visibility[] { Visibility.Hidden, Visibility.Hidden, Visibility.Visible });
            Messenger.Default.Send<List<Visibility>>(_PlotVisibility, "PlotVisiblityUpdate");
            _PlotVisibility.Clear();
            _PlotVisibility = null;

            PlotVisibility = new List<bool>(new bool[3] { false, false, true });
            RaisePropertyChanged("PlotVisibility");
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
            RaisePropertyChanged(MashHopTimerDisplayPropertyName);

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
            if(DesignMode) { RunAutoStateMachine(); return; }
            brewery.HLT.Temp.Value = _brewery.HLT.Temp.Value;
            brewery.MLT.Temp.Value = _brewery.MLT.Temp.Value;
            brewery.BK.Temp.Value = _brewery.BK.Temp.Value;
       
            if (brewery.AutomationMode != automationMode.Automatic) { return; }
            RunAutoStateMachine();
        }

        // Volume Update
        private void VolumeUpdate_MessageReceived(Brewery _brewery)
        {
            if(DesignMode) { RunAutoStateMachine();  return; }
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
            RaisePropertyChanged(AirPump1StatusPropertyName);
        }

        // Air Pump 2 Update
        private void AirPump2Update_MessageReceived(Brewery _brewery)
        {
            brewery.AirPump2.IsOn = _brewery.AirPump2.IsOn;
            RaisePropertyChanged(AirPump2StatusPropertyName);
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

        // HLT LastError Update
        private void HLTPIDUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.LastError = _brewery.HLT.Temp.LastError;
            brewery.HLT.Temp.ErrorIntegral = _brewery.HLT.Temp.ErrorIntegral;

            // Add the new error value in the error array for derivation calcultions
            brewery.HLT.Temp.ErrorDerivative.Add(_brewery.HLT.Temp.LastError);

            // Resize the error array if needed to limit to 100 elements
            if (brewery.HLT.Temp.ErrorDerivative.Count > hardwareSettings.ErrorDerivativeCount) { brewery.HLT.Temp.ErrorDerivative.RemoveAt(0); }
        }

        // MLT LastError Update
        private void MLTPIDUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Temp.LastError = _brewery.MLT.Temp.LastError;
            brewery.MLT.Temp.ErrorIntegral = _brewery.MLT.Temp.ErrorIntegral;

            // Add the new error value in the error array for derivation calcultions
            brewery.MLT.Temp.ErrorDerivative.Add(_brewery.MLT.Temp.LastError);

            // Resize the error array if needed to limit to 100 elements
            if (brewery.MLT.Temp.ErrorDerivative.Count > hardwareSettings.ErrorDerivativeCount) { brewery.MLT.Temp.ErrorDerivative.RemoveAt(0); }
        }

        // BK LastError Update
        private void BKPIDUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Temp.LastError = _brewery.BK.Temp.LastError;
            brewery.BK.Temp.ErrorIntegral = _brewery.BK.Temp.ErrorIntegral;

            // Add the new error value in the error array for derivation calcultions
            brewery.BK.Temp.ErrorDerivative.Add(_brewery.BK.Temp.LastError);

            // Resize the error array if needed to limit to 100 elements
            if (brewery.BK.Temp.ErrorDerivative.Count > hardwareSettings.ErrorDerivativeCount) { brewery.BK.Temp.ErrorDerivative.RemoveAt(0); }
        }

        // Valve Update
        private void ValveUpdate_MessageReceived(Brewery.valve _Valve)
        {
            brewery.Valves[_Valve.Number] = _Valve;
            RaisePropertyChanged("ValveList");
        }

        // Safe Mode Update
        private void SafeModeUpdate_MessageReceived( bool _SafeModeState)
        {

            brewery.SafeModeOn = true;
            UpdateSensorsTimer.Stop();
            breweryCommand.PreDisconnect();
            RunAutoStateMachine();
        }

        // DEBUG METHODS

        private void DebugTemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.Value = _brewery.HLT.Temp.Value;
            brewery.MLT.Temp.Value = _brewery.MLT.Temp.Value;
            brewery.BK.Temp.Value = _brewery.BK.Temp.Value;
        }

        private void DebugVolumeUpdate_MessageReveived(Brewery _brewery)
        {
            brewery.HLT.Volume.Value = _brewery.HLT.Volume.Value; 
            brewery.BK.Volume.Value = _brewery.BK.Volume.Value; 
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
            Messenger.Default.Register<Brewery>(this, "DebugTemperatureUpdate", DebugTemperatureUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "DebugVolumeUpdate", DebugVolumeUpdate_MessageReveived);
            Messenger.Default.Send<bool>(DesignMode, "DesignMode");
            startBrewSessionClickCommand();
        }

        #endregion


    }
}