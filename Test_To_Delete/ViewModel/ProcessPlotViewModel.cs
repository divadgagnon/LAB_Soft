using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;
using System.Windows.Media;
using System.Windows.Threading;

namespace LAB.ViewModel
{
    public class ProcessPlotViewModel
    {
        SeriesCollection dataSeries;
        LineSeries HLTTemp = new LineSeries();
        LineSeries HLTTempSetPoint = new LineSeries();

        public Func<double,string> XFormatter { get; set; }
        public Func<double, double> YFormatter { get; set; }

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
            if(HLTTemp.Values.Count > 600) { HLTTemp.Values.RemoveAt(0); }
            if(HLTTempSetPoint.Values.Count > 600) { HLTTempSetPoint.Values.RemoveAt(0); }

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
