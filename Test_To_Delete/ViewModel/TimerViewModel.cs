using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.ViewModel
{

    public class TimerViewModel : ViewModelBase
    {
        // Model Instances
        BreweryState breweryState;

        // TimeSpan variables
        TimeSpan sessionStartTime;
        TimeSpan stepStartTime;
        TimeSpan sessionTime;
        TimeSpan stepTime;

        // Timer instance
        DispatcherTimer UpdateTimer;

        // Property Names
        public const string SessionTimePropertyName = "SessionTime";
        public const string StepTimePropertyName = "StepTime";

        // Bindable Properties
        public string SessionTime
        {
            get
            {
                return sessionTime.Hours.ToString() + ":" + String.Format("{0:00}",sessionTime.Minutes) + ":" + String.Format("{0:00}", sessionTime.Seconds);
            }
        }

        public string StepTime
        {
            get
            {
                return stepTime.Hours.ToString() + ":" + String.Format("{0:00}", stepTime.Minutes) + ":" + String.Format("{0:00}", stepTime.Seconds);
            }
        }

        public TimerViewModel()
        {
            // Initialize local variables
            breweryState = BreweryState.StandBy;
            sessionTime = new TimeSpan();
            sessionStartTime = new TimeSpan();
            stepStartTime = new TimeSpan();
            stepTime = new TimeSpan();

            // Register to incoming messages
            Messenger.Default.Register<BreweryState>(this, breweryState_MessageReceived);

            // Timer Initialisation
            UpdateTimer = new DispatcherTimer();

        }

        private void breweryState_MessageReceived(BreweryState _breweryState)
        {
            if(_breweryState != breweryState)
            {
                breweryState = _breweryState;
                if (breweryState == BreweryState.HLT_Fill) { SessionTimer(); }
                StepTimer();
            }
        }

        private void SessionTimer()
        {
            sessionStartTime = DateTime.Now.TimeOfDay;

            // Start update timer
            UpdateTimer.Interval = TimeSpan.FromMilliseconds(250);
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Start();

        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Update Session Time and Step Time
            sessionTime = DateTime.Now.TimeOfDay - sessionStartTime;
            stepTime = DateTime.Now.TimeOfDay - stepStartTime;
            RaisePropertyChanged(SessionTimePropertyName);
            RaisePropertyChanged(StepTimePropertyName);
        }

        private void StepTimer()
        {
            stepStartTime = DateTime.Now.TimeOfDay;
        }
    }
}