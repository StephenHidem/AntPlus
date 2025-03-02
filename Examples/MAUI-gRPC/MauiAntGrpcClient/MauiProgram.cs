using AntGrpcShared.ClientServices;
using CommunityToolkit.Maui;
using MauiAntGrpcClient.Pages;
using MauiAntGrpcClient.ViewModels;
using Serilog;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntGrpcClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Initialize Serilog early, without access to configuration or services
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug(outputTemplate:
                    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}") // + file or centralized logging
                .MinimumLevel.Debug()
                .CreateLogger();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // StartScanning the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseAntPlus()
                .RegisterAppServices();

#if DEBUG
            //builder.Logging.AddDebug();
            builder.Logging.AddSerilog(dispose: true);
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services
                .AddSingleton<IAntRadio, AntRadioService>()
                .AddSingleton<CancellationTokenSource>()

                // Register the pages and view models with shell routes
                .AddTransientWithShellRoute<HomePage, HomePageViewModel>("Home")
                .AddTransientWithShellRoute<RadioCapabilitiesPage, RadioCapabilitiesViewModel>("RadioCapabilities")
                .AddTransientWithShellRoute<AssetTrackerPage, AssetTrackerViewModel>("AssetTracker")
                .AddTransientWithShellRoute<BicyclePowerPage, BicyclePowerViewModel>("BicyclePower")
                .AddTransientWithShellRoute<BikeCadencePage, BikeCadenceViewModel>("BikeCadence")
                .AddTransientWithShellRoute<BikeSpeedPage, BikeSpeedViewModel>("BikeSpeed")
                .AddTransientWithShellRoute<BikeSpeedAndCadencePage, BikeSpeedAndCadenceViewModel>("SpeedAndCadence")
                .AddTransientWithShellRoute<CTFPage, CTFViewModel>("CrankTorqueFrequency")
                .AddTransientWithShellRoute<FitnessEquipmentPage, FitnessEquipmentViewModel>("FitnessEquipment")
                .AddTransientWithShellRoute<GeocachePage, GeocacheViewModel>("Geocache")
                .AddTransientWithShellRoute<HeartRatePage, HeartRateViewModel>("HeartRate")
                .AddTransientWithShellRoute<MuscleOxygenPage, MuscleOxygenViewModel>("MuscleOxygen")
                .AddTransientWithShellRoute<StrideBasedMonitorPage, SDMViewModel>("StrideBasedMonitor")
                .AddTransientWithShellRoute<UnknownDevicePage, UnknownDeviceViewModel>("UnknownDevice")

                // register the BikeRadar device, page, and view model
                .AddKeyedScoped<AntDevice, BikeRadar>(BikeRadar.DeviceClass)
                .AddTransientWithShellRoute<BikeRadarPage, BikeRadarViewModel>("BikeRadar");

            return mauiAppBuilder;
        }
    }
}
