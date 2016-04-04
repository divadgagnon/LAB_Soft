using LAB.Model;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media;

namespace LAB.Views
{
    /// <summary>
    /// Description for MLTinPipeView.
    /// </summary>
    public partial class MLTinPipeView : UserControl
    {
        private SolidColorBrush WaterColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976CD"));
        private SolidColorBrush TransparentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));

        public MLTinPipeView()
        {
            InitializeComponent();
            Messenger.Default.Register<Brewery.valve>(this, ValveUpdate_MessageReceived);
        }

        private void ValveUpdate_MessageReceived(Brewery.valve Valve)
        {
            if (Valve.Name != "MLTin") { return; }

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