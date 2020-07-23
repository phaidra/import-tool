using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace APS.UI
{
    public class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.DataContext = _viewModel = new MainWindowViewModel(this);
            _viewModel.CloseAction = () => { Close(); };
            this.Opened += MainWindow_Opened;
        }

        private async void MainWindow_Opened(object sender, System.EventArgs e)
        {
            await _viewModel.Login();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}