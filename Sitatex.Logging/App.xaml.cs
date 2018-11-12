using log4net;
using Sitatex.Logging.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Unity;
using SplashScreen = Sitatex.Logging.Views.SplashScreen;

namespace Sitatex.Logging
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
    {
        // add this code
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MinimumSplashScreenTime = 1500; // Miliseconds

        [STAThread]
        public static void Main()
        {
            var app = new App
            {
                MainWindow = new MasterPage()//new MainWindow(Log) 
            };
            app.Run();
        }

        // add this code
        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            // Unity
            IUnityContainer container = new UnityContainer();

            // Inject the ILog
            container.RegisterType<ILog>()
                .RegisterInstance(typeof(ILog), LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            
            // Start first Log entry
            Log.Info("        =============  Started Logging  =============        ");
            log4net.Config.XmlConfigurator.Configure();

            container.RegisterType<IMainWindow, MainWindow>();
            
            //var mainWindow = container.Resolve<MainWindow>();
            var mainWindow = container.Resolve<MasterPage>();

            // Splash screen
            SplashScreen splashScreen = container.Resolve<SplashScreen>();
            splashScreen.Show();

            // Step 2 - Start a stop watch  
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // Step 3 - Load MainWindows but don't show it yet
            base.OnStartup(e);

            stopWatch.Stop();

            int remainingTimeToShowSplash = MinimumSplashScreenTime - (int)stopWatch.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0)
                Thread.Sleep(remainingTimeToShowSplash);

            splashScreen.Close();

            // English
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)));

            mainWindow.Show();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            OnUnhandledException(unhandledExceptionEventArgs.ExceptionObject as Exception);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            OnUnhandledException(dispatcherUnhandledExceptionEventArgs.Exception);
        }

        private void OnUnhandledException(Exception exception)
        {
            MessageBox.Show(MainWindow, $"Error: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
