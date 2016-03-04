using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using LAB.Model;

namespace LAB.ViewModel
{
    public class Pump2ViewModel : ViewModelBase
    {
        // Defining model class instances
        Brewery brewery;

        // Defining RelayCommand
        public RelayCommand PumpClickCommand { get; private set; }

        // Defining Bindable property names
        public const string PumpIsOnColorPropertyName = "PumpIsOnColor";

        // Defining bindable properties
        public string PumpIsOnColor
        {
            get
            {
                if (brewery.Pump2.IsOn) { return "#FF218512"; }
                else { return "DimGray"; }
            }
        }

        // Constructor
        public Pump2ViewModel()
        {
            // Initializing model instances
            brewery = new Brewery();

            // Initializing RelayCommands
            PumpClickCommand = new RelayCommand(pumpClickCommand);

            // Registering to incoming messages
            Messenger.Default.Register<Brewery>(this, "Pump2Update", Pump2Update_MessageReceived);
        }

        // UI Methods
        private void pumpClickCommand()
        {
            Messenger.Default.Send<Brewery.pump>(brewery.Pump2, "PumpOverride");
        }

        // Message Received handling
        private void Pump2Update_MessageReceived(Brewery _brewery)
        {
            brewery.Pump2.IsOn = _brewery.Pump2.IsOn;
            RaisePropertyChanged(PumpIsOnColorPropertyName);
        }
    }
}