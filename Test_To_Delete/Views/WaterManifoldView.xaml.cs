using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace LAB.Views
{

    public partial class WaterManifoldView : UserControl, INotifyPropertyChanged
    {
        private SolidColorBrush WaterColor;
        private SolidColorBrush TransparentBrush;

        public event PropertyChangedEventHandler PropertyChanged;

        // Bindable Properties
        public bool Valve3IsOpen
        {
            get { return (bool)GetValue(Valve3IsOpenProperty); }
            set { SetValue(Valve3IsOpenProperty, value); }
        }

        public bool Valve4IsOpen
        {
            get { return (bool)GetValue(Valve4IsOpenProperty); }
            set { SetValue(Valve4IsOpenProperty, value); }
        }

        public SolidColorBrush FillColor
        {
            get
            {
                if (Valve3IsOpen||Valve4IsOpen) { return WaterColor; }
                else { return TransparentBrush; }
            }
        }

        // Dependency Properties
        public static readonly DependencyProperty Valve3IsOpenProperty = DependencyProperty.Register("Valve3IsOpen", typeof(bool), typeof(WaterManifoldView), new PropertyMetadata(false, ValveIsOpenCallBack));
        public static readonly DependencyProperty Valve4IsOpenProperty = DependencyProperty.Register("Valve4IsOpen", typeof(bool), typeof(WaterManifoldView), new PropertyMetadata(false, ValveIsOpenCallBack));

        private static void ValveIsOpenCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WaterManifoldView _WaterManifoldView = o as WaterManifoldView;

            if (_WaterManifoldView != null)
            {
                _WaterManifoldView.RaisePropertyChanged("Valve3IsOpen");
                _WaterManifoldView.RaisePropertyChanged("Valve4IsOpen");
                _WaterManifoldView.RaisePropertyChanged("FillColor");
            }
        }

        public WaterManifoldView()
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