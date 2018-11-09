using Sitatex.Logging.ViewModels;
using System.Windows;

namespace Sitatex.Logging.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private readonly ConfigurationViewModel _configurationViewModel = new ConfigurationViewModel();

        public ConfigurationWindow()
        {
            InitializeComponent();

            DataContext = _configurationViewModel;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (FontListBox.SelectedItem != null)
            {
                FontListBox.ScrollIntoView(FontListBox.SelectedItem);
            }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
