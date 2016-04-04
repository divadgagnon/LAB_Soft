using System.Windows.Media;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.ViewModel
{

    public partial class MLTreturnPipeView : UserControl
    {
        private SolidColorBrush WaterColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976CD"));
        private SolidColorBrush TransparentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));

        public MLTreturnPipeView()
        {
            InitializeComponent();
            Messenger.Default.Register<Brewery.valve>(this, ValveUpdate_MessageReceived);
        }

        private void ValveUpdate_MessageReceived(Brewery.valve Valve)
        {
            if (Valve.Name != "MLTreturn") { return; }

            if (Valve.IsOpen)
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