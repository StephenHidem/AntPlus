using CommunityToolkit.Maui;
using MauiAntGrpcClient.Pages;
using MauiAntGrpcClient.Services;
using MauiAntGrpcClient.ViewModels;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntGrpcClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
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
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IAntRadio, AntRadioService>();
            mauiAppBuilder.Services.AddSingleton<CancellationTokenSource>();

            mauiAppBuilder.Services.AddTransientWithShellRoute<HomePage, HomePageViewModel>("Home");
            mauiAppBuilder.Services.AddTransientWithShellRoute<RadioCapabilitiesPage, RadioCapabilitiesViewModel>("RadioCapabilities");
            mauiAppBuilder.Services.AddTransientWithShellRoute<AssetTrackerPage, AssetTrackerViewModel>("AssetTracker");
            mauiAppBuilder.Services.AddTransientWithShellRoute<BicyclePowerPage, BicyclePowerViewModel>("BicyclePower");
            mauiAppBuilder.Services.AddTransientWithShellRoute<BikeCadencePage, BikeCadenceViewModel>("BikeCadence");
            mauiAppBuilder.Services.AddTransientWithShellRoute<BikeSpeedPage, BikeSpeedViewModel>("BikeSpeed");
            mauiAppBuilder.Services.AddTransientWithShellRoute<BikeSpeedAndCadencePage, BikeSpeedAndCadenceViewModel>("SpeedAndCadence");
            mauiAppBuilder.Services.AddTransientWithShellRoute<CTFPage, CTFViewModel>("CrankTorqueFrequency");
            mauiAppBuilder.Services.AddTransientWithShellRoute<FitnessEquipmentPage, FitnessEquipmentViewModel>("FitnessEquipment");
            mauiAppBuilder.Services.AddTransientWithShellRoute<GeocachePage, GeocacheViewModel>("Geocache");
            mauiAppBuilder.Services.AddTransientWithShellRoute<HeartRatePage, HeartRateViewModel>("HeartRate");
            mauiAppBuilder.Services.AddTransientWithShellRoute<MuscleOxygenPage, MuscleOxygenViewModel>("MuscleOxygen");
            mauiAppBuilder.Services.AddTransientWithShellRoute<StrideBasedMonitorPage, SDMViewModel>("StrideBasedMonitor");
            mauiAppBuilder.Services.AddTransientWithShellRoute<UnknownDevicePage, UnknownDeviceViewModel>("UnknownDevice");

            return mauiAppBuilder;
        }
    }
}
