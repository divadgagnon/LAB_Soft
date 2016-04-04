using System.Windows.Media;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.Views
{
    /// <summary>
    /// Description for WaterPipeView.
    /// </summary>
    public partial class HLToutPipeView : UserControl
    {
        private SolidColorBrush WaterColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976CD"));
        private SolidColorBrush TransparentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));


        public HLToutPipeView()
        {
            InitializeComponent();
            Messenger.Default.Register<Brewery.valve>(this, ValveUpdate_MessageReceived);
        }

        private void ValveUpdate_MessageReceived(Brewery.valve Valve)
        {
            if(Valve.Name != "HLTout") { return; }

            if(Valve.IsOpen)
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