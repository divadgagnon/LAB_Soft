using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;
using System;

namespace LAB.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class BKViewModel : ViewModelBase
    {
        // Instance intialization
        Brewery brewery;

        // Property Names
        public const string WaterHeightPropertyName = "WaterHeight";
        public const string WaterHeightSetPointPropertyName = "WaterHeightSetPoint";
        public const string BK_VolumePropertyName = "BK_Volume";
        public const string BK_TempPropertyName = "BK_Temp";
        public const string BK_Volume_SetPointPropertyName = "BK_Volume_SetPoint";
        public const string BK_SetPoint_Label_VisibilityPropertyName = "BK_SetPoint_Visibility";
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
                return (int)Math.Round(brewery.BK.Volume.Value / 50 * (KegHeight - 5), 0);
            }
        }

        // Water Set Point indicator Position
        public int WaterHeightSetPoint
        {
            get
            {
                return (int)Math.Round(brewery.BK.Volume.SetPoint / 50 * (KegHeight - 5) - 3, 0);
            }
        }

        // Water Volume Numerical Display
        public string BK_Volume
        {
            get
            {
                return Math.Round(brewery.BK.Volume.Value, 1) + " liters";
            }
        }

        // Water Volume Set Point Numerical Display
        public string BK_Volume_SetPoint
        {
            get
            {
                return Math.Round(brewery.BK.Volume.SetPoint, 1) + " l";
            }
        }

        // Water Volume Set Point Visibility
        public string BK_SetPoint_Visibility
        {
            get
            {
                if (brewery.BK.Volume.SetPointReached) { return "Hidden"; }
                else { return "Visible"; }
            }
        }

        // Water Temperaure Numerical Display
        public string BK_Temp
        {
            get
            {
                return Math.Round(brewery.BK.Temp.Value, 1) + " °C";
            }
        }

        // Thermometer Height Display
        public int Thermo_Height
        {
            get
            {
                return (int)Math.Round(brewery.BK.Temp.Value / 100 * (KegHeight - 30) + 5, 0);
            }
        }

        // Thermometer Set Point Height Display
        public int Thermo_Height_SetPoint
        {
            get
            {
                return (int)Math.Round(brewery.BK.Temp.SetPoint / 100 * (KegHeight - 30) + 20, 0);
            }
        }

        // Thermometer Set Point Label Display
        public string Thermo_SetPoint
        {
            get
            {
                return Math.Round(brewery.BK.Temp.SetPoint, 1) + " °C";
            }
        }

        // Thermometer Set Point Indicator Visiblity
        public string Thermo_SetPoint_Visibility
        {
            get
            {
                if (brewery.BK.Temp.SetPointReached) { return "Hidden"; }
                else { return "Visible"; }
            }
        }

        // Burner status display (on or off)
        public string Flames_Visibility
        {
            get
            {
                if (brewery.BK.Burner.IsOn) { return "Visible"; }
                else { return "Hidden"; }
            }
        }

        public BKViewModel()
        {
            // Create new instances of model classes
            brewery = new Brewery();

            // Register to sensor update messages
            Messenger.Default.Register<Brewery>(this, "VolumeUpdate", VolumeUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKBurnerUpdate", BKBurnerUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKVolumeSetPointUpdate", BKVolumeSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKVolumeSetPointReachedUpdate", BKVolumeSetPointReachedUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKTempSetPointUpdate", BKTempSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "BKTempSetPointReachedUpdate", BKTempSetPointReachedUpdate_MessageReceived);
        }

        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the Tempperature
            brewery.BK.Temp.Value = _brewery.BK.Temp.Value;

            // Raise the temp related properties changed event
            RaisePropertyChanged(BK_TempPropertyName);
            RaisePropertyChanged(Thermo_HeightPropertyName);
        }

        private void VolumeUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the Volume
            if (_brewery.BK.Volume.Value == 500) { return; }
            brewery.BK.Volume.Value = _brewery.BK.Volume.Value;

            // Raise the volume related properties changed event
            RaisePropertyChanged(BK_VolumePropertyName);
            RaisePropertyChanged(WaterHeightPropertyName);
        }

        // BK Burner Update
        private void BKBurnerUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the BK burner state
            brewery.BK.Burner.IsOn = _brewery.BK.Burner.IsOn;

            // Raise the flames visibility changed event
            RaisePropertyChanged(Flames_VisibilityPropertyName);
        }

        private void BKVolumeSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Volume.SetPoint = _brewery.BK.Volume.SetPoint;
            RaisePropertyChanged(BK_Volume_SetPointPropertyName);
            RaisePropertyChanged(WaterHeightSetPointPropertyName);
        }

        private void BKVolumeSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Volume.SetPointReached = _brewery.BK.Volume.SetPointReached;
            RaisePropertyChanged(BK_SetPoint_Label_VisibilityPropertyName);
        }

        private void BKTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Temp.SetPoint = _brewery.BK.Temp.SetPoint;
            RaisePropertyChanged(Thermo_Height_SetPointPropertyName);
            RaisePropertyChanged(Thermo_SetPointPropertyName);
        }

        private void BKTempSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.BK.Temp.SetPointReached = _brewery.BK.Temp.SetPointReached;
            RaisePropertyChanged(Thermo_SetPoint_VisibilityPropertyName);
        }
    }
}