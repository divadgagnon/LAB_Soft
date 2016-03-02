using System;
using System.IO;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using LAB.Model;

namespace LAB.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class HardwareSetupViewModel : ViewModelBase
    {

        #region Fields

        // Model Class Instances
        HardwareSettings hardwareSettings;

        // Relay Commands
        public RelayCommand EditSettingClickCommand { get; private set; }
        public RelayCommand SaveSettingClickCommand { get; private set; }
        public RelayCommand ConfirmPinSelectedCommand { get; private set; }
        public RelayCommand CancelClickCommand { get; private set; }
        public RelayCommand SaveAsDefault { get; private set; }

        // Path to XML Default data
        private string DefaultSettingsPath = @"..\..\Data\DefaultHardwareSettings.xml";

        // Define Property Names for RaiseProperTyChanged Events
        public const string SelectedSettingPropertyName = "SelectedSetting";
        public const string VolRefreshRatePropertyName = "VolRefreshRate";
        public const string TempRefreshRatePropertyName = "TempRefreshRate";
        public const string SelectedRefreshRatePropertyName = "SelectedRefreshRate";
        public const string OneWireBusPropertyName = "OneWireBus";
        public const string HLT_Vol_PinPropertyName = "HLT_Vol_Pin";
        public const string BK_Vol_PinPropertyName = "BK_Vol_Pin";
        public const string HLT_Burner_PinPropertyName = "HLT_Burner_Pin";
        public const string MLT_Burner_PinPropertyName = "MLT_Burner_Pin";
        public const string BK_Burner_PinPropertyName = "BK_Burner_Pin";
        public const string Pump1_PinPropertyName = "Pump1_Pin";
        public const string Pump2_PinPropertyName = "Pump2_Pin";
        public const string AirPump1_PinPropertyName = "AirPump1_Pin";
        public const string AirPump2_PinPropertyName = "AirPump2_Pin";
        public const string HLT_Temp_ProbePropertyName = "HLT_Temp_Probe";
        public const string MLT_Temp_ProbePropertyName = "MLT_Temp_Probe";
        public const string BK_Temp_ProbePropertyName = "BK_Temp_Probe";
        public const string DigitalPinsPropertyName = "DigitalPins";
        public const string AnalogPinsPropertyName = "AnalogPins";
        public const string ProbeColorsPropertyName = "ProbeColors";
        public const string SelectedPinPropertyName = "SelectedPin";
        public const string SelectedProbeColorPropertyName = "SelectedProbeColor";

        #endregion

        #region Bindable Properties

        // Define Bindable Properties
        // Name Of selected item in listbox
        //private ListBoxItem _selectedSetting = null;
        private ListBoxItem _selectedSetting;
        public ListBoxItem SelectedSetting
        {
            get
            {
                return _selectedSetting;
            }

            set
            {
                if (_selectedSetting == value)
                {
                    return;
                }

                _selectedSetting = value;
                RaisePropertyChanged(SelectedSettingPropertyName);
            }
        }

        // Selected Refresh Rate
        private int _SelectedRefreshRate;
        public int SelectedRefreshRate
        {
            get
            {
                return _SelectedRefreshRate;
            }

            set
            {
                if (_SelectedRefreshRate == value)
                {
                    return;
                }

                _SelectedRefreshRate = value;
                RaisePropertyChanged(SelectedRefreshRatePropertyName);
            }
        }


        // Refresh Rate Property
        private int _volRefreshRate;
        public int VolRefreshRate
        {
            get
            {
                return _volRefreshRate;
            }

            set
            {
                if (_volRefreshRate == value)
                {
                    return;
                }

                _volRefreshRate = value;
                RaisePropertyChanged(VolRefreshRatePropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _tempRefreshRate;
        public int TempRefreshRate
        {
            get
            {
                return _tempRefreshRate;
            }

            set
            {
                if (_tempRefreshRate == value)
                {
                    return;
                }

                _tempRefreshRate = value;
                RaisePropertyChanged(TempRefreshRatePropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _HLT_Vol_Pin;
        public int HLT_Vol_Pin
        {
            get
            {
                return _HLT_Vol_Pin;
            }

            set
            {
                if (_HLT_Vol_Pin == value)
                {
                    return;
                }

                _HLT_Vol_Pin = value;
                RaisePropertyChanged(HLT_Vol_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _BK_Vol_Pin;
        public int BK_Vol_Pin
        {
            get
            {
                return _BK_Vol_Pin;
            }

            set
            {
                if (_BK_Vol_Pin == value)
                {
                    return;
                }

                _BK_Vol_Pin = value;
                RaisePropertyChanged(BK_Vol_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _HLT_Burner_Pin;
        public int HLT_Burner_Pin
        {
            get
            {
                return _HLT_Burner_Pin;
            }

            set
            {
                if (_HLT_Burner_Pin == value)
                {
                    return;
                }

                _HLT_Burner_Pin = value;
                RaisePropertyChanged(HLT_Burner_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _MLT_Burner_Pin;
        public int MLT_Burner_Pin
        {
            get
            {
                return _MLT_Burner_Pin;
            }

            set
            {
                if (_MLT_Burner_Pin == value)
                {
                    return;
                }

                _MLT_Burner_Pin = value;
                RaisePropertyChanged(MLT_Burner_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _BK_Burner_Pin;
        public int BK_Burner_Pin
        {
            get
            {
                return _BK_Burner_Pin;
            }

            set
            {
                if (_BK_Burner_Pin == value)
                {
                    return;
                }

                _BK_Burner_Pin = value;
                RaisePropertyChanged(BK_Burner_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _Pump1_Pin;
        public int Pump1_Pin
        {
            get
            {
                return _Pump1_Pin;
            }

            set
            {
                if (_Pump1_Pin == value)
                {
                    return;
                }

                _Pump1_Pin = value;
                RaisePropertyChanged(Pump1_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _Pump2_Pin;
        public int Pump2_Pin
        {
            get
            {
                return _Pump2_Pin;
            }

            set
            {
                if (_Pump2_Pin == value)
                {
                    return;
                }

                _Pump2_Pin = value;
                RaisePropertyChanged(Pump2_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _AirPump1_Pin;
        public int AirPump1_Pin
        {
            get
            {
                return _AirPump1_Pin;
            }

            set
            {
                if (_AirPump1_Pin == value)
                {
                    return;
                }

                _AirPump1_Pin = value;
                RaisePropertyChanged(AirPump1_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private int _AirPump2_Pin;
        public int AirPump2_Pin
        {
            get
            {
                return _AirPump2_Pin;
            }

            set
            {
                if (_AirPump2_Pin == value)
                {
                    return;
                }

                _AirPump2_Pin = value;
                RaisePropertyChanged(AirPump2_PinPropertyName);
            }
        }

        // One Wire Bus Pin Property
        private string _HLT_Temp_Probe;
        public string HLT_Temp_Probe
        {
            get
            {
                return _HLT_Temp_Probe;
            }

            set
            {
                if (_HLT_Temp_Probe == value)
                {
                    return;
                }

                _HLT_Temp_Probe = value;
                RaisePropertyChanged(HLT_Temp_ProbePropertyName);
            }
        }

        // One Wire Bus Pin Property
        private string _MLT_Temp_Probe;
        public string MLT_Temp_Probe
        {
            get
            {
                return _MLT_Temp_Probe;
            }

            set
            {
                if (_MLT_Temp_Probe == value)
                {
                    return;
                }

                _MLT_Temp_Probe = value;
                RaisePropertyChanged(MLT_Temp_ProbePropertyName);
            }
        }

        // One Wire Bus Pin Property
        private string _BK_Temp_Probe;
        public string BK_Temp_Probe
        {
            get
            {
                return _BK_Temp_Probe;
            }

            set
            {
                if (_BK_Temp_Probe == value)
                {
                    return;
                }

                _BK_Temp_Probe = value;
                RaisePropertyChanged(BK_Temp_ProbePropertyName);
            }
        }

        // Digital Pin List
        private List<int> _DigitalPins;
        public List<int> DigitalPins
        {
            get
            {
                return _DigitalPins;
            }

            private set
            {
                _DigitalPins = value;
                RaisePropertyChanged(DigitalPinsPropertyName);
            }
        }

        // Analog Pin List
        private List<int> _AnalogPins;
        public List<int> AnalogPins
        {
            get
            {
                return _AnalogPins;
            }

            private set
            {
                _AnalogPins = value;
                RaisePropertyChanged(AnalogPinsPropertyName);
            }
        }

        // Selected Pin From Edit Menu
        private int _SelectedPin;
        public int SelectedPin
        {
            get
            {
                return _SelectedPin;
            }

            set
            {
                _SelectedPin = value;
                RaisePropertyChanged(SelectedPinPropertyName);
            }
        }

        // Probe colors list
        private List<string> _ProbeColors;
        public List<string> ProbeColors
        {
            get
            {
                return _ProbeColors;
            }

            private set
            {
                _ProbeColors = value;
                RaisePropertyChanged(ProbeColorsPropertyName);
            }
        }

        // Selected probe color from dialog
        private string _SelectedProbeColor;
        public string SelectedProbeColor
        {
            get
            {
                return _SelectedProbeColor;
            }
            set
            {
                _SelectedProbeColor = value;
                RaisePropertyChanged(SelectedProbeColorPropertyName);
            }
            
        }

        #endregion

        public HardwareSetupViewModel()
        {
            // Creating Model Class Instances
            hardwareSettings = new HardwareSettings();

            // Creating Relay Command Instances
            EditSettingClickCommand = new RelayCommand(editSettingClickCommand);
            SaveSettingClickCommand = new RelayCommand(saveSettingClickCommand);
            ConfirmPinSelectedCommand = new RelayCommand(confirmSettingCommand);
            CancelClickCommand = new RelayCommand(cancelClickCommand);
            SaveAsDefault = new RelayCommand(saveAsDefault);

            // Initialize Digital Pin list
            DigitalPins = new List<int>();
            for (int i=0; i<14; i++)
            {
                DigitalPins.Add(i);
            }

            // Initialize Analog Pins
            AnalogPins = new List<int>();
            for(int i=0; i<6; i++)
            {
                AnalogPins.Add(i);
            }

            // Initialize Probe Colors list
            ProbeColors = new List<string>();

            //Register to incoming messages
            Messenger.Default.Register<Probes>(this, "GetConnectedProbes", Probes_MessageReceived);

            // Set Default Values for settings
            GetSaveedSettings();
            SetDefaultSettings();
        }

        private void SetDefaultSettings()
        {
            VolRefreshRate = hardwareSettings.VolResfreshRate;
            TempRefreshRate = hardwareSettings.TempRefreshRate;
            HLT_Vol_Pin = hardwareSettings.HLT_Vol_Pin;
            BK_Vol_Pin = hardwareSettings.BK_Vol_Pin;
            HLT_Burner_Pin = hardwareSettings.HLT_Burner_Pin;
            MLT_Burner_Pin = hardwareSettings.MLT_Burner_Pin;
            BK_Burner_Pin = hardwareSettings.BK_Burner_Pin;
            Pump1_Pin = hardwareSettings.Pump1_Pin;
            Pump2_Pin = hardwareSettings.Pump2_Pin;
            AirPump1_Pin = hardwareSettings.AirPump1_Pin;
            AirPump2_Pin = hardwareSettings.AirPump2_Pin;
            HLT_Temp_Probe = "Yellow";
            MLT_Temp_Probe = "Pink";
            BK_Temp_Probe = "Orange";
        }

        private void cancelClickCommand()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseHardwareSettings"), "WindowOperation");
        }

        private void saveSettingClickCommand()
        {
            // Verify if pin configuration is possible (no multiple assignation)
            //To implement

            // Save Temperature Probes
            Probes _probe = new Probes();

            switch (HLT_Temp_Probe)
            {
                case "Yellow":
                    {
                        hardwareSettings.HLT_Temp_Probe.Address = _probe.Yellow.Address;
                        hardwareSettings.HLT_Temp_Probe.Color = _probe.Yellow.Color;
                        hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Pink":
                    {
                        hardwareSettings.HLT_Temp_Probe.Address = _probe.Pink.Address;
                        hardwareSettings.HLT_Temp_Probe.Color = _probe.Pink.Color;
                        hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Orange":
                    {
                        hardwareSettings.HLT_Temp_Probe.Address = _probe.Orange.Address;
                        hardwareSettings.HLT_Temp_Probe.Color = _probe.Orange.Color;
                        hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Yellow and Pink":
                    {
                        hardwareSettings.HLT_Temp_Probe.Address = _probe.YellowPink.Address;
                        hardwareSettings.HLT_Temp_Probe.Color = _probe.YellowPink.Color;
                        hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Yellow and Orange":
                    {
                        hardwareSettings.HLT_Temp_Probe.Address = _probe.YellowOrange.Address;
                        hardwareSettings.HLT_Temp_Probe.Color = _probe.YellowOrange.Color;
                        hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                        break;
                    }
            }

            _probe = new Probes();

            switch (MLT_Temp_Probe)
            {
                case "Yellow":
                    {
                        hardwareSettings.MLT_Temp_Probe.Address = _probe.Yellow.Address;
                        hardwareSettings.MLT_Temp_Probe.Color = _probe.Yellow.Color;
                        hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Pink":
                    {
                        hardwareSettings.MLT_Temp_Probe.Address = _probe.Pink.Address;
                        hardwareSettings.MLT_Temp_Probe.Color = _probe.Pink.Color;
                        hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Orange":
                    {
                        hardwareSettings.MLT_Temp_Probe.Address = _probe.Orange.Address;
                        hardwareSettings.MLT_Temp_Probe.Color = _probe.Orange.Color;
                        hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Yellow and Pink":
                    {
                        hardwareSettings.MLT_Temp_Probe.Address = _probe.YellowPink.Address;
                        hardwareSettings.MLT_Temp_Probe.Color = _probe.YellowPink.Color;
                        hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Yellow and Orange":
                    {
                        hardwareSettings.MLT_Temp_Probe.Address = _probe.YellowOrange.Address;
                        hardwareSettings.MLT_Temp_Probe.Color = _probe.YellowOrange.Color;
                        hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                        break;
                    }
            }

            _probe = new Probes();

            switch (BK_Temp_Probe)
            {
                case "Yellow":
                    {
                        hardwareSettings.BK_Temp_Probe.Address = _probe.Yellow.Address;
                        hardwareSettings.BK_Temp_Probe.Color = _probe.Yellow.Color;
                        hardwareSettings.BK_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Pink":
                    {
                        hardwareSettings.BK_Temp_Probe.Address = _probe.Pink.Address;
                        hardwareSettings.BK_Temp_Probe.Color = _probe.Pink.Color;
                        hardwareSettings.BK_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Orange":
                    {
                        hardwareSettings.BK_Temp_Probe.Address = _probe.Orange.Address;
                        hardwareSettings.BK_Temp_Probe.Color = _probe.Orange.Color;
                        hardwareSettings.BK_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Yellow and Pink":
                    {
                        hardwareSettings.BK_Temp_Probe.Address = _probe.YellowPink.Address;
                        hardwareSettings.BK_Temp_Probe.Color = _probe.YellowPink.Color;
                        hardwareSettings.BK_Temp_Probe.IsConnected = true;
                        break;
                    }

                case "Yellow and Orange":
                    {
                        hardwareSettings.BK_Temp_Probe.Address = _probe.YellowOrange.Address;
                        hardwareSettings.BK_Temp_Probe.Color = _probe.YellowOrange.Color;
                        hardwareSettings.BK_Temp_Probe.IsConnected = true;
                        break;
                    }
            }

            // Send the new hardwaresettings to the arduinoCommands Class
            Messenger.Default.Send<HardwareSettings>(hardwareSettings, "HardwareSettingsUpdate");

            // Close the hardware settings window
            Messenger.Default.Send<NotificationMessageAction>(new NotificationMessageAction("CloseHardwareSettings", CloseHardwareSettings_CallBack), "WindowOperation");
        }

        private void CloseHardwareSettings_CallBack()
        {
        }

        private void editSettingClickCommand()
        {
            if(SelectedSetting == null) { MessageBox.Show("Select a setting to edit"); return; }

            if (SelectedSetting.Name == "HLT_Temp_Probe" || SelectedSetting.Name == "MLT_Temp_Probe" || SelectedSetting.Name == "BK_Temp_Probe")
            {
                // Send message to open EditProbeColorsDialogView
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("OpenProbeColorsDialog"), "WindowOperation");
                return;
            }

            if(SelectedSetting.Name == "VolRefreshRate" || SelectedSetting.Name == "TempRefreshRate")
            {
                // Send message to open EditRefreshRateDialogView
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("OpenRefreshRateDialog"), "WindowOperation");
                return;
            }

            if(SelectedSetting.Name == "HLT_Vol_Pin" || SelectedSetting.Name == "BK_Vol_Pin")
            {
                // Send message to open EditAnalogPinDialogView
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("OpenAnalogPinDialog"), "WindowOperation");
                return;
            }

            if(SelectedSetting.Name == "OneWireBus")
            {
                MessageBoxResult result = MessageBox.Show("This Value is Hardcoded in the microcontroller are you sure you want to change it?", "Edit One Wire Bus", MessageBoxButton.YesNo);
                if(result != MessageBoxResult.Yes) { return; }
            }
            
            // Else send message to open EditDigitalPinDialogView
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("OpenDigitalPinDialog"), "WindowOperation");
        }

        // Save the settings as new default settings
        private void saveAsDefault()
        {
            XmlSerializer serializer = new XmlSerializer(hardwareSettings.GetType());
            StreamWriter writer = new StreamWriter(DefaultSettingsPath);

            serializer.Serialize(writer, hardwareSettings);

            serializer = null;
            writer.Close();
            writer = null;
        }

        // Get the settings saved as default
        private void GetSaveedSettings()
        {
            XmlSerializer serializer = new XmlSerializer(hardwareSettings.GetType());
            StreamReader reader = new StreamReader(DefaultSettingsPath);

            hardwareSettings = (HardwareSettings)serializer.Deserialize(reader);
        }

        // Apply changes from the edit dialog
        private void confirmSettingCommand()
        {
            switch (SelectedSetting.Name)
            {
                case "VolRefreshRate":
                    {
                        if(SelectedRefreshRate < 1000) { MessageBox.Show("The volume sensor refresh rate can't be smaller than 1000 milliseconds"); return; }
                        VolRefreshRate = SelectedRefreshRate;
                        hardwareSettings.VolResfreshRate = VolRefreshRate;
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseRefreshRateDialog"), "WindowOperation");
                        return;
                    }
                case "TempRefreshRate":
                    {
                        if(SelectedRefreshRate<2500) { MessageBox.Show("Temperature sensors refresh rate can't be smaller tahn 2500 milliseconds"); return; }
                        TempRefreshRate = SelectedRefreshRate;
                        hardwareSettings.TempRefreshRate = TempRefreshRate;
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseRefreshRateDialog"), "WindowOperation");
                        return;
                    }
                case "HLT_Vol_Pin":
                    {
                        HLT_Vol_Pin = SelectedPin;
                        hardwareSettings.HLT_Vol_Pin = HLT_Vol_Pin;
                        break;
                    }
                case "BK_Vol_Pin":
                    {
                        BK_Vol_Pin = SelectedPin;
                        hardwareSettings.BK_Vol_Pin = BK_Vol_Pin;
                        break;
                    }
                case "HLT_Burner_Pin":
                    {
                        HLT_Burner_Pin = SelectedPin;
                        hardwareSettings.HLT_Burner_Pin = HLT_Burner_Pin;
                        break;
                    }
                case "MLT_Burner_Pin":
                    {
                        MLT_Burner_Pin = SelectedPin;
                        hardwareSettings.MLT_Burner_Pin = MLT_Burner_Pin;
                        break;
                    }
                case "BK_Burner_Pin":
                    {
                        BK_Burner_Pin = SelectedPin;
                        hardwareSettings.BK_Burner_Pin = BK_Burner_Pin;
                        break;
                    }
                case "Pump1_Pin":
                    {
                        Pump1_Pin = SelectedPin;
                        hardwareSettings.Pump1_Pin = Pump1_Pin;
                        break;
                    }
                case "Pump2_Pin":
                    {
                        Pump2_Pin = SelectedPin;
                        hardwareSettings.Pump2_Pin = Pump2_Pin;
                        break;
                    }
                case "AirPump1_Pin":
                    {
                        AirPump1_Pin = SelectedPin;
                        hardwareSettings.AirPump1_Pin = AirPump1_Pin;
                        break;
                    }
                case "AirPump2_Pin":
                    {
                        AirPump2_Pin = SelectedPin;
                        hardwareSettings.AirPump2_Pin = AirPump2_Pin;
                        break;
                    }
                case "HLT_Temp_Probe":
                    {
                        Probes _probe = new Probes();

                        switch(SelectedProbeColor)
                        {
                            case "Yellow":
                                {
                                    HLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.HLT_Temp_Probe.Address = _probe.Yellow.Address;
                                    hardwareSettings.HLT_Temp_Probe.Color = _probe.Yellow.Color;
                                    hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Pink":
                                {
                                    HLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.HLT_Temp_Probe.Address = _probe.Pink.Address;
                                    hardwareSettings.HLT_Temp_Probe.Color = _probe.Pink.Color;
                                    hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Orange":
                                {
                                    HLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.HLT_Temp_Probe.Address = _probe.Orange.Address;
                                    hardwareSettings.HLT_Temp_Probe.Color = _probe.Orange.Color;
                                    hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Yellow and Pink":
                                {
                                    HLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.HLT_Temp_Probe.Address = _probe.YellowPink.Address;
                                    hardwareSettings.HLT_Temp_Probe.Color = _probe.YellowPink.Color;
                                    hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Yellow and Orange":
                                {
                                    HLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.HLT_Temp_Probe.Address = _probe.YellowOrange.Address;
                                    hardwareSettings.HLT_Temp_Probe.Color = _probe.YellowOrange.Color;
                                    hardwareSettings.HLT_Temp_Probe.IsConnected = true;
                                    break;
                                }
                        }
                        break;
                    }

                case "MLT_Temp_Probe":
                    {
                        Probes _probe = new Probes();

                        switch (SelectedProbeColor)
                        {
                            case "Yellow":
                                {
                                    MLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.MLT_Temp_Probe.Address = _probe.Yellow.Address;
                                    hardwareSettings.MLT_Temp_Probe.Color = _probe.Yellow.Color;
                                    hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Pink":
                                {
                                    MLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.MLT_Temp_Probe.Address = _probe.Pink.Address;
                                    hardwareSettings.MLT_Temp_Probe.Color = _probe.Pink.Color;
                                    hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Orange":
                                {
                                    MLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.MLT_Temp_Probe.Address = _probe.Orange.Address;
                                    hardwareSettings.MLT_Temp_Probe.Color = _probe.Orange.Color;
                                    hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Yellow and Pink":
                                {
                                    MLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.MLT_Temp_Probe.Address = _probe.YellowPink.Address;
                                    hardwareSettings.MLT_Temp_Probe.Color = _probe.YellowPink.Color;
                                    hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Yellow and Orange":
                                {
                                    MLT_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.MLT_Temp_Probe.Address = _probe.YellowOrange.Address;
                                    hardwareSettings.MLT_Temp_Probe.Color = _probe.YellowOrange.Color;
                                    hardwareSettings.MLT_Temp_Probe.IsConnected = true;
                                    break;
                                }
                        }
                        break;
                    }

                case "BK_Temp_Probe":
                    {
                        Probes _probe = new Probes();

                        switch (SelectedProbeColor)
                        {
                            case "Yellow":
                                {
                                    BK_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.BK_Temp_Probe.Address = _probe.Yellow.Address;
                                    hardwareSettings.BK_Temp_Probe.Color = _probe.Yellow.Color;
                                    hardwareSettings.BK_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Pink":
                                {
                                    BK_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.BK_Temp_Probe.Address = _probe.Pink.Address;
                                    hardwareSettings.BK_Temp_Probe.Color = _probe.Pink.Color;
                                    hardwareSettings.BK_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Orange":
                                {
                                    BK_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.BK_Temp_Probe.Address = _probe.Orange.Address;
                                    hardwareSettings.BK_Temp_Probe.Color = _probe.Orange.Color;
                                    hardwareSettings.BK_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Yellow and Pink":
                                {
                                    BK_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.BK_Temp_Probe.Address = _probe.YellowPink.Address;
                                    hardwareSettings.BK_Temp_Probe.Color = _probe.YellowPink.Color;
                                    hardwareSettings.BK_Temp_Probe.IsConnected = true;
                                    break;
                                }

                            case "Yellow and Orange":
                                {
                                    BK_Temp_Probe = SelectedProbeColor;
                                    hardwareSettings.BK_Temp_Probe.Address = _probe.YellowOrange.Address;
                                    hardwareSettings.BK_Temp_Probe.Color = _probe.YellowOrange.Color;
                                    hardwareSettings.BK_Temp_Probe.IsConnected = true;
                                    break;
                                }
                        }
                        break;
                    }
            }

            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseAnalogPinDialog"), "WindowOperation");
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseDigitalPinDialog"), "WindowOperation");
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseProbeColorsDiaglog"), "WindowOperation");
        }

        private void Probes_MessageReceived(Probes probes)
        {
            ProbeColors.Clear(); 

            if (probes.Orange.IsConnected) { ProbeColors.Add(probes.Orange.Color); }
            if(probes.Pink.IsConnected) { ProbeColors.Add(probes.Pink.Color); }
            if(probes.Yellow.IsConnected) { ProbeColors.Add(probes.Yellow.Color); }
            if(probes.YellowOrange.IsConnected) { ProbeColors.Add(probes.YellowOrange.Color); }
            if (probes.YellowPink.IsConnected) { ProbeColors.Add(probes.YellowPink.Color); }  
        }
    }
}