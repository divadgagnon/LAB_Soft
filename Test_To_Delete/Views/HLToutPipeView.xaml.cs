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
        private SolidColorBrush Water = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976CD"));
        private SolidColorBrush MenuBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF333333"));


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
                HLToutHorizontal.Fill = Water;
                HLToutHorizontal.Opacity = 0.3;

                HLToutVertical.Fill = Water;
                HLToutVertical.Opacity = 0.3;
            }
            else
            {
                HLToutVertical.Fill = MenuBackground;
                HLToutVertical.Opacity = 1;

                HLToutHorizontal.Fill = MenuBackground;
                HLToutHorizontal.Opacity = 1;
            }
        }
    }
}