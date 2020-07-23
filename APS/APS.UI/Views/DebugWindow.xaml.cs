using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace APS.UI
{
    public class DebugWindow : Window
    {
        private DebugViewModel _viewModel;

        public DebugWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public DebugWindow(DebugViewModel viewModel) : this()
        {
            this.DataContext = _viewModel = viewModel;
        }

        public DebugViewModel ViewModel => _viewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
