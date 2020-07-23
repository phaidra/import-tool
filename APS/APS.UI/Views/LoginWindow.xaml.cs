using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace APS.UI
{
    public class LoginWindow : Window
    {
        private LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public LoginWindow(LoginViewModel loginViewModel) : this()
        {
            this.DataContext = _viewModel = loginViewModel;
            this.Opened += LoginWindow_Opened;
        }

        private void LoginWindow_Opened(object sender, System.EventArgs e)
        {
            _viewModel.ExecuteAutoLogon();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}