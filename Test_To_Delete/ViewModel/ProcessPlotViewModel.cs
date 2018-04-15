using System;
using System.Windows;
using LiveCharts;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using LAB.Model;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;

namespace LAB.ViewModel
{
    public class ProcessPlotViewModel : ViewModelBase
    {
        // Define Chart classes
        SeriesCollection dataSeries;

        LineSeries HLTTemp = new LineSeries();
        LineSeries HLTTempSetPoint = new LineSeries();

        LineSeries MLTTemp = new LineSeries();
        LineSeries MLTTempSetPoint = new LineSeries();

        LineSeries BKTemp = new LineSeries();
        LineSeries BKTempSetPoint = new LineSeries();

        // Define Bindable Properties
        public Func<double, string> XFormatter { get; private set; }
        public Func<double, double> YFormatter { get; private set; }

        public const string MinValuePropertyName = "MinValue";
        public const string MaxValuePropertyName = "MaxValue";

        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        private int XAxisScale { get; set; } = 600;

        // Define Relay commands for chart controls
        public RelayCommand HLTChartClickCommand;
        public RelayCommand MLTChartCLickCommand;
        public RelayCommand BKChartClickCommand;

        // Define private variables
        private double currentHLTTemp;
        private double currentHLTSetPoint;
        private double currentMLTTemp;
        private double currentMLTSetPoint;
        private double currentBKTemp;
        private double currentBKSetPoint;
        private TimeSpan startTime;
        private TimeSpan currentTime;
        private DispatcherTimer UpdateTimer = new DispatcherTimer();

        public SeriesCollection DataSeries
        {
            get { return dataSeries; }
        }

        // Constructor
        public ProcessPlotViewModel()
        {
            dataSeries = new SeriesCollection();

            HLTTemp.Values = new ChartValues<double>();
            HLTTempSetPoint.Values = new ChartValues<double>();
            MLTTemp.Values = new ChartValues<double>();
            MLTTempSetPoint.Values = new ChartValues<double>();
            BKTemp.Values = new ChartValues<double>();
            BKTempSetPoint.Values = new ChartValues<double>();

            HLTTemp.PointRadius = 0;
            HLTTempSetPoint.PointRadius = 0;
            HLTTemp.Fill = new SolidColorBrush();
            HLTTempSetPoint.Fill = new SolidColorBrush();

            MLTTemp.PointRadius = 0;
            MLTTempSetPoint.PointRadius = 0;
            MLTTemp.Fill = new SolidColorBrush();
            MLTTempSetPoint.Fill = new SolidColorBrush();

            BKTemp.PointRadius = 0;
            BKTempSetPoint.PointRadius = 0;
            BKTemp.Fill = new SolidColorBrush();
            BKTempSetPoint.Fill = new SolidColorBrush();

            HLTTemp.Title = "HLT Temp.";
            HLTTempSetPoint.Title = "Set Point";

            MinValue = 0;
            MaxValue = XAxisScale;

            dataSeries.Add(HLTTemp);
            dataSeries.Add(HLTTempSetPoint);
            dataSeries.Add(MLTTemp);
            dataSeries.Add(MLTTempSetPoint);
            dataSeries.Add(BKTemp);
            dataSeries.Add(BKTempSetPoint);

            HLTTemp.Values.Add(0.0);
            HLTTempSetPoint.Values.Add(0.0);

            MLTTemp.Visibility = Visibility.Hidden;
            MLTTempSetPoint.Visibility = Visibility.Hidden;
            MLTTemp.Values.Add(0.0);
            MLTTempSetPoint.Values.Add(0.0);

            BKTemp.Visibility = Visibility.Hidden;
            BKTempSetPoint.Visibility = Visibility.Hidden;
            BKTemp.Values.Add(0.0);
            BKTempSetPoint.Values.Add(0.0);

            HLTTemp.Values.Add(currentHLTTemp);
            HLTTempSetPoint.Values.Add(currentHLTSetPoint);

            startTime = new TimeSpan();
            currentTime = new TimeSpan();

            XFormatter = val => (val / 10).ToString();

            Messenger.Default.Register<BreweryState>(this, breweryState_MessageReceived);
            Messenger.Default.Register<List<Visibility>>(this, "PlotVisiblityUpdate", PlotVisibilityUpdate_MessageReceived);
        }

        // Incoming Messages handling
        private void breweryState_MessageReceived(BreweryState breweryState)
        {
            if (breweryState == BreweryState.Strike_Heat)
            {
                startTime = DateTime.Now.TimeOfDay;
                Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
                Messenger.Default.Register<Brewery>(this, "HLTTempSetPointUpdate", HLTTempSetPointUpdate_MessageReceived);
                Messenger.Default.Register<Brewery>(this, "MLTTempSetPointUpdate", MLTTempSetPointUpdate_MessageReceived);
                Messenger.Default.Register<Brewery>(this, "BKTempSetPointUpdate", BKTempSetPointUpdate_MessageReceived);

                UpdateTimer.Interval = TimeSpan.FromSeconds(6);
                UpdateTimer.Tick += UpdateTimer_Tick;
                UpdateTimer.Start();
            }

            if (breweryState == BreweryState.Fermenter_Transfer) { UpdateTimer.Stop(); }
        }

        private void PlotVisibilityUpdate_MessageReceived(List<Visibility> PlotVisibility)
        {
            HLTTemp.Visibility = PlotVisibility[0];
            HLTTempSetPoint.Visibility = PlotVisibility[0];

            MLTTemp.Visibility = PlotVisibility[1];
            MLTTempSetPoint.Visibility = PlotVisibility[1];

            BKTemp.Visibility = PlotVisibility[2];
            BKTempSetPoint.Visibility = PlotVisibility[2];
        }

        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            currentHLTTemp = _brewery.HLT.Temp.Value;
            currentMLTTemp = _brewery.MLT.Temp.Value;
            currentBKTemp = _brewery.BK.Temp.Value;
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }

        private void HLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            currentHLTSetPoint = _brewery.HLT.Temp.SetPoint;
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }

        private void MLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            currentMLTSetPoint = _brewery.MLT.Temp.SetPoint;
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }

        private void BKTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            currentBKSetPoint = _brewery.BK.Temp.SetPoint;
            currentTime = DateTime.Now.TimeOfDay - startTime;
        }


        // Timer Tick Events
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (HLTTemp.Values.Count > XAxisScale)
            {
                MaxValue++;
                MinValue++;

                RaisePropertyChanged(MaxValuePropertyName);
                RaisePropertyChanged(MinValuePropertyName);
            }

            HLTTemp.Values.Add(currentHLTTemp);
            MLTTemp.Values.Add(currentMLTTemp);
            BKTemp.Values.Add(currentBKTemp);

            HLTTempSetPoint.Values.Add(currentHLTSetPoint);
            MLTTempSetPoint.Values.Add(currentMLTSetPoint);
            BKTempSetPoint.Values.Add(currentBKSetPoint);
        }

    }
}
