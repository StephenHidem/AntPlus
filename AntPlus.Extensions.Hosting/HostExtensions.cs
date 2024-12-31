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
    public static class HostExtensions
    {
        /// <summary>
        /// Gets the service services. This allows the <see cref="AntCollection"/> to enumerate the service services to find
        /// the desired implementation type.
        /// </summary>
        internal static IServiceCollection? ServiceCollection;

        /// <summary>
        /// Adds AntPlus classes to host builder service services.
        /// </summary>
        /// <param name="builder">The host app builder</param>
        /// <returns>The host app builder</returns>
        public static IHostBuilder UseAntPlus(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, collection) =>
            {
                AddAntPlusServices(collection);
            });
            return builder;
        }

        /// <summary>
        /// Adds AntPlus classes to MAUI builder service services.
        /// </summary>
        /// <param name="builder">The MAUI host app builder</param>
        /// <returns>The MAUI host app builder</returns>
        public static MauiAppBuilder UseAntPlus(this MauiAppBuilder builder)
        {
            AddAntPlusServices(builder.Services);
            return builder;
        }

        /// <summary>
        /// Adds AntPlus services to the specified service collection.
        /// </summary>
        /// <remarks>
        /// Use this method to add AntPlus services to the service collection when UseAntPlus() is not
        /// suitable, for example, when using a custom host builder.
        /// <code language="cs">
        ///     builder.ConfigureServices((context, services) => services.AddAntPlusServices());
        /// </code>
        /// </remarks>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection with added services.</returns>
        public static IServiceCollection AddAntPlusServices(this IServiceCollection services)
        {
            ServiceCollection = services;

            // add devices with a single implementation
            services.AddKeyedTransient<AntDevice, HeartRate>(HeartRate.DeviceClass);
            services.AddKeyedTransient<AntDevice, BikeCadenceSensor>(BikeCadenceSensor.DeviceClass);
            services.AddKeyedTransient<AntDevice, BikeSpeedSensor>(BikeSpeedSensor.DeviceClass);
            services.AddKeyedTransient<AntDevice, CombinedSpeedAndCadenceSensor>(CombinedSpeedAndCadenceSensor.DeviceClass);
            services.AddKeyedTransient<AntDevice, MuscleOxygen>(MuscleOxygen.DeviceClass);
            services.AddKeyedTransient<AntDevice, Geocache>(Geocache.DeviceClass);
            services.AddKeyedTransient<AntDevice, Tracker>(Tracker.DeviceClass);
            services.AddKeyedTransient<AntDevice, StrideBasedSpeedAndDistance>(StrideBasedSpeedAndDistance.DeviceClass);
            services.AddTransient<AntDevice, UnknownDevice>();

            // bicycle power and implementation selector
            services.AddKeyedTransient<AntDevice, CrankTorqueFrequencySensor>(BicyclePower.DeviceClass);
            services.AddKeyedTransient<AntDevice, StandardPowerSensor>(BicyclePower.DeviceClass);
            services.AddKeyedSingleton<ISelectImplementation, SelectBicyclePowerImplementation>(BicyclePower.DeviceClass);

            // fitness equipment and implementation selector
            services.AddKeyedTransient<AntDevice, Climber>(FitnessEquipment.DeviceClass);
            services.AddKeyedTransient<AntDevice, Elliptical>(FitnessEquipment.DeviceClass);
            services.AddKeyedTransient<AntDevice, NordicSkier>(FitnessEquipment.DeviceClass);
            services.AddKeyedTransient<AntDevice, Rower>(FitnessEquipment.DeviceClass);
            services.AddKeyedTransient<AntDevice, TrainerStationaryBike>(FitnessEquipment.DeviceClass);
            services.AddKeyedTransient<AntDevice, Treadmill>(FitnessEquipment.DeviceClass);
            services.AddKeyedSingleton<ISelectImplementation, SelectFitnessEquipmentImplementation>(FitnessEquipment.DeviceClass);

            // options and the ANT services
            services.AddOptions<TimeoutOptions>().BindConfiguration(nameof(TimeoutOptions));
            services.AddSingleton<AntCollection>();
            return services;
        }
    }
}
