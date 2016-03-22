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
    public partial class BallValveView
    {
        // Timers
        public DispatcherTimer FlashingIndicatorTimer;

        // Timer Variable
        private bool ActiveIndicator;

        // Private Variables
        private SolidColorBrush IndicatorColor;
        private SolidColorBrush OpenRequestColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF218512"));
        private SolidColorBrush CloseRequestColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8F0000"));
        private SolidColorBrush InactiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));

        // Control Properties
        private bool isOpen;
        public bool IsOpen
        {
            set
            {
                isOpen = value;
                HandlePositionUpdate();
            }
            get
            {
                return isOpen;
            }
        }

        private bool openRequest;
        public bool OpenRequest
        {
            set
            {
                openRequest = value;
                if(value) { ActionRequest(); }
                else { FlashingIndicatorTimer.Stop(); HandlePositionUpdate(); }
            }
        }

        private bool closeRequest;
        public bool CloseRequest
        {
            set
            {
                closeRequest = value;
                ActionRequest();
            }
        }

        #region Constructor

        public BallValveView()
        {
            InitializeComponent();
            this.DataContext = this;

            // Message Registering
            Messenger.Default.Register<Brewery.valve>(this, ValveUpdate_MessageReceived);

            // Timers
            FlashingIndicatorTimer = new DispatcherTimer();
            FlashingIndicatorTimer.Interval = TimeSpan.FromMilliseconds(1000);
            FlashingIndicatorTimer.Tick += FlashingIndicatorTimer_Tick;

            // Set Default Colors
            HandlePositionUpdate();

        }

        private void FlashingIndicatorTimer_Tick(object sender, EventArgs e)
        {
            if(ActiveIndicator)
            {
                Handle.Fill = InactiveColor;
                Handle.Stroke = InactiveColor;
                Indicator.Fill = InactiveColor;
                ActiveIndicator = false;
            }
            else
            {
                Handle.Fill = IndicatorColor;
                Handle.Stroke = IndicatorColor;
                Indicator.Fill = IndicatorColor;
                ActiveIndicator = true;
            }
        }

        #endregion

        #region UI Methods

        private void HandlePositionUpdate()
        {
            RotateTransform rotation = new RotateTransform();
            rotation.CenterX = 180;

            if(isOpen)
            {
                rotation.Angle = -90;
                Indicator.Fill =OpenRequestColor;
                Handle.Fill = OpenRequestColor;
                Handle.Stroke = OpenRequestColor;
            }
            else
            {
                rotation.Angle = 0;
                Indicator.Fill = CloseRequestColor;
                Handle.Fill = CloseRequestColor;
                Handle.Stroke = CloseRequestColor;
            }

            Handle.RenderTransform = rotation;
        }

        private void ActionRequest()
        {
            if (openRequest)
            {
                IndicatorColor = OpenRequestColor;
                FlashingIndicatorTimer.Start();
            }
            if(closeRequest)
            {
                IndicatorColor = CloseRequestColor;
                FlashingIndicatorTimer.Start();
            }
        }

        #endregion

        private void ValveUpdate_MessageReceived(Brewery.valve valve)
        {
            if (valve.Name == Name)
            {
                IsOpen = valve.IsOpen;
                OpenRequest = valve.Request.Open;
                CloseRequest = valve.Request.Close;
            }
        }

    }
}