using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using LAB.Model;
using System;

namespace LAB.ViewModel
{
    public class HLTViewModel : ViewModelBase
    {
        // Model Instance intialization
        Brewery brewery;

        // Relay Commands Initialization

        public RelayCommand BurnerClickCommand { get; private set; }

        // Property Names
        public const string WaterHeightPropertyName = "WaterHeight";
        public const string WaterHeightSetPointPropertyName = "WaterHeightSetPoint";
        public const string HLT_VolumePropertyName = "HLT_Volume";
        public const string HLT_TempPropertyName = "HLT_Temp";
        public const string HLT_Volume_SetPointPropertyName = "HLT_Volume_SetPoint";
        public const string HLT_SetPoint_Label_VisibilityPropertyName = "HLT_SetPoint_Visibility";
        public const string Thermo_HeightPropertyName = "Thermo_Height";
        public const string Thermo_Height_SetPointPropertyName = "Thermo_Height_SetPoint";
        public const string Thermo_SetPointPropertyName = "Thermo_SetPoint";
        public const string Thermo_SetPoint_VisibilityPropertyName = "Thermo_SetPoint_Visibility";
        public const string Flames_VisibilityPropertyName = "Flames_Visibility";

        // Bindable Properties

        // Keg Height
        public int KegHeight
        {
            get { return 300; }
        }

        // Water rectangle height
        public int WaterHeight
        {
            get
            {
                return (int)Math.Round(brewery.HLT.Volume.Value/50 * (KegHeight-5), 0);
            }
        }

        // Water Set Point indicator Position
        public int WaterHeightSetPoint
        {
            get
            {
                return (int)Math.Round(brewery.HLT.Volume.SetPoint / 50 * (KegHeight-5) - 3, 0);
            }
        }

        // Water Volume Numerical Display
        public string HLT_Volume
        {
            get
            {
                return Math.Round(brewery.HLT.Volume.Value, 1) + " liters";
            }
        }

        // Water Volume Set Point Numerical Display
        public string HLT_Volume_SetPoint
        {
            get
            {
                return Math.Round(brewery.HLT.Volume.SetPoint, 1) + " l";
            }
        }

        // Water Volume Set Point Visibility
        public string HLT_SetPoint_Visibility
        {
            get
            {
                if (brewery.HLT.Volume.SetPointReached) { return "Hidden"; }
                else { return "Visible"; }
            }
        }

        // Water Temperaure Numerical Display
        public string HLT_Temp
        {
            get
            {
                return Math.Round(brewery.HLT.Temp.Value,1) + " °C";
            }
        }

        // Thermometer Height Display
        public int Thermo_Height
        {
            get
            {
                return (int)Math.Round(brewery.HLT.Temp.Value / 100 * (KegHeight-30) + 5 ,0);
            }
        }

        // Thermometer Set Point Height Display
        public int Thermo_Height_SetPoint
        {
            get
            {
                return (int)Math.Round(brewery.HLT.Temp.SetPoint / 100 * (KegHeight-30) + 20,0);
            }
        }

        // Thermometer Set Point Label Display
        public string Thermo_SetPoint
        {
            get
            {
                return Math.Round(brewery.HLT.Temp.SetPoint,1) + " °C";
            }
        }

        // Thermometer Set Point Indicator Visiblity
        public string Thermo_SetPoint_Visibility
        {
            get
            {
                if(brewery.HLT.Temp.SetPointReached) { return "Hidden"; }
                else { return "Visible"; }
            }
        }

        // Burner status display (on or off)
        public string Flames_Visibility
        {
            get
            {
                if(brewery.HLT.Burner.IsOn) { return "Visible"; }
                else { return "Hidden"; }
            }
        }

        public HLTViewModel()
        {
            // Create new instances of model classes
            brewery = new Brewery();

            // Create new instances of relay commands
            BurnerClickCommand = new RelayCommand(burnerClickCommand);

            // Register to sensor update messages
            Messenger.Default.Register<Brewery>(this, "VolumeUpdate", VolumeUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTBurnerUpdate", HLTBurnerUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTVolumeSetPointUpdate", HLTVolumeSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTVolumeSetPointReachedUpdate", HLTVolumeSetPointReachedUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTTempSetPointUpdate", HLTTempSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTTempSetPointReachedUpdate", HLTTempSetPointReachedUpdate_MessageReceived);
        }

        private void burnerClickCommand()
        {
            Messenger.Default.Send<Brewery.vessel>(brewery.HLT, "BurnerOverride");
        }

        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the Tempperature
            brewery.HLT.Temp.Value = _brewery.HLT.Temp.Value;

            // Raise the temp related properties changed event
            RaisePropertyChanged(HLT_TempPropertyName);
            RaisePropertyChanged(Thermo_HeightPropertyName);
        }

        private void VolumeUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the Volume
            if(_brewery.HLT.Volume.Value == 500) { return; }
            brewery.HLT.Volume.Value = _brewery.HLT.Volume.Value;

            // Raise the volume related properties changed event
            RaisePropertyChanged(HLT_VolumePropertyName);
            RaisePropertyChanged(WaterHeightPropertyName);
        }

        // HLT Burner Update
        private void HLTBurnerUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the HLT burner state
            brewery.HLT.Burner.IsOn = _brewery.HLT.Burner.IsOn;

            // Raise the flames visibility changed event
            RaisePropertyChanged(Flames_VisibilityPropertyName);
        }

        private void HLTVolumeSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Volume.SetPoint = _brewery.HLT.Volume.SetPoint;
            RaisePropertyChanged(HLT_Volume_SetPointPropertyName);
            RaisePropertyChanged(WaterHeightSetPointPropertyName);
        }

        private void HLTVolumeSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Volume.SetPointReached = _brewery.HLT.Volume.SetPointReached;
            RaisePropertyChanged(HLT_SetPoint_Label_VisibilityPropertyName);
        }

        private void HLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.SetPoint = _brewery.HLT.Temp.SetPoint;
            RaisePropertyChanged(Thermo_Height_SetPointPropertyName);
            RaisePropertyChanged(Thermo_SetPointPropertyName);
        }

        private void HLTTempSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.HLT.Temp.SetPointReached = _brewery.HLT.Temp.SetPointReached;
            RaisePropertyChanged(Thermo_SetPoint_VisibilityPropertyName);
        }
    }
}