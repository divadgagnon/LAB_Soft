using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class BallValveViewModel : ViewModelBase
    {
        // Model Instances
        Brewery.valve Valve;

        // Relay Commands
        public RelayCommand ValveClickCommand { get; private set; }

        // Private Variables
        private string OpenRequestColor = "#FF218512";
        private string CloseRequestColor = "#FF8F0000";
        private string InactiveColor = "#FFA8A8A8";
        private string indicatorColor;

        // Bindable Property Names
        public const string HandlePositionPropertyName = "HandlePosition";
        public const string IndicatorColorPropertyName = "IndicatorColor";
        
        // Bindable Properties
        public double HandlePosition
        {
            get { if (Valve.IsOpen) { return -90; } else { return 0; } }
        }

        public string IndicatorColor
        {
            get { return indicatorColor; }
        }

        #region Constructor

        public BallValveViewModel()
        {
            // Initialize RelayCommand instances
            ValveClickCommand = new RelayCommand(valveClickCommand);

            // Initialize Default Values
            indicatorColor = InactiveColor;
        }

        #endregion

        #region UI Methods

        private void valveClickCommand()
        {
            Valve.IsOpen = !Valve.IsOpen;
            RaisePropertyChanged(HandlePositionPropertyName);
        }

        #endregion
    }
}