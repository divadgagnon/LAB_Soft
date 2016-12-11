using System.Windows.Media;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using LAB.Model;

namespace LAB.Views
{
    /// <summary>
    /// Description for WaterPipeView.
    /// </summary>
    public partial class HLToutPipeView : UserControl, INotifyPropertyChanged
    {
        private SolidColorBrush WaterColor;
        private SolidColorBrush TransparentBrush;
        public event PropertyChangedEventHandler PropertyChanged;

        // Bindable properties
        public bool IsFilled
        {
            get { return (bool)GetValue(IsFilledProperty); }
            set { SetValue(IsFilledProperty, value); }
        }

        // Private control properties
        public SolidColorBrush FillColor
        {
            get
            {
                if(IsFilled) { return WaterColor; }
                else { return TransparentBrush; }
            }
        }

        // Dependency Properties definition
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register("IsFilled", typeof(bool), typeof(HLToutPipeView), new PropertyMetadata(false, IsFilledPropertyCallBack));

        private static void IsFilledPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HLToutPipeView _HLToutPipeView = o as HLToutPipeView;
            if(_HLToutPipeView != null)
            {
                _HLToutPipeView.RaisePropertyChanged("IsFilled");
                _HLToutPipeView.RaisePropertyChanged("FillColor");
            }
        }

        public HLToutPipeView()
        {
            InitializeComponent();

            WaterColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976CD"));
            TransparentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}