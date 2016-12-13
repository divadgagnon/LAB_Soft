using System.Windows.Media;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.ComponentModel;
using LAB.Model;

namespace LAB.Views
{
    /// <summary>
    /// Description for BKinPipeView.
    /// </summary>
    public partial class BKinPipeView : UserControl, INotifyPropertyChanged
    {
        private SolidColorBrush WaterColor;
        private SolidColorBrush TransparentBrush;

        public event PropertyChangedEventHandler PropertyChanged;

        // Bindable Properties
        public bool IsFilled
        {
            get { return (bool)GetValue(IsFilledProperty); }
            set { SetValue(IsFilledProperty, value); }
        }

        public SolidColorBrush FillColor
        {
            get
            {
                if (IsFilled) { return WaterColor; }
                else { return TransparentBrush; }
            }
        }

        // Dependency Properties
        public static readonly DependencyProperty IsFilledProperty = DependencyProperty.Register("IsFilled", typeof(bool), typeof(BKinPipeView), new PropertyMetadata(false, IsFilledPropertyCallBack));

        private static void IsFilledPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BKinPipeView _BKinPipeView = o as BKinPipeView;
            if (_BKinPipeView != null)
            {
                _BKinPipeView.RaisePropertyChanged("IsFilled");
                _BKinPipeView.RaisePropertyChanged("FillColor");
            }
        }

        public BKinPipeView()
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