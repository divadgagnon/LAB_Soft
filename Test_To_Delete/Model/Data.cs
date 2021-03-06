﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace LAB.Model
{
    public class General
    {
        public string Name { get; set; }
        public double SRMColor { get; set; }
        public double BatchSize { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Brewer { get; set; }
    }

    public class Style
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Process
    {
        public session Session { get; set; }
        public strike Strike { get; set; }
        public sparge Sparge { get; set; }
        public boil Boil { get; set; }
        public fermentation Fermentation { get; set; }

        public List<MashStep> MashSteps { get; set; }

        public Process()
        {
            MashSteps = new List<MashStep>();
            Strike = new strike();
            Sparge = new sparge();
            Boil = new boil();
            Fermentation = new fermentation();
            Session = new session();
        }

        public class session
        {
            public bool IsStarted { get; set; }
            public bool StartRequested { get; set; }
            public double TotalWaterNeeded { get; set; }
            public automationMode ControlMode { get; set; }
        }

        public class MashStep
        {
            public string Name { get; set; }
            public double Volume { get; set; }
            public double Temp { get; set; }
            public double Time { get; set; }
        }

        public class strike
        {
            public double Temp { get; set;}
            public double Volume { get; set; }
        }

        public class sparge
        {
            public double Temp { get; set; }
            public double Volume { get; set; }
        }

        public class boil
        {
            public double Time { get; set; }
            public double Volume { get; set; }
        }

        public class fermentation
        {
            public double Temp { get; set; }
            public double Age { get; set; }
        }

        public class settings
        {
            public double TempHoldingRange { get; set; } = 0.5;
        }
    }

    public class Ingredients
    {
        public List<Malt> Malts { get; set; }
        public List<Hop> Hops { get; set; }
        public List<Malt> Adjuncts { get; set; }
        public yeast Yeast { get; set; }

        public Ingredients()
        {
            Malts = new List<Malt>();
            Hops = new List<Hop>();
            Adjuncts = new List<Malt>();
            Yeast = new yeast();
        }

        public class Malt
        {
            public visibility Visibility { get; }
            public string Name { get; set; }
            public double Quantity { get; set; }
            public double SRM { get; set; }
            public string Units { get; } = "Kg";
        }

        public class Hop
        {
            public visibility Visibility { get; }
            public string Name { get; set; }
            public double Quantity { get; set; }
            public double BoilTime { get; set; }
            public string QtyUnits { get; } = "g";
            public string TimeUnits { get; } = "min";
        }

        public class yeast
        {
            public visibility Visiblity { get; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Form { get; set; }
            public double MaxTemp { get; set; }
            public double MinTemp { get; set; }
        }
    }

    public class visibility
    {
        public string Collapsed { get; } = "Collapsed";
        public string Hidden { get; } = "Hidden";
        public string Visible { get; } = "Visible";
    }

    public class AnalogReturn
    {
        public int Pin { get; set; }
        public int Value { get; set; }
        public bool OverByte { get; set; }
    }

    public class DigitalReturn
    {
        public int Pin { get; set; }
        public bool Value { get; set; }
    }


    // Probe Class containing all the probes (Addresses and string identifiers (Color) hardcoded for now
    public class Probes
    {
        public probe Yellow { get; set; }
        public probe Pink { get; set; }
        public probe Orange { get; set; }
        public probe YellowOrange { get; set; }
        public probe YellowPink { get; set; }

        // Constructor
        public Probes()
        {
            Yellow = new probe(0x26, "Yellow");
            Pink = new probe(0x4F, "Pink");
            Orange = new probe(0xA5, "Orange");
            YellowOrange = new probe(0x9E, "Yellow and Orange");
            YellowPink = new probe(0x4B, "Yellow and Pink");
        }
    }


    // Single probe class
    public class probe
    {
        public byte Address { get; }
        public double Temp { get; set; }
        public bool IsConnected { get; set; }
        public string Color { get; }

        public probe(byte address, string color)
        {
            Address = address;
            Color = color;
        }

    }

    public class SpecificProbes
    {
        public byte Address { get; set; } 
        public double Temp { get; set; }
        public bool IsConnected { get; set; }
        public string Color { get; set; }
    }

    public class HardwareSettings
    {
        public string comPort { get; set; }
        public int VolResfreshRate { get; set; } = 1500;
        public int TempRefreshRate { get; set; } = 2500;
        public int SensorsRefreshRate { get; set; } = 1500;
        public int OneWireBus { get; set; } = 2;
        public int HLT_Vol_Pin { get; set; } = 0;
        public int BK_Vol_Pin { get; set; } = 1;
        public int Pump1_Pin { get; set; } = 3;
        public int Pump2_Pin { get; set; } = 4;
        public int HLT_Burner_Pin { get; set; } = 5;
        public int MLT_Burner_Pin { get; set; } = 6;
        public int BK_Burner_Pin { get; set; } = 7;
        public int AirPump1_Pin { get; set; } = 8;
        public int AirPump2_Pin { get; set; } = 9;
        public int PrimingDelay { get; set; } = 5000;
        public double Kp { get; set; } = 0.3;
        public double Ki { get; set; } = 0.0002;
        public double Kd { get; set; } = 20;
        public int ErrorDerivativeCount { get; set; } = 120;
        public double PIDTimerInterval { get; set; } = 5;
        public SpecificProbes HLT_Temp_Probe { get; set; } = new SpecificProbes();
        public SpecificProbes MLT_Temp_Probe { get; set; } = new SpecificProbes();
        public SpecificProbes BK_Temp_Probe { get; set; } = new SpecificProbes();
    }

    public class Brewery
    {
        public bool IsConnected { get; set; }
        public bool CalibrationModeOn { get; set; } = false; // DEBUG

        public vessel HLT { get; set; }
        public vessel MLT { get; set; }
        public vessel BK { get; set; }
        public pump Pump1 { get; set; }
        public pump Pump2 { get; set; }
        public pump AirPump1 { get; set; }
        public pump AirPump2 { get; set; }
        public automationMode AutomationMode { get; set; } = automationMode.Automatic;
        public ObservableCollection<valve> Valves { get; set; }
        public valveConfig ValveConfig { get; set; }
        public calibration Calibration { get; set; }
        public bool SafeModeOn { get; set; }

        public Brewery()
        {
            // Create Class instances
            HLT = new vessel(Vessels.HLT);
            MLT = new vessel(Vessels.MLT);
            BK = new vessel(Vessels.BK);
            Pump1 = new pump();
            Pump2 = new pump();
            AirPump1 = new pump();
            AirPump2 = new pump();
            AutomationMode = new automationMode();
            ValveConfig = new valveConfig();
            ValveConfig.ConfigSet = ValveConfigs.AllOff;
            Calibration = new calibration();

            // Create Valve List
            Valves = new ObservableCollection<valve>();
            Valves.Add(new valve() { Name = "HLTout", Number = 0 });
            Valves.Add(new valve() { Name = "MLTin", Number = 1 });
            Valves.Add(new valve() { Name = "MLTout", Number = 2 });
            Valves.Add(new valve() { Name = "MLTreturn", Number = 3 });
            Valves.Add(new valve() { Name = "BKin", Number = 4 });
            Valves.Add(new valve() { Name = "BKout", Number = 5 });

            Pump1.Number = 1;
            Pump2.Number = 2;
        }

        public class vessel
        {
            public volume Volume { get; set; }
            public temp Temp { get; set; }
            public burner Burner { get; set; }
            public Vessels Name { get; private set; }

            public vessel(Vessels _vesselType)
            {
                Volume = new volume();
                Temp = new temp();
                Burner = new burner();
                Name = _vesselType;
            }
        }

        public class volume
        {
            public double Value { get; set; }
            public double SetPoint { get; set; }
            public bool SetPointReached { get; set; } = true;
        }

        public class temp
        {
            public double Value { get; set; }
            public double SetPoint { get; set; }
            public double LastError { get; set; }
            public double ErrorIntegral { get; set; }
            public List<double> ErrorDerivative { get; set; }
            public bool SetPointReached { get; set; } = true;
            public bool BoilReached { get; set; }

            public temp()
            {
                ErrorDerivative = new List<double>();
            }
        }

        public class burner
        {
            public bool IsOn { get; set; }
            public bool PilotIsOn { get; set; }
            public double DutyCycle { get; set; }
        }

        public class pump
        {
            public int Number { get; set; }
            public bool IsOn { get; set; }
            public bool IsPrimed { get; set; }
            public bool IsPriming { get; set; }
        }

        public class calibration
        {
            public CalibrationState State { get; set; } = CalibrationState.StandBy;
            public bool IsOn { get; set; }

            public calibration()
            {
                State = new CalibrationState();
            }
        }

        public class valve
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public bool IsOpen { get; set; }
            public request Request { get; set; }
            
            public valve()
            {
                Request = new request();
            }
            
            public class request
            {
                public bool Open { get; set; }
                public bool Close { get; set; }
            }
        }

        public class valveConfig
        {
            public ValveConfigs ConfigSet
            {
                get;
                set;
            }

            public bool Check(ValveConfigs Config, ObservableCollection<valve> Valves)
            {
                switch(Config)
                {
                    case ValveConfigs.Strike_Transfer:
                        {
                            // Cancel the requests and update the UI
                            if(Valves[0].IsOpen)
                            {
                                Valves[0].Request.Open = false;
                                Messenger.Default.Send(Valves[0]);
                            }

                            if(Valves[1].IsOpen)
                            {
                                Valves[1].Request.Open = false;
                                Messenger.Default.Send(Valves[1]);
                            }

                            if(!Valves[2].IsOpen)
                            {
                                Valves[2].Request.Close = false;
                                Messenger.Default.Send(Valves[2]);
                            }

                            if (Valves[0].IsOpen && Valves[1].IsOpen && !Valves[2].IsOpen) { return true; }
                            else { return false; }
                        }

                    case ValveConfigs.Mash_Recirc:
                        {
                            // Cancel the requests
                            if(Valves[2].IsOpen)
                            {
                                Valves[2].Request.Open = false;
                                Messenger.Default.Send(Valves[2]);
                            }

                            if(Valves[3].IsOpen)
                            {
                                Valves[3].Request.Open = false;
                                Messenger.Default.Send(Valves[3]);
                            }

                            if(Valves[2].IsOpen && Valves[3].IsOpen) { return true; }
                            else { return false; }
                        }

                    case ValveConfigs.Fly_Sparge:
                        {
                            if(Valves[0].IsOpen)
                            {
                                Valves[0].Request.Open = false;
                                Messenger.Default.Send(Valves[0]);
                            }

                            if (Valves[1].IsOpen)
                            {
                                Valves[1].Request.Open = false;
                                Messenger.Default.Send(Valves[1]);
                            }

                            if (Valves[2].IsOpen)
                            {
                                Valves[2].Request.Open = false;
                                Messenger.Default.Send(Valves[2]);
                            }

                            if (!Valves[3].IsOpen)
                            {
                                Valves[3].Request.Close = false;
                                Messenger.Default.Send(Valves[3]);
                            }

                            if (Valves[4].IsOpen)
                            {
                                Valves[4].Request.Open = false;
                                Messenger.Default.Send(Valves[4]);
                            }

                            if (Valves[0].IsOpen && Valves[1].IsOpen && Valves[2].IsOpen && !Valves[3].IsOpen && Valves[4].IsOpen) { return true; }
                            else { return false; }
                        }

                    case ValveConfigs.Fermenter_Transfer:
                        {
                            if(Valves[5].IsOpen) { Valves[5].Request.Open = false; }
                            Messenger.Default.Send(Valves[5]);
                        }

                        if(Valves[5].IsOpen) { return true; }
                        else { return false; }

                    default:
                        {
                            return false;
                        }
                }
            }

            public void Set(ValveConfigs Config, ObservableCollection<valve> Valves)
            {
                switch(Config)
                {
                    case ValveConfigs.Strike_Transfer:
                        {
                            // Set the resquests
                            if (!Valves[0].IsOpen) { Valves[0].Request.Open = true; }
                            if (!Valves[1].IsOpen) { Valves[1].Request.Open = true; }
                            if (Valves[2].IsOpen) { Valves[2].Request.Close = true; }

                            // Update the UI
                            Messenger.Default.Send(Valves[0]);
                            Messenger.Default.Send(Valves[1]);
                            Messenger.Default.Send(Valves[2]);

                            break;
                        }

                    case ValveConfigs.Mash_Recirc:
                        {
                            // Set the resquests
                            if (!Valves[2].IsOpen) { Valves[2].Request.Open = true; }
                            if(!Valves[3].IsOpen) { Valves[3].Request.Open = true; }

                            // Update the UI
                            Messenger.Default.Send(Valves[2]);
                            Messenger.Default.Send(Valves[3]);

                            break;
                        }

                    case ValveConfigs.Fly_Sparge:
                        {
                            // Set the requests
                            if(!Valves[0].IsOpen) { Valves[0].Request.Open = true; }
                            if(!Valves[1].IsOpen) { Valves[1].Request.Open = true; }
                            if(!Valves[2].IsOpen) { Valves[2].Request.Open = true; }
                            if(Valves[3].IsOpen) { Valves[3].Request.Close = true; }
                            if(!Valves[4].IsOpen) { Valves[4].Request.Open = true; }

                            // Update the UI
                            for(int i=0;i<=4;i++)
                            {
                                Messenger.Default.Send(Valves[i]);
                            }

                            break;
                        }

                    case ValveConfigs.Fermenter_Transfer:
                        {
                            // Set the requests
                            if(!Valves[5].IsOpen) { Valves[5].Request.Open = true; }

                            // Update the UI
                            Messenger.Default.Send(Valves[5]);

                            break;
                        }
                }
            }
        }
        
    }

    public class UserAlarm
    {
        public bool IsActive { get; set; }
        public BreweryState CurrentState { get; set; }
        public string AlarmType { get; set; }
        public Process ProcessData { get; set; }
        public bool ProceedIsPressed { get; set; }
        public bool HoldIsPressed { get; set; }
        public bool MessageSent { get; set; }
        public bool VisualAlarm { get; set; }
        public bool AudibleAlarm { get; set; }
        public int CurrentMashStep { get; set;}
        public TimeSpan RemainingTime { get; set; }
        public double NumData { get; set; }
    }

    public class MashTimerDisplayItem
    {
        public string StepName { get; set; }
        public string Time { get; set; }
    }

    public class DataLog
    {
        // à implémenter
    }

    public enum Vessels
    {
        HLT,
        MLT,
        BK,
    }

    public enum ValveList
    {
        HLTout = 0,
        MLTin = 1,
        MLTout = 2,
        MLTreturn = 3,
        BKin = 4,
        BKout = 5,
    }

    public enum ValveConfigs
    {
        AllOff,
        Strike_Transfer,
        Mash_Recirc,
        Fly_Sparge,
        Fermenter_Transfer,
    }

    public enum BreweryState
    {
        StandBy,
        HLT_Fill,
        Strike_Heat,
        Strike_Transfer,
        DoughIn,
        Mash,
        Sparge_Heat,
        Sparge,
        Boil,
        Chill,
        Fermenter_Transfer,
        Manual_Override,
        SemiAuto,
        CalibrationMode,
    }

    public enum CalibrationState
    {
        StandBy,
        Setup,
        Fill,
        DataAquisition,
        OutputData,
    }

    public enum PinModes
    {
        Output = 0x00,
        Input = 0x01,
    }

    public enum RelayState
    {
        On = 0x00,
        Off = 0x01,
    }

    public enum automationMode
    {
        Automatic,
        SemiAuto,
        Manual,
    }


}
