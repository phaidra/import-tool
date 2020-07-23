using APS.Lib.Helper;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Reflection;

namespace APS.UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            InitLog();
            Lib.PhaidraAttributesCache.Init(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName);
            AvaloniaXamlLoader.Load(this);
        }


        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
            //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            //    singleView.MainView = new MainView();
            base.OnFrameworkInitializationCompleted();
            new Avalonia.Controls.Grid();
        }
        private void InitLog()
        {
            try
            {
                string logDir = System.IO.Path.Combine(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "Logs");
                Logger.Init(logDir);
            }
            catch (Exception) { }
        }
    }
}