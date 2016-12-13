using LAB.Model;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;

namespace LAB.Views
{
    /// <summary>
    /// Description for MLTinPipeView.
    /// </summary>
    public partial class MLTinPipeView : UserControl, INotifyPropertyChanged
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
                if (IsFilled) { return WaterColor; }
                else { return TransparentBrush; }
            }
        }

        // Dependency Properties
        public static readonly DependencyProperty IsFilledProperty = DependencyProperty.Register("IsFilled", typeof(bool), typeof(MLTinPipeView), new PropertyMetadata(false, IsFilledPropertyCallBack));

        private static void IsFilledPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MLTinPipeView _MLTinPipeView = o as MLTinPipeView;
            if (_MLTinPipeView != null)
            {
                _MLTinPipeView.RaisePropertyChanged("IsFilled");
                _MLTinPipeView.RaisePropertyChanged("FillColor");
            }
        }

        public MLTinPipeView()
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