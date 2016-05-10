using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;
using System.Windows.Media;

namespace LAB.ViewModel
{
    public class ProcessPlotViewModel
    {
        SeriesCollection dataSeries;
        SeriesConfiguration<ChartConfig> Config = new SeriesConfiguration<ChartConfig>();
        LineSeries HLTTemp = new LineSeries();
        LineSeries HLTTempSetPoint = new LineSeries();

        public SeriesCollection DataSeries
        {
            get { return dataSeries; }
        }

        public ProcessPlotViewModel()
        {
            Config.X(model => model.CurrentTemp);
            Config.Y(model => model.CurrentTemp);

            dataSeries = new SeriesCollection(Config);

            HLTTemp.Values = new ChartValues<double> { };
            HLTTempSetPoint.Values = new ChartValues<double> { };

            HLTTemp.PointRadius = 0;
            HLTTempSetPoint.PointRadius = 0;
            HLTTemp.Fill = new SolidColorBrush();
            HLTTempSetPoint.Fill = new SolidColorBrush();

            HLTTemp.Title = "HLT Temp.";
            HLTTempSetPoint.Title = "Set Point";
            
            dataSeries.Add(HLTTemp);
            dataSeries.Add(HLTTempSetPoint);
        }
    }

    public class ChartConfig
    {
        private TimeSpan startTime;
        private TimeSpan currentTime;
        private BreweryState currentState = BreweryState.StandBy;

        public double CurrentTemp { get; }
        public TimeSpan CurrentTime { get; }

        public ChartConfig()
        {
            startTime = new TimeSpan();
            currentTime = new TimeSpan();

            Messenger.Default.Register<BreweryState>(this, breweryState_MessageReceived);
        }

        // Incoming Messages handling
        private void breweryState_MessageReceived(BreweryState breweryState)
        {
            if(breweryState != currentState && breweryState == BreweryState.Strike_Heat)
            {
                startTime = DateTime.Now.TimeOfDay;
                Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
                Messenger.Default.Register<Brewery>(this, "HLTTempSetPointUpdate", HLTTempSetPointUpdate_MessageReceived);
            }
        }

        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }

        private void HLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }
    }
}
