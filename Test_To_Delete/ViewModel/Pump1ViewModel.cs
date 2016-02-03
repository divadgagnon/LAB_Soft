using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
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

            // Registering to incoming messages
            Messenger.Default.Register<Brewery>(this, "Pump1Update", Pump1Update_MessageReceived);
        }

        // Message Received handling
        private void Pump1Update_MessageReceived(Brewery _brewery)
        {
            brewery.Pump1.IsOn = _brewery.Pump1.IsOn;
            RaisePropertyChanged(PumpIsOnColorPropertyName);
        }
    }
}