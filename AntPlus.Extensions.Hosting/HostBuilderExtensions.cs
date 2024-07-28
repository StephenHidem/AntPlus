using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace SmallEarthTech.AntPlus.Extensions.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class HostBuilderExtensions
    {
        /// <summary>
        /// Adds AntPlus classes to host builder service collection.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <returns></returns>
        public static IHostBuilder UseAntPlus(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, collection) =>
            {
                collection.AddTransient<AntDevice, HeartRate>();
                collection.AddTransient<AntDevice, BikeCadenceSensor>();
                collection.AddTransient<AntDevice, BikeSpeedSensor>();
                collection.AddTransient<AntDevice, CombinedSpeedAndCadenceSensor>();
                collection.AddTransient<AntDevice, MuscleOxygen>();
                collection.AddTransient<AntDevice, Geocache>();
                collection.AddTransient<AntDevice, Tracker>();
                collection.AddTransient<AntDevice, StrideBasedSpeedAndDistance>();
                collection.AddTransient<AntDevice, UnknownDevice>();

                // bicycle power
                collection.AddTransient<AntDevice, CrankTorqueFrequencySensor>();
                collection.AddTransient<AntDevice, StandardPowerSensor>();

                // fitness equipment
                collection.AddTransient<AntDevice, Climber>();
                collection.AddTransient<AntDevice, Elliptical>();
                collection.AddTransient<AntDevice, NordicSkier>();
                collection.AddTransient<AntDevice, Rower>();
                collection.AddTransient<AntDevice, TrainerStationaryBike>();
                collection.AddTransient<AntDevice, Treadmill>();

                collection.AddOptions<TimeoutOptions>().BindConfiguration(nameof(TimeoutOptions));
                collection.AddSingleton<AntCollection>();

            });
            return builder;
        }

        /// <summary>
        /// Adds AntPlus classes to MAUI builder service collection.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static MauiAppBuilder UseMauiAntPlus(this MauiAppBuilder builder)
        {
            IServiceCollection collection = builder.Services;
            collection.AddTransient<HeartRate>();
            collection.AddTransient<BikeCadenceSensor>();
            collection.AddTransient<BikeSpeedSensor>();
            collection.AddTransient<CombinedSpeedAndCadenceSensor>();
            collection.AddTransient<MuscleOxygen>();
            collection.AddTransient<Geocache>();
            collection.AddTransient<Tracker>();
            collection.AddTransient<StrideBasedSpeedAndDistance>();
            collection.AddTransient<UnknownDevice>();

            // bicycle power
            collection.AddTransient<CrankTorqueFrequencySensor>();
            collection.AddTransient<StandardPowerSensor>();

            // fitness equipment
            collection.AddTransient<Climber>();
            collection.AddTransient<Elliptical>();
            collection.AddTransient<NordicSkier>();
            collection.AddTransient<Rower>();
            collection.AddTransient<TrainerStationaryBike>();
            collection.AddTransient<Treadmill>();

            collection.AddOptions<TimeoutOptions>().BindConfiguration(nameof(TimeoutOptions));
            collection.AddSingleton<AntCollection>();

            return builder;
        }
    }
}
