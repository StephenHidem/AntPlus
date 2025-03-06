
using System.Diagnostics;

namespace MauiAntGrpcClient
{
    public partial class App : Application
    {
        private readonly CancellationTokenSource _cts;

        public App(CancellationTokenSource cancellationTokenSource)
        {
            _cts = cancellationTokenSource;
            InitializeComponent();
        }

        // This is the simplest way to handle cross-platform application lifecycle event handling
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = new(new AppShell());
            window.Destroying += (s, e) =>
            {
                Debug.WriteLine("App: destroying window.");
                _cts.Cancel();
            };
            return window;
        }
    }
}
