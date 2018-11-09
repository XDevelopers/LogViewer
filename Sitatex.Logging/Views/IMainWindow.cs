namespace Sitatex.Logging.Views
{
    public interface IMainWindow : IWindow
    {
        void InitializeComponent();
    }

    public interface IWindow
    {
        object DataContext { get; set; }

        void Show();

        bool Activate();

        bool? ShowDialog();

        void Close();
    }
}