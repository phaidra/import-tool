using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace APS.UI
{
    public class PhaidraFieldBrowserWindow : Window
    {
        private PhaidraFieldBrowserViewModel _viewModel;
        private TextBox _txtFilter;

        public PhaidraFieldBrowserWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _txtFilter = this.FindControl<TextBox>("txtFilter");
            this.Opened += PhaidraFieldBrowserWindow_Opened;
        }

        private void PhaidraFieldBrowserWindow_Opened(object sender, EventArgs e)
        {
            _txtFilter.Focus();
        }

        public PhaidraFieldBrowserWindow(PhaidraFieldBrowserViewModel viewModel) : this()
        {
            this.DataContext = _viewModel = viewModel;
        }

        public PhaidraFieldBrowserViewModel ViewModel => _viewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void lstFields_DoubleTapped(object sender, RoutedEventArgs e)
        {
            _viewModel?.SelectCommand?.Execute(null);
        }
    }
}
