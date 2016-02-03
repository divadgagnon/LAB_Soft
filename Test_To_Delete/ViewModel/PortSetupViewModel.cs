using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.IO.Ports;
using System.Collections.Generic;
using System.Windows.Threading;
using System;
using System.Windows;

namespace LAB.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PortSetupViewModel : ViewModelBase
    {
        #region Fields

        // Commandes
        public RelayCommand CloseConnectionSetup { get; private set; }
        public RelayCommand ConnectProbe { get; private set; }

        // Timers
        private DispatcherTimer comPortList_Timer;

        #endregion

        #region Bindable Properties

        // Define Property Names for RaisePropertyChanged events
        public const string comPortListPropertyName = "comPortList";
        public const string SelectedcomPortPropertyName = "SelectedcomPort";
        public const string ConnectButtonContentPropertyName = "ConnectButtonContent";

        // comPort list Property
        private List<string> _comPortList = new List<string>();
        public List<string> comPortList
        {
            get
            {
                return _comPortList;
            }

            set
            {
                if (_comPortList == value)
                {
                    return;
                }

                _comPortList = value;
                RaisePropertyChanged(comPortListPropertyName);
            }
        }

        // Selected comPort Property
        private string _selectedPort = string.Empty;
        public string SelectedcomPort
        {
            get
            {
                return _selectedPort;
            }

            set
            {
                if (_selectedPort == value)
                {
                    return;
                }

                _selectedPort = value;
                RaisePropertyChanged(SelectedcomPortPropertyName);
                Messenger.Default.Send<string>(_selectedPort, "SelectedcomPort");
            }
        }


        // Connect Button Content Property
        private string _connectButtonContent = string.Empty;
        public string ConnectButtonContent
        {
            get
            {
                return _connectButtonContent;
            }

            set
            {
                if (_connectButtonContent == value)
                {
                    return;
                }

                _connectButtonContent = value;
                RaisePropertyChanged(ConnectButtonContentPropertyName);
            }
        }

        #endregion

        #region Constructor

        public PortSetupViewModel()
        {
            // Commands
            CloseConnectionSetup = new RelayCommand(closeConnectionSetup);
            ConnectProbe = new RelayCommand(connectBrewery);

            // Message Registers
            Messenger.Default.Register<bool>(this, "ConnectButtonContent", ConnectButtonContent_MessageReceived);

            // Serial comPort List Refresh Timer
            comPortList_Timer = new DispatcherTimer();
            comPortList_Timer.Interval = TimeSpan.FromMilliseconds(500);
            comPortList_Timer.Tick += ComPortList_Timer_Tick;
            comPortList_Timer.Start();
        }

        #endregion

        #region Timer Tick Events

        private void ComPortList_Timer_Tick(object sender, EventArgs e)
        {
            comPortList.Clear();

            foreach (string port in SerialPort.GetPortNames())
            {
                //if(port != "COM4" && port != "COM3")
                //{
                    comPortList.Add(port);
                //}
            }

            if(comPortList.Count == 1) { SelectedcomPort = comPortList[0]; }
            RaisePropertyChanged("comPortList");
        }

        #endregion

        #region Send Messages to MainView

        private void connectBrewery()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Connect"), "BreweryCommand");
        }

        private void closeConnectionSetup()
        {
            comPortList_Timer.Stop();
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseConnectionSetup"), "WindowOperation");
        }

        #endregion

        #region Received Messages

        private void ConnectButtonContent_MessageReceived(bool IsConnected)
        {
            if(IsConnected)
            {
                closeConnectionSetup();
                ConnectButtonContent = "Disconnect";
                return;
            }
            ConnectButtonContent = "Connect";
        }

        #endregion
    }
}