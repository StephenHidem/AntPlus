
using System.Diagnostics;

namespace MauiAntGrpcClient
{
    public partial class App : Application
    {
        // provides an application-wide cancellation token source
        public readonly static CancellationTokenSource CancellationTokenSource = new();

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        // This is the simplest way to handle cross-platform application lifecycle event handling
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Destroying += (s, e) =>
            {
                Debug.WriteLine("App: destroying window.");
                CancellationTokenSource.Cancel();
            };
            return window;
        }
    }
}
