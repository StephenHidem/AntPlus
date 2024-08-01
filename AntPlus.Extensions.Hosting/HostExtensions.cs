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
    /// This class provides static methods to configure the application host builder. MAUI apps and standard Windows apps are supported. 
    /// </summary>
    public static partial class HostExtensions
    {
        /// <summary>
        /// Gets the service collection. This allows the <see cref="AntCollection"/> to enumerate the service collection to find
        /// the desired implementation type.
        /// </summary>
        internal static IServiceCollection? ServiceCollection;

        /// <summary>
        /// Adds AntPlus classes to host builder service collection.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <returns></returns>
        public static IHostBuilder UseAntPlus(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, collection) =>
            {
                AddServices(collection);
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
            AddServices(builder.Services);
            return builder;
        }

        private static void AddServices(IServiceCollection collection)
        {
            ServiceCollection = collection;

            // add devices with a single implementation
            collection.AddKeyedTransient<AntDevice, HeartRate>(HeartRate.DeviceClass);
            collection.AddKeyedTransient<AntDevice, BikeCadenceSensor>(BikeCadenceSensor.DeviceClass);
            collection.AddKeyedTransient<AntDevice, BikeSpeedSensor>(BikeSpeedSensor.DeviceClass);
            collection.AddKeyedTransient<AntDevice, CombinedSpeedAndCadenceSensor>(CombinedSpeedAndCadenceSensor.DeviceClass);
            collection.AddKeyedTransient<AntDevice, MuscleOxygen>(MuscleOxygen.DeviceClass);
            collection.AddKeyedTransient<AntDevice, Geocache>(Geocache.DeviceClass);
            collection.AddKeyedTransient<AntDevice, Tracker>(Tracker.DeviceClass);
            collection.AddKeyedTransient<AntDevice, StrideBasedSpeedAndDistance>(StrideBasedSpeedAndDistance.DeviceClass);
            collection.AddTransient<AntDevice, UnknownDevice>();

            // bicycle power and implementation selector
            collection.AddKeyedTransient<AntDevice, CrankTorqueFrequencySensor>(BicyclePower.DeviceClass);
            collection.AddKeyedTransient<AntDevice, StandardPowerSensor>(BicyclePower.DeviceClass);
            collection.AddKeyedSingleton<SelectImplementation, SelectBicyclePowerImplementation>(BicyclePower.DeviceClass);

            // fitness equipment and implementation selector
            collection.AddKeyedTransient<AntDevice, Climber>(FitnessEquipment.DeviceClass);
            collection.AddKeyedTransient<AntDevice, Elliptical>(FitnessEquipment.DeviceClass);
            collection.AddKeyedTransient<AntDevice, NordicSkier>(FitnessEquipment.DeviceClass);
            collection.AddKeyedTransient<AntDevice, Rower>(FitnessEquipment.DeviceClass);
            collection.AddKeyedTransient<AntDevice, TrainerStationaryBike>(FitnessEquipment.DeviceClass);
            collection.AddKeyedTransient<AntDevice, Treadmill>(FitnessEquipment.DeviceClass);
            collection.AddKeyedSingleton<SelectImplementation, SelectFitnessEquipmentImplementation>(FitnessEquipment.DeviceClass);

            // options and the ANT collection
            collection.AddOptions<TimeoutOptions>().BindConfiguration(nameof(TimeoutOptions));
            collection.AddSingleton<AntCollection>();
        }
    }
}
