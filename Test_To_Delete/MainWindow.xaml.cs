using System.Windows;
using LAB.ViewModel;
using GalaSoft.MvvmLight;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using LAB.Views;
using LAB.Debug_Tools;
using LAB.Model;

namespace LAB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        HardwareSetup HardwareSetupWindow;
        ConnectionSetup ConnectionSetupWindow;
        DebugTool debugTool;

        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessageAction>(this,"WindowOperation", WindowOperation_MessageReceived);
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }


        private void WindowOperation_MessageReceived(NotificationMessageAction msg)
        {
            if(msg.Notification == "OpenDebugTool")
            {
                debugTool = new DebugTool();
                msg.Execute();
                debugTool.Show();
            }

            if (msg.Notification == "OpenConnectionSetup")
            {
                ConnectionSetupWindow = new ConnectionSetup();
                msg.Execute();
                ConnectionSetupWindow.ShowDialog();
            }

            if (msg.Notification == "OpenHardwareSetup")
            {
                HardwareSetupWindow = new HardwareSetup();
                msg.Execute();
                HardwareSetupWindow.ShowDialog();
            }

            if(msg.Notification == "CloseHardwareSettings")
            {
                if(HardwareSetupWindow != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new System.Action(() => { HardwareSetupWindow.Close(); }));
                }
            }

            if(msg.Notification == "CloseConnectionSetup")
            {
                if(ConnectionSetupWindow != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new System.Action(() => { ConnectionSetupWindow.Close(); }));
                }
            }
        }

        private void MLTreturnPipe_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}