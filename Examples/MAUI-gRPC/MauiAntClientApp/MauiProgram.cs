using CommunityToolkit.Maui;
using MauiAntClientApp.Services;
using MauiAntClientApp.ViewModels;
using MauiAntClientApp.Views;
using MauiAntClientApp.Views.BicyclePowerPages;
using MauiAntClientApp.Views.FitnessEquipmentPages;
using MauiAntClientApp.Views.GeocachePages;
using MauiAntClientApp.Views.HeartRatePages;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntClientApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IAntRadio, AntRadioService>();
            mauiAppBuilder.Services.AddSingleton<AntDeviceCollection>();
            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<HomePageViewModel>();
            mauiAppBuilder.Services.AddTransient<RadioCapabilitiesViewModel>();
            mauiAppBuilder.Services.AddTransient<AssetTrackerViewModel>();
            mauiAppBuilder.Services.AddTransient<BicyclePowerViewModel>();
            mauiAppBuilder.Services.AddTransient<BikeCadenceViewModel>();
            mauiAppBuilder.Services.AddTransient<BikeSpeedViewModel>();
            mauiAppBuilder.Services.AddTransient<BikeSpeedAndCadenceViewModel>();
            mauiAppBuilder.Services.AddTransient<FitnessEquipmentViewModel>();
            mauiAppBuilder.Services.AddTransient<GeocacheViewModel>();
            mauiAppBuilder.Services.AddTransient<HeartRateViewModel>();
            mauiAppBuilder.Services.AddTransient<MuscleOxygenViewModel>();
            mauiAppBuilder.Services.AddTransient<SDMViewModel>();
            mauiAppBuilder.Services.AddTransient<UnknownDeviceViewModel>();
            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<HomePage>();
            mauiAppBuilder.Services.AddTransient<RadioCapabilitiesPage>();
            mauiAppBuilder.Services.AddTransient<AssetTrackerPage>();
            mauiAppBuilder.Services.AddTransient<BicyclePowerPage>();
            mauiAppBuilder.Services.AddTransient<BikeCadencePage>();
            mauiAppBuilder.Services.AddTransient<BikeSpeedAndCadencePage>();
            mauiAppBuilder.Services.AddTransient<BikeSpeedPage>();
            mauiAppBuilder.Services.AddTransient<FitnessEquipmentPage>();
            mauiAppBuilder.Services.AddTransient<GeocachePage>();
            mauiAppBuilder.Services.AddTransient<HeartRatePage>();
            mauiAppBuilder.Services.AddTransient<MuscleOxygenPage>();
            mauiAppBuilder.Services.AddTransient<StrideBasedMonitorPage>();
            mauiAppBuilder.Services.AddTransient<UnknownDevicePage>();
            return mauiAppBuilder;
        }
    }
}
