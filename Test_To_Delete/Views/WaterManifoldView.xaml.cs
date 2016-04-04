using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.Views
{

    public partial class WaterManifoldView : UserControl
    {
        private SolidColorBrush WaterColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976CD"));
        private SolidColorBrush TransparentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));

        private Brewery.valve _MLTreturn;
        private Brewery.valve _BKin;

        public WaterManifoldView()
        {
            InitializeComponent();

            _MLTreturn = new Brewery.valve();
            _BKin = new Brewery.valve();

            _MLTreturn.Name = "MLTreturn";
            _BKin.Name = "BKin";

            Messenger.Default.Register<Brewery.valve>(this, ValveUpdate_MessageReceived);
        }

        private void ValveUpdate_MessageReceived(Brewery.valve Valve)
        {
            if (Valve.Name != "MLTreturn" && Valve.Name != "BKin") { return; }

            if (Valve.Name == "MLTreturn") { _MLTreturn.IsOpen = Valve.IsOpen; }
            if(Valve.Name == "BKin") { _BKin.IsOpen = Valve.IsOpen; }

            if (_BKin.IsOpen || _MLTreturn.IsOpen)
            {
                Water.Fill = WaterColor;
            }
            else
            {
                Water.Fill = TransparentBrush;
            }
        }
    }
}