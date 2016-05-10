using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Windows.Threading;
using LAB.Model;
using System;

namespace LAB.ViewModel
{

    public class UserActionViewModel : ViewModelBase
    {
        // Initializing Models
        Ingredients ingredients;

        // Initializing Private Variables
        private string displayText;
        private bool holdButtonIsEnabled;
        private bool proceedButtonIsEnabled;
        private bool userActionIsRequired;
        private bool isVisible = false;

        // Initializing Commands
        public RelayCommand ProceedClickCommand { get; private set; }

        // Initializing Timers
        DispatcherTimer UserActionAlarmTimer;

        // Initializing Property Names strings
        public const string DisplayTextPropertyName = "DisplayText";
        public const string HoldButtonIsEnabledPropertyName = "HoldButtonIsEnabled";
        public const string ProceedButtonIsEnabledPropertyName = "ProceedButtonIsEnabled";
        public const string UserActionAlarmPropertyName = "UserActionAlarm";
        public const string IsVisiblePropertyName = "IsVisible";

        // Bindable Properties
        public string DisplayText
        {
            get {return displayText;}

            set
            {
                if(displayText == value) { return; }
                displayText = value;
                RaisePropertyChanged(DisplayTextPropertyName);
            }
        }

        public bool ProceedButtonIsEnabled
        {
            get { return proceedButtonIsEnabled; }

            set
            {
                if (proceedButtonIsEnabled == value) { return; }
                proceedButtonIsEnabled = value;
                RaisePropertyChanged(ProceedButtonIsEnabledPropertyName);
            }
        }

        public string UserActionAlarm
        {
            get
            {
                if(userActionIsRequired) { return "Visible"; }
                else { return "Hidden"; }
            }
        }

        public string IsVisible
        {
            get
            {
                if(isVisible) { return "Visible"; }
                else { return "Hidden"; }
            }
        }

        // Constructor
        public UserActionViewModel()
        {
            // Create Models Instances
            ingredients = new Ingredients();

            // Create RelayCommand instances
            ProceedClickCommand = new RelayCommand(proceedClickCommand);

            // Create Timer instances
            UserActionAlarmTimer = new DispatcherTimer();

            // Registering to relevant messages
            Messenger.Default.Register<UserAlarm>(this, SetUserActionAlarm);
            Messenger.Default.Register<Ingredients>(this, Ingredients_MessageReceived);
        }

        // Proceed button click command
        private void proceedClickCommand()
        {
            UserAlarm _userAlarm = new UserAlarm();
            _userAlarm.ProceedIsPressed = true;

            displayText = "";
            proceedButtonIsEnabled = false;
            userActionIsRequired = false;
            isVisible = false;
            RaisePropertyChanged(IsVisiblePropertyName);
            RaisePropertyChanged(DisplayTextPropertyName);
            RaisePropertyChanged(UserActionAlarmPropertyName);
            UserActionAlarmTimer.Stop();

            Messenger.Default.Send<UserAlarm>(_userAlarm, "UserAlarmReturn");
        }

        // Message Received Handling
        private void SetUserActionAlarm(UserAlarm userAlarm)
        {
            if (userAlarm.IsActive)
            {
                isVisible = true;
                RaisePropertyChanged(IsVisiblePropertyName);

                if (userAlarm.VisualAlarm)
                {
                    UserActionAlarmTimer.Interval = System.TimeSpan.FromMilliseconds(1000);
                    UserActionAlarmTimer.Tick += UserActionAlarmTimer_Tick;
                    UserActionAlarmTimer.Start();
                }
                UpdateUserAlarmText(userAlarm);
            }
        }

        private void Ingredients_MessageReceived(Ingredients _ingredients)
        {
            ingredients = _ingredients;
        }

        // Timer Tick Events

        private void UserActionAlarmTimer_Tick(object sender, System.EventArgs e)
        {
            userActionIsRequired = !userActionIsRequired;
            RaisePropertyChanged(UserActionAlarmPropertyName);
        }

