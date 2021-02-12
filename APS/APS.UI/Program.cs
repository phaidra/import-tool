using System;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace APS.UI
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static int Main(string[] args)
        {
            //BuildAvaloniaApp().Start(AppMain, args); 
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            return 0; 
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        //private static void AppMain(Application app, string[] args) // removed due to breaking change in Avalonia update 0.8 -> 0.9
        //{
        //    app.Run(new MainWindow());
        //}
    }
}
