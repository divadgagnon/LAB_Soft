using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.ViewModel
{
    public class Pump2ViewModel : ViewModelBase
    {
        // Defining model class instances
        Brewery brewery;

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

            // Registering to incoming messages
            Messenger.Default.Register<Brewery>(this, "Pump2Update", Pump2Update_MessageReceived);
        }

        // Message Received handling
        private void Pump2Update_MessageReceived(Brewery _brewery)
        {
            brewery.Pump2.IsOn = _brewery.Pump2.IsOn;
            RaisePropertyChanged(PumpIsOnColorPropertyName);
        }
    }
}