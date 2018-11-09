using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Microsoft.Win32;
using Sitatex.Logging.Extensions;
using Sitatex.Logging.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace Sitatex.Logging.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        ILog Log { get; }

        private readonly MainViewModel _mainViewModel = new MainViewModel();

        public static MainWindow Current { get; set; }

        public MainWindow(ILog log)
        {
            Log = log;
            InitializeComponent();

            Log.Info("MainWindow.xaml loaded");
            DataContext = _mainViewModel;
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var logFiles = GetLogFiles();
            if (logFiles == null) return;

            var lastLogFile = GetLastestLogEntries(logFiles);
            if (lastLogFile == null) return;

            _mainViewModel.AddFileMonitor(lastLogFile.FullName);
        }

        private object ConvertToList(IEnumerable<string> logEntries)
        {
            var result = new List<LogEntry>();

            if (logEntries == null || !logEntries.Any()) return result;

            foreach (var json in logEntries)
            {
                var log = new LogEntry();
                if (json.ValidateJson())
                {
                    log = LogEntry.FromJson(json);
                }
                else
                {
                    // Here we needed to prepare string to get json
                    var validJson = json;
                }
            }

            return result;
        }

        private FileInfo GetLastestLogEntries(IEnumerable<FileInfo> logFiles)
        {
            var lastLogFile = logFiles
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

            return lastLogFile;
        }

        private IEnumerable<string> ReadAllLines(string path)
        {
            var lines = new List<string>();
            try
            {
                //using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = File.OpenText(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
            return lines;
        }

        private IEnumerable<FileInfo> GetLogFiles()
        {
            var currentFolder = Directory.GetCurrentDirectory();
            var fileAppender = GetLoggerConfiguration();
            if (fileAppender == null) return null;
            var logFolder = new DirectoryInfo(Path.GetDirectoryName(fileAppender.File));
            if (logFolder == null) return null;

            return logFolder.GetFiles("*.log", SearchOption.TopDirectoryOnly);
        }

        private RollingFileAppender GetLoggerConfiguration()
        {
            var currentFolder = Directory.GetCurrentDirectory();
            var logConfigRepository = (Hierarchy)LogManager.GetRepository();
            if (logConfigRepository == null) return null;

            var fileAppender = logConfigRepository.Root.GetAppender("file");//logAppenders.FirstOrDefault(a => a is RollingFileAppender);
            if (fileAppender == null) return null;

            return ((RollingFileAppender)fileAppender);//currentAppender.ActivateOptions(); // Refresh settings of appender
        }

        protected override void OnClosed(EventArgs e)
        {
            App.Current.Shutdown();
            base.OnClosed(e);
        }

        private void AddLogClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Content)
            {
                case "Log Info":
                    {
                        Log.Info($"Button: {button.Content} was clicked!");
                    }
                    break;
                case "Log Warning":
                    {
                        Log.Warn($"Button: {button.Content} was clicked!");
                    }
                    break;
                case "Log Error":
                    {
                        Log.Error($"Button: {button.Content} was clicked!", new Exception("Exception created dynamically!"));
                    }
                    break;
                case "Log Debug":
                    {
                        Log.Debug($"Button: {button.Content} was clicked!");
                    }
                    break;
                case "Log Fatal":
                    {
                        Log.Fatal($"Button: {button.Content} was clicked!", new ApplicationException("ApplicationException created dynamically"));
                    }
                    break;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void SetupClick(object sender, RoutedEventArgs e)
        {
            Overlay.IsEnabled = false;
            new ConfigurationWindow { Owner = this }.ShowDialog();
            Overlay.IsEnabled = true;
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { CheckFileExists = false, Multiselect = true };
            if (openFileDialog.ShowDialog() != true)
                return;

            try
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    _mainViewModel.AddFileMonitor(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBoxBaseOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var fileMonitorViewModel = (FileMonitorViewModel)textBox.DataContext;

            if (fileMonitorViewModel?.IsFrozen == false)
                textBox.ScrollToEnd();
        }
    }
}