        private void UpdateUserAlarmText(UserAlarm userAlarm)
        {
            switch(userAlarm.CurrentState)
            {

                case BreweryState.StandBy:
                    {
                        displayText = "Standing by, select proceed to start the brewing session.";
                        RaisePropertyChanged(DisplayTextPropertyName);
                        proceedButtonIsEnabled = true;
                        break;
                    }

                case BreweryState.HLT_Fill:
                    {
                        displayText = "Add " + Math.Round(userAlarm.ProcessData.Session.TotalWaterNeeded,1) 
                            + " l of water to the hot liquor tank.";
                        RaisePropertyChanged(DisplayTextPropertyName);
                        proceedButtonIsEnabled = false;
                        break;
                    }

                case BreweryState.Strike_Heat:
                    {
                        if (userAlarm.AlarmType == "Pilot")
                        {
                            displayText = "Verify that all three pilot lights for each burner. Resume the session by clicking the " 
                                + "proceed button once all pilot lights are verified";
                            RaisePropertyChanged(DisplayTextPropertyName);
                            proceedButtonIsEnabled = true;
                        }
                        break;
                    }

                case BreweryState.Strike_Transfer:
                    {
                        if(userAlarm.AlarmType == "Priming")
                        {
                            displayText = "Waiting for pump priming delay to end...";
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }
                        else if(userAlarm.AlarmType == "Transfering")
                        {
                            displayText = "Transfering " + Math.Round(userAlarm.ProcessData.MashSteps[0].Volume, 1) 
                                + " l of strike water to MLT";
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }
                        break;
                    }

                case BreweryState.DoughIn:
                    {
                        if (userAlarm.AlarmType == "DoughIn")
                        {
                            displayText = "Add : \n";
                            foreach (var malt in ingredients.Malts)
                            {
                                displayText = displayText + Math.Round(malt.Quantity,3) + " Kg of " + malt.Name + "\n";
                            }
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }
                        break;
                    }

                case BreweryState.Mash:
                    {
                        break;
                    }

                case BreweryState.Sparge:
                    {
                        if(userAlarm.AlarmType == "MissingSpargeWater")
                        {
                            displayText = "Target sparge wolume could not be reached" + "\n" + "total HLT volume was missing "
                                + Math.Round(userAlarm.NumData,2) + " l";
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }

                        else
                        {
                            displayText = "Sparging with " + Math.Round(userAlarm.ProcessData.Sparge.Volume, 1) + " l of water at "
                                + Math.Round(userAlarm.ProcessData.Sparge.Temp, 1) + " °C";
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }
                        break;
                    }
                case BreweryState.Boil:
                    {
                        if(userAlarm.AlarmType == "BoilOver")
                        {
                            displayText = "Watch for Boil Over, reduce heat once boiling";
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }

                        if(userAlarm.AlarmType == "BoilTimer")
                        {
                            int minStep = userAlarm.RemainingTime.Minutes;
                            int secStep = userAlarm.RemainingTime.Seconds;

                            displayText = "Boil Time Remaining" + "\n" + minStep + " : " + String.Format("{0:00}", secStep);
                            RaisePropertyChanged(DisplayTextPropertyName);
                        }
                        break;
                    }
                case BreweryState.Chill:
                    {
                        displayText = "Chill the wort approximately to " + Math.Round(userAlarm.ProcessData.Fermentation.Temp, 1) 
                            + " °C" + "\n and transfer it to a fermentation vessel";
                        RaisePropertyChanged(DisplayTextPropertyName);
                        break;
                    }

                case BreweryState.Fermenter_Transfer:
                    {
                        displayText = "Ferment at " + Math.Round(userAlarm.ProcessData.Fermentation.Temp, 1) + " °C for " 
                            + Math.Round(userAlarm.ProcessData.Fermentation.Age, 0) + " days.";
                        RaisePropertyChanged(DisplayTextPropertyName);
                        break;
                    }
            }
        }
    }
}