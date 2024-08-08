using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System;

namespace SmallEarthTech.AntPlus.Extensions.Hosting
{
    /// <summary>
    /// This interface defines a single method to get the implementation type of an ANT device.
    /// </summary>
    public interface ISelectImplementation
    {
        /// <summary>
        /// Selects the type of the implementation.
        /// </summary>
        /// <param name="page">The data page used to pick an implementation.</param>
        /// <returns>null if an implementation could not be found.</returns>
        public Type? SelectImplementationType(byte[] page);
    }

    #region SelectBicyclePowerImplementation
    /// <summary>
    /// Selects the bicycle power implementation type.
    /// </summary>
    internal class SelectBicyclePowerImplementation : ISelectImplementation
    {
        private readonly ILogger<SelectBicyclePowerImplementation> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectBicyclePowerImplementation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SelectBicyclePowerImplementation(ILogger<SelectBicyclePowerImplementation> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the type of the bicycle power sensor.
        /// </summary>
        /// <remarks>
        /// <see cref="CrankTorqueFrequencySensor"/>s only broadcast their main page. Other bicycle power sensors broadcast
        /// any number of other pages. This allows the method to determine the sensor type.
        /// </remarks>
        /// <inheritdoc />
        public Type? SelectImplementationType(byte[] page)
        {
            _logger.LogDebug("Selecting implementation type. DataPage = {DataPage}", BitConverter.ToString(page));
            if ((BicyclePower.DataPage)page[0] == BicyclePower.DataPage.CrankTorqueFrequency)
            {
                // CTF sensor
                return typeof(CrankTorqueFrequencySensor);
            }
            else
            {
                return typeof(StandardPowerSensor);
            }
        }
    }
    #endregion

    /// <summary>
    /// Selects the fitness equipment implementation type.
    /// </summary>
    internal class SelectFitnessEquipmentImplementation : ISelectImplementation
    {
        private readonly ILogger<SelectFitnessEquipmentImplementation> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFitnessEquipmentImplementation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SelectFitnessEquipmentImplementation(ILogger<SelectFitnessEquipmentImplementation> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the type of the fitness equipment.
        /// </summary>
        /// <remarks>
        /// The fitness equipment type can be determined from the <see cref="FitnessEquipment.DataPage.GeneralFEData"/> page
        /// or pages specific to the equipment type. Any other page will return null.
        /// </remarks>
        /// <inheritdoc />
        public Type? SelectImplementationType(byte[] page)
        {
            _logger.LogDebug("Selecting implementation type. DataPage = {DataPage}", BitConverter.ToString(page));
            switch ((FitnessEquipment.DataPage)page[0])
            {
                case FitnessEquipment.DataPage.GeneralFEData:
                    switch ((FitnessEquipment.FitnessEquipmentType)page[1])
                    {
                        case FitnessEquipment.FitnessEquipmentType.Treadmill:
                            return typeof(Treadmill);
                        case FitnessEquipment.FitnessEquipmentType.Elliptical:
                            return typeof(Elliptical);
                        case FitnessEquipment.FitnessEquipmentType.Rower:
                            return typeof(Rower);
                        case FitnessEquipment.FitnessEquipmentType.Climber:
                            return typeof(Climber);
                        case FitnessEquipment.FitnessEquipmentType.NordicSkier:
                            return typeof(NordicSkier);
                        case FitnessEquipment.FitnessEquipmentType.TrainerStationaryBike:
                            return typeof(TrainerStationaryBike);
                        default:
                            _logger.LogError("Unknown equipment type = {EquipmentType}", page[1]);
                            return null;
                    }
                case FitnessEquipment.DataPage.TreadmillData:
                    return typeof(Treadmill);
                case FitnessEquipment.DataPage.EllipticalData:
                    return typeof(Elliptical);
                case FitnessEquipment.DataPage.RowerData:
                    return typeof(Rower);
                case FitnessEquipment.DataPage.ClimberData:
                    return typeof(Climber);
                case FitnessEquipment.DataPage.NordicSkierData:
                    return typeof(NordicSkier);
                case FitnessEquipment.DataPage.TrainerStationaryBikeData:
                    return typeof(TrainerStationaryBike);
                case FitnessEquipment.DataPage.TrainerTorqueData:
                    return typeof(TrainerStationaryBike);
                default:
                    _logger.LogWarning("Unknown equipment type.");
                    return null;
            }
        }
    }
}
