using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using LAB.Model;

namespace LAB.Views
{
    /// <summary>
    /// Description for WaterPipeView.
    /// </summary>
    public partial class MLToutPipeView : UserControl, INotifyPropertyChanged
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
        public static readonly DependencyProperty IsFilledProperty = DependencyProperty.Register("IsFilled", typeof(bool), typeof(MLToutPipeView), new PropertyMetadata(false, IsFilledPropertyCallBack));

        private static void IsFilledPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MLToutPipeView _MLToutPipeView = o as MLToutPipeView;

            if(_MLToutPipeView != null)
            {
                _MLToutPipeView.RaisePropertyChanged("IsFilled");
                _MLToutPipeView.RaisePropertyChanged("FillColor");
            }
        }

        public MLToutPipeView()
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