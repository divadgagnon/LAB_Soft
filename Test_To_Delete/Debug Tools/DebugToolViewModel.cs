using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;
using System;
using System.Windows.Threading;

namespace LAB.Debug_Tools
{   
    public class DebugToolViewModel : ViewModelBase
    {
        // Model Instances
        Brewery brewery;

        // Timer Instances
        DispatcherTimer UpdateTimer;

        public double HLTVOL { set { brewery.HLT.Volume.Value = value; } }
        public double MLTVOL { set { brewery.MLT.Volume.Value = value; } }
        public double BKVOL { set { brewery.BK.Volume.Value = value; } }
        public double HLTTEMP { set { brewery.HLT.Temp.Value = value; } }
        public double MLTTEMP { set { brewery.MLT.Temp.Value = value; } }
        public double BKTEMP { set { brewery.BK.Temp.Value = value; } }

        public DebugToolViewModel()
        {
            // Model Instances
            brewery = new Brewery();

            // Timer Instances
            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            UpdateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, System.EventArgs e)
        {
            Messenger.Default.Send<Brewery>(brewery, "TemperatureUpdate");
            Messenger.Default.Send<Brewery>(brewery, "VolumeUpdate");
        }
    }
}