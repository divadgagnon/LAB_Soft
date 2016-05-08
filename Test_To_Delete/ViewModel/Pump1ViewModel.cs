using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using LAB.Model;

namespace LAB.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class Pump1ViewModel : ViewModelBase
    {
        // Defining model class instances
        Brewery brewery;
        HardwareSettings hardwareSettings;

        // Defining RelayCommand
        public RelayCommand PumpClickCommand { get; private set; }

        // Defining Timers
        DispatcherTimer PrimingTimer;

        // Defining Bindable property names
        public const string PumpIsOnColorPropertyName = "PumpIsOnColor";

        // Defining bindable properties
        public string PumpIsOnColor
        {
            get
            {
                if(brewery.Pump1.IsOn) { return "#FF218512"; }
                else { return "DimGray"; }
            }
        }

        // Constructor
        public Pump1ViewModel()
        {
            // Initializing model instances
            brewery = new Brewery();
            hardwareSettings = new HardwareSettings();

            // Initializing RelayCommands
            PumpClickCommand = new RelayCommand(pumpClickCommand);

            // Initializing Timers
            PrimingTimer = new DispatcherTimer();
            PrimingTimer.Interval = TimeSpan.FromMilliseconds(500);
            PrimingTimer.Tick += PrimingTimer_Tick;

            // Registering to incoming messages
            Messenger.Default.Register<Brewery>(this, "Pump1Update", Pump1Update_MessageReceived);
            Messenger.Default.Register<Brewery.pump>(this,"PrimingUpdate", PumpPriming_MessageReceived);
        }

        // Timer Tick Events
        private void PrimingTimer_Tick(object sender, EventArgs e)
        {
            brewery.Pump1.IsOn = !brewery.Pump1.IsOn;
            RaisePropertyChanged(PumpIsOnColor);
        }

        // UI Methods
        private void pumpClickCommand()
        {
            Messenger.Default.Send<Brewery.pump>(brewery.Pump1, "PumpOverride");
        }

        // Message Received handling
        private void Pump1Update_MessageReceived(Brewery _brewery)
        {
            PrimingTimer.Stop();
            brewery.Pump1.IsOn = _brewery.Pump1.IsOn;
            RaisePropertyChanged(PumpIsOnColorPropertyName);
        }

        private void PumpPriming_MessageReceived(Brewery.pump _pump)
        {
            if(_pump.Number != 1) { return; }
            if (_pump.IsPriming)
            {
                PrimingTimer.Start();
                brewery.Pump1.IsOn = !brewery.Pump1.IsOn;
                RaisePropertyChanged(PumpIsOnColor);
            }
            else { PrimingTimer.Stop(); }
        }
    }
}