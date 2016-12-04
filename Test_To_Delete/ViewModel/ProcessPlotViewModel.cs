using System;
using System.Windows.Input;
using LiveCharts;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using LAB.Model;
using System.Windows.Media;
using System.Windows.Threading;

namespace LAB.ViewModel
{
    public class ProcessPlotViewModel : ViewModelBase
    {
        // Define Chart classes
        SeriesCollection dataSeries;
        LineSeries HLTTemp = new LineSeries();
        LineSeries HLTTempSetPoint = new LineSeries();

        // Define Bindable Properties
        public Func<double, string> XFormatter { get; private set; }
        public Func<double, double> YFormatter { get; private set; }

        public const string MinValuePropertyName = "MinValue";
        public const string MaxValuePropertyName = "MaxValue";

        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        private int XAxisScale { get; set; } = 600;

        // Define private variables
        private double currentTemp;
        private double currentSetPoint;
        private TimeSpan startTime;
        private TimeSpan currentTime;
        private BreweryState currentState = BreweryState.StandBy;
        private DispatcherTimer UpdateTimer = new DispatcherTimer();

        public SeriesCollection DataSeries
        {
            get { return dataSeries; }
        }

        public ProcessPlotViewModel()
        {
            dataSeries = new SeriesCollection();

            HLTTemp.Values = new ChartValues<double>();
            HLTTempSetPoint.Values = new ChartValues<double>();

            HLTTemp.PointRadius = 0;
            HLTTempSetPoint.PointRadius = 0;
            HLTTemp.Fill = new SolidColorBrush();
            HLTTempSetPoint.Fill = new SolidColorBrush();

            HLTTemp.Title = "HLT Temp.";
            HLTTempSetPoint.Title = "Set Point";

            MinValue = 0;
            MaxValue = XAxisScale;
            
            dataSeries.Add(HLTTemp);
            dataSeries.Add(HLTTempSetPoint);
            startTime = new TimeSpan();
            currentTime = new TimeSpan();

            HLTTemp.Values.Add(currentTemp);
            HLTTempSetPoint.Values.Add(currentSetPoint);

            XFormatter = val => (val / 10).ToString();

            Messenger.Default.Register<BreweryState>(this, breweryState_MessageReceived);
        }

        // Incoming Messages handling
        private void breweryState_MessageReceived(BreweryState breweryState)
        {
            if (breweryState != currentState && breweryState == BreweryState.Strike_Heat)
            {
                startTime = DateTime.Now.TimeOfDay;
                Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
                Messenger.Default.Register<Brewery>(this, "HLTTempSetPointUpdate", HLTTempSetPointUpdate_MessageReceived);

                UpdateTimer.Interval = TimeSpan.FromSeconds(6);
                UpdateTimer.Tick += UpdateTimer_Tick;
                UpdateTimer.Start();
            }

            if(breweryState == BreweryState.Boil) { UpdateTimer.Stop(); }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if(HLTTemp.Values.Count > XAxisScale)
            {
                MaxValue++;
                MinValue++;

                RaisePropertyChanged(MaxValuePropertyName);
                RaisePropertyChanged(MinValuePropertyName);
            }

            HLTTemp.Values.Add(currentTemp);
            HLTTempSetPoint.Values.Add(currentSetPoint);
        }

        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            currentTemp = _brewery.HLT.Temp.Value;
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }

        private void HLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            currentSetPoint = _brewery.HLT.Temp.SetPoint;
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }
    }
}
