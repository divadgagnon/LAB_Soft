using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
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
    public class MLTViewModel : ViewModelBase
    {
        // Instance intialization
        Brewery brewery;

        // Relay Command Initialization
        public RelayCommand BurnerClickCommand { get; private set; }

        // Property Names
        public const string WaterHeightPropertyName = "WaterHeight";
        public const string WaterHeightSetPointPropertyName = "WaterHeightSetPoint";
        public const string MLT_VolumePropertyName = "MLT_Volume";
        public const string MLT_TempPropertyName = "MLT_Temp";
        public const string MLT_Duty_CyclePropertyName = "MLT_Duty_Cycle";
        public const string MLT_Volume_SetPointPropertyName = "MLT_Volume_SetPoint";
        public const string MLT_SetPoint_Label_VisibilityPropertyName = "MLT_SetPoint_Visibility";
        public const string Thermo_HeightPropertyName = "Thermo_Height";
        public const string Thermo_Height_SetPointPropertyName = "Thermo_Height_SetPoint";
        public const string Thermo_SetPointPropertyName = "Thermo_SetPoint";
        public const string Thermo_SetPoint_VisibilityPropertyName = "Thermo_SetPoint_Visibility";
        public const string Flames_VisibilityPropertyName = "Flames_Visibility";

        #region Bindable Properties

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
                return (int)Math.Round(brewery.MLT.Volume.Value / 50 * (KegHeight - 5), 0);
            }
        }

        // Water Set Point indicator Position
        public int WaterHeightSetPoint
        {
            get
            {
                return (int)Math.Round(brewery.MLT.Volume.SetPoint / 50 * (KegHeight - 5) - 3, 0);
            }
        }

        // Water Volume Numerical Display
        public string MLT_Volume
        {
            get
            {
                return Math.Round(brewery.MLT.Volume.Value, 1) + " liters";
            }
        }

        // Water Volume Set Point Numerical Display
        public string MLT_Volume_SetPoint
        {
            get
            {
                return Math.Round(brewery.MLT.Volume.SetPoint, 1) + " l";
            }
        }

        // Water Volume Set Point Visibility
        public string MLT_SetPoint_Visibility
        {
            get
            {
                if (brewery.MLT.Volume.SetPointReached) { return "Hidden"; }
                else { return "Visible"; }
            }
        }

        // Water Temperaure Numerical Display
        public string MLT_Temp
        {
            get
            {
                return Math.Round(brewery.MLT.Temp.Value, 1) + " °C";
            }
        }

        // Thermometer Height Display
        public int Thermo_Height
        {
            get
            {
                return (int)Math.Round(brewery.MLT.Temp.Value / 100 * (KegHeight - 30) + 5, 0);
            }
        }

        // Thermometer Set Point Height Display
        public int Thermo_Height_SetPoint
        {
            get
            {
                return (int)Math.Round(brewery.MLT.Temp.SetPoint / 100 * (KegHeight - 30) + 20, 0);
            }
        }

        // Thermometer Set Point Label Display
        public string Thermo_SetPoint
        {
            get
            {
                return Math.Round(brewery.MLT.Temp.SetPoint, 1) + " °C";
            }
        }

        // Thermometer Set Point Indicator Visiblity
        public string Thermo_SetPoint_Visibility
        {
            get
            {
                if (brewery.MLT.Temp.SetPointReached) { return "Hidden"; }
                else { return "Visible"; }
            }
        }

        // Burner status display (on or off)
        public string Flames_Visibility
        {
            get
            {
                if (brewery.MLT.Burner.IsOn) { return "Visible"; }
                else { return "Hidden"; }
            }
        }

        // Burner DutyCycle
        public string MLT_Duty_Cycle
        {
            get
            {
                return "Duty Cycle : " + Math.Round(brewery.MLT.Burner.DutyCycle * 100, 2) + " %";
            }
        }

        #endregion

        public MLTViewModel()
        {
            // Create new instances of model classes
            brewery = new Brewery();

            // Create instances of Relay Commands
            BurnerClickCommand = new RelayCommand(burnerClickCommand);

            // Register to sensor update messages
            Messenger.Default.Register<Brewery>(this, "MLTVolumeUpdate", MLTVolumeUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTBurnerUpdate", MLTBurnerUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTVolumeSetPointUpdate", MLTVolumeSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTVolumeSetPointReachedUpdate", MLTVolumeSetPointReachedUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTTempSetPointUpdate", MLTTempSetPointUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "MLTTempSetPointReachedUpdate", MLTTempSetPointReachedUpdate_MessageReceived);
        }

        private void burnerClickCommand()
        {
            Messenger.Default.Send<Brewery.vessel>(brewery.MLT, "BurnerOverride");
        }

        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the Tempperature
            brewery.MLT.Temp.Value = _brewery.MLT.Temp.Value;

            // Raise the temp related properties changed event
            RaisePropertyChanged(MLT_TempPropertyName);
            RaisePropertyChanged(Thermo_HeightPropertyName);
        }

        private void MLTVolumeUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the Volume
            brewery.MLT.Volume.Value = _brewery.MLT.Volume.Value;
            brewery.MLT.Burner.DutyCycle = _brewery.MLT.Burner.DutyCycle;

            // Raise the volume related properties changed event
            RaisePropertyChanged(MLT_VolumePropertyName);
            RaisePropertyChanged(WaterHeightPropertyName);
            RaisePropertyChanged(MLT_Duty_CyclePropertyName);
        }

        // MLT Burner Update
        private void MLTBurnerUpdate_MessageReceived(Brewery _brewery)
        {
            // Update the MLT burner state
            brewery.MLT.Burner.IsOn = _brewery.MLT.Burner.IsOn;

            // Raise the flames visibility changed event
            RaisePropertyChanged(Flames_VisibilityPropertyName);
        }

        private void MLTVolumeSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Volume.SetPoint = _brewery.MLT.Volume.SetPoint;
            RaisePropertyChanged(MLT_Volume_SetPointPropertyName);
            RaisePropertyChanged(WaterHeightSetPointPropertyName);
        }

        private void MLTVolumeSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Volume.SetPointReached = _brewery.MLT.Volume.SetPointReached;
            RaisePropertyChanged(MLT_SetPoint_Label_VisibilityPropertyName);
        }

        private void MLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Temp.SetPoint = _brewery.MLT.Temp.SetPoint;
            RaisePropertyChanged(Thermo_Height_SetPointPropertyName);
            RaisePropertyChanged(Thermo_SetPointPropertyName);
        }

        private void MLTTempSetPointReachedUpdate_MessageReceived(Brewery _brewery)
        {
            brewery.MLT.Temp.SetPointReached = _brewery.MLT.Temp.SetPointReached;
            RaisePropertyChanged(Thermo_SetPoint_VisibilityPropertyName);
        }
    }
}