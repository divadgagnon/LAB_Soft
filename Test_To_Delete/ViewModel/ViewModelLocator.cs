/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Test_To_Delete.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using LAB.Model;
using LAB.Debug_Tools;

namespace LAB.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PortSetupViewModel>();
            SimpleIoc.Default.Register<HardwareSetupViewModel>();
            SimpleIoc.Default.Register<HLTViewModel>();
            SimpleIoc.Default.Register<MLTViewModel>();
            SimpleIoc.Default.Register<BKViewModel>();
            SimpleIoc.Default.Register<Pump1ViewModel>();
            SimpleIoc.Default.Register<Pump2ViewModel>();
            SimpleIoc.Default.Register<DebugToolViewModel>();
            SimpleIoc.Default.Register<UserActionViewModel>();
            SimpleIoc.Default.Register<SidePanelViewModel>();
            SimpleIoc.Default.Register<TimerViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public PortSetupViewModel PortSetup 
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PortSetupViewModel>();
            }
        }

        public HardwareSetupViewModel HardwareSetup
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HardwareSetupViewModel>();
            }
        }

        public HLTViewModel HLT
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HLTViewModel>();
            }
        }

        public MLTViewModel MLT
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MLTViewModel>();
            }
        }

        public BKViewModel BK
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BKViewModel>();
            }
        }

        public Pump1ViewModel Pump1
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Pump1ViewModel>();
            }
        }

        public Pump2ViewModel Pump2
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Pump2ViewModel>();
            }
        }

        public DebugToolViewModel Debug
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DebugToolViewModel>();
            }
        }

        public UserActionViewModel UserAction
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UserActionViewModel>();
            }
        }

        public SidePanelViewModel SidePanel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SidePanelViewModel>();
            }
        }

        public TimerViewModel Timer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TimerViewModel>();
            }
        }

        public BallValveViewModel BallValve
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BallValveViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}