using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public double TempHoldingRange { get; set; } = 0.2;
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
    }

    public class DigitalReturn
    {
        public int Pin { get; set; }
        public bool Value { get; set; }
    }

    public class Probes
    {
        public yellow Yellow { get; set; }
        public pink Pink { get; set; }
        public orange Orange { get; set; }
        public yellowOrange YellowOrange { get; set; }
        public yellowPink YellowPink { get; set; }

        // Constructor
        public Probes()
        {
            Yellow = new yellow();
            Pink = new pink();
            Orange = new orange();
            YellowOrange = new yellowOrange();
            YellowPink = new yellowPink();
        }

        // Temperature probes

        public class yellow
        {
            public byte Address { get; } = 0x26;
            public double Temp { get; set; }
            public bool IsConnected { get; set; }
            public string Color { get; } = "Yellow";
        }

        public class pink
        {
            public byte Address { get; } = 0x4F;
            public double Temp { get; set; }
            public bool IsConnected { get; set; }
            public string Color { get; } = "Pink";
        }

        public class orange
        {
            public byte Address { get; } = 0xA5;
            public double Temp { get; set; }
            public bool IsConnected { get; set; }
            public string Color { get; } = "Orange";
        }

        public class yellowOrange
        {
            public byte Address { get; } = 0x9E;
            public double Temp { get; set; }
            public bool IsConnected { get; set; }
            public string Color { get; } = "Yellow and Orange";
        }

        public class yellowPink
        {
            public byte Address { get; } = 0x4B;
            public double Temp { get; set; }
            public bool IsConnected { get; set; }
            public string Color { get; } = "Yellow and Pink";
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
        public int VolResfreshRate { get; set; } = 1000;
        public int TempRefreshRate { get; set; } = 2500;
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
        public SpecificProbes HLT_Temp_Probe { get; set; } = new SpecificProbes();
        public SpecificProbes MLT_Temp_Probe { get; set; } = new SpecificProbes();
        public SpecificProbes BK_Temp_Probe { get; set; } = new SpecificProbes();
    }

    public class Brewery
    {
        public bool IsConnected { get; set; }

        public vessel HLT { get; set; }
        public vessel MLT { get; set; }
        public vessel BK { get; set; }
        public pump Pump1 { get; set; }
        public pump Pump2 { get; set; }
        public pump AirPump1 { get; set; }
        public pump AirPump2 { get; set; }
        public automationMode AutomationMode { get; set; } = automationMode.Automatic;

        public Brewery()
        {
            HLT = new vessel(Vessels.HLT);
            MLT = new vessel(Vessels.MLT);
            BK = new vessel(Vessels.BK);
            Pump1 = new pump();
            Pump2 = new pump();
            AirPump1 = new pump();
            AirPump2 = new pump();
            AutomationMode = new automationMode();

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
            public bool SetPointReached { get; set; } = true;
            public bool BoilReached { get; set; }
        }

        public class burner
        {
            public bool IsOn { get; set; }
            public bool PilotIsOn { get; set; }
        }

        public class pump
        {
            public int Number { get; set; }
            public bool IsOn { get; set; }
            public bool IsPrimed { get; set; }
            public bool IsPriming { get; set; }
        }

        public class valve
        {
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
