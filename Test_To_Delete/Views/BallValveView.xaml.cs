using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB.Views
{
    /// <summary>
    /// Description for BallValveView.
    /// </summary>
    public partial class BallValveView : UserControl, INotifyPropertyChanged
    {
        // Timers
        public DispatcherTimer FlashingIndicatorTimer;

        // Timer Variable
        private bool ActiveIndicator;

        // Private Variables
        public SolidColorBrush IndicatorColor;
        public SolidColorBrush OpenRequestColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF218512"));
        public SolidColorBrush CloseRequestColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8F0000"));
        public SolidColorBrush InactiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));

        // Private variables
        private RotateTransform rotation;
        private SolidColorBrush handleColor;

        // Property Changed event defintion
        public event PropertyChangedEventHandler PropertyChanged;

        // External Control Properties
        public bool IsOpen
        {
            set { SetValue(IsOpenProperty, value); }
            get { return (bool)GetValue(IsOpenProperty); }
        }

        public bool OpenRequest
        {
            get { return (bool)GetValue(OpenRequestProperty); }
            set { SetValue(OpenRequestProperty, value); }
        }

        public bool CloseRequest
        {
            get { return (bool)GetValue(CloseRequestProperty); }
            set { SetValue(CloseRequestProperty, value); }
        }

        // Dependency Property Definitions
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(BallValveView), new PropertyMetadata(false, IsOpenPropertyCallBack));
        public static readonly DependencyProperty OpenRequestProperty = DependencyProperty.Register("OpenRequest", typeof(bool), typeof(BallValveView), new PropertyMetadata(false, OpenRequestPropertyCallBack));
        public static readonly DependencyProperty CloseRequestProperty = DependencyProperty.Register("CloseRequest", typeof(bool), typeof(BallValveView), new PropertyMetadata(false, CloseRequestPropertyCallBack));

        // Dependency Properties Callbacks
        private static void IsOpenPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BallValveView _BallValveView = o as BallValveView;

            if(_BallValveView != null)
            {
                _BallValveView.HandlePositionUpdate();
            }
        }

        private static void OpenRequestPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BallValveView _BallValveView = o as BallValveView;

            if (_BallValveView != null)
            {
                if (_BallValveView.OpenRequest) { _BallValveView.ActionRequest(); }
                else { _BallValveView.FlashingIndicatorTimer.Stop(); _BallValveView.HandlePositionUpdate(); }
            }
        }

        private static void CloseRequestPropertyCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BallValveView _BallValveView = o as BallValveView;

            if (_BallValveView != null)
            {
                if (_BallValveView.CloseRequest) { _BallValveView.ActionRequest(); }
                else { _BallValveView.FlashingIndicatorTimer.Stop(); _BallValveView.HandlePositionUpdate(); }
            }
        }

        // Internal Control Properties
        public RotateTransform Rotation
        {
            get { return rotation; }
        }

        public SolidColorBrush HandleColor
        {
            get { return handleColor; }
        }

        #region Constructor

        public BallValveView()
        {
            InitializeComponent();

            // Timers
            FlashingIndicatorTimer = new DispatcherTimer();
            FlashingIndicatorTimer.Interval = TimeSpan.FromMilliseconds(1000);
            FlashingIndicatorTimer.Tick += FlashingIndicatorTimer_Tick;

            // Initialize local variables
            rotation = new RotateTransform();

            // Set Default Colors
            HandlePositionUpdate();

        }

        private void FlashingIndicatorTimer_Tick(object sender, EventArgs e)
        {
            if(ActiveIndicator)
            {
                handleColor = InactiveColor;
                ActiveIndicator = false;
            }
            else
            {
                handleColor = IndicatorColor;
                ActiveIndicator = true;
            }

            RaisePropertyChanged("HandleColor");
        }

        #endregion

        #region UI Methods

        private void HandlePositionUpdate()
        {
            rotation.CenterX = 180;

            if(IsOpen)
            {
                rotation.Angle = -90;
                handleColor = OpenRequestColor;
            }
            else
            {
                rotation.Angle = 0;
                handleColor = CloseRequestColor;
            }

            RaisePropertyChanged("Rotation");
            RaisePropertyChanged("HandleColor");

        }

        private void ActionRequest()
        {
            if (OpenRequest)
            {
                IndicatorColor = OpenRequestColor;
                FlashingIndicatorTimer.Start();
            }
            if(CloseRequest)
            {
                IndicatorColor = CloseRequestColor;
                FlashingIndicatorTimer.Start();
            }
        }

        #endregion

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}