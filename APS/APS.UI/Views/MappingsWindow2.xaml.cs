using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace APS.UI
{
    public class MappingsWindow2 : Window
    {
        private MappingsViewModel _viewModel;

        public MappingsWindow2()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public MappingsWindow2(MappingsViewModel viewModel) : this()
        {
            this.DataContext = _viewModel = viewModel;
        }

        public MappingsViewModel ViewModel => _viewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
