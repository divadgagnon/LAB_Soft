using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.ViewModel
{
    public class ProcessPlotViewModel
    {
        SeriesCollection dataSeries = new SeriesCollection();
        LineSeries HLTTemp = new LineSeries();
        LineSeries HLTTempSetPoint = new LineSeries();

        public SeriesCollection DataSeries
        {
            get { return dataSeries; }
        }

        public ProcessPlotViewModel()
        {
            Messenger.Default.Register<Brewery>(this, "TemperatureUpdate", TemperatureUpdate_MessageReceived);
            Messenger.Default.Register<Brewery>(this, "HLTTempSetPointUpdate", HLTTempSetPointUpdate_MessageReceived);

            HLTTemp.Values = new ChartValues<double> { };
            HLTTempSetPoint.Values = new ChartValues<double> { };

            HLTTemp.PointRadius = 0;
            HLTTempSetPoint.PointRadius = 0;

            dataSeries.Add(HLTTemp);
            dataSeries.Add(HLTTempSetPoint);
        }

        // Incoming Messages handling
        private void TemperatureUpdate_MessageReceived(Brewery _brewery)
        {
            HLTTemp.Values.Add(_brewery.HLT.Temp.Value);
        }

        private void HLTTempSetPointUpdate_MessageReceived(Brewery _brewery)
        {
            HLTTempSetPoint.Values.Add(_brewery.HLT.Temp.SetPoint);
        }
    }
}
