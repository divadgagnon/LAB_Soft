using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace LAB.Views
{
    /// <summary>
    /// Description for HardwareSetup.
    /// </summary>
    public partial class HardwareSetup : Window
    {
        EditDigitalPinDialogView DigitalPinDialog;
        EditRefreshRateDialogView RefreshRateView;
        EditAnalogPinDialogView AnalogPinDialog;
        EditProbeColorsDialogView ProbeColorsDialog;

        public HardwareSetup()
        {
            Messenger.Default.Register<NotificationMessage>(this, "WindowOperation", MessageReceived);
            InitializeComponent();
        }

        private void MessageReceived(NotificationMessage msg)
        {
            if(msg.Notification == "OpenDigitalPinDialog")
            {
                DigitalPinDialog = new EditDigitalPinDialogView();
                DigitalPinDialog.ShowDialog();
                return;
            }

            if(msg.Notification == "OpenRefreshRateDialog")
            {
                RefreshRateView = new EditRefreshRateDialogView();
                RefreshRateView.ShowDialog();
                return;
            }

            if(msg.Notification == "OpenAnalogPinDialog")
            {
                AnalogPinDialog = new EditAnalogPinDialogView();
                AnalogPinDialog.ShowDialog();
                return;
            }

            if(msg.Notification == "OpenProbeColorsDialog")
            {
                ProbeColorsDialog = new EditProbeColorsDialogView();
                ProbeColorsDialog.ShowDialog();
                return;
            }

            if(msg.Notification == "CloseProbeColorsDiaglog")
            {
                if(ProbeColorsDialog != null)
                {
                    ProbeColorsDialog.Close();
                }
                return;
            }

            if (msg.Notification == "CloseRefreshRateDialog")
            {
                if (RefreshRateView != null)
                {
                    RefreshRateView.Close();
                }
                return;
            }

            if(msg.Notification == "CloseDigitalPinDialog")
            {
                if (DigitalPinDialog != null)
                {
                    DigitalPinDialog.Close();
                }
                return;
            }

            if(msg.Notification == "CloseAnalogPinDialog")
            {
                if(AnalogPinDialog != null)
                {
                    AnalogPinDialog.Close();
                }
                return;
            }
        }
    }
}