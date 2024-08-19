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
            return (FitnessEquipment.DataPage)page[0] switch
            {
                FitnessEquipment.DataPage.GeneralFEData => (FitnessEquipment.FitnessEquipmentType)page[1] switch
                {
                    FitnessEquipment.FitnessEquipmentType.Treadmill => typeof(Treadmill),
                    FitnessEquipment.FitnessEquipmentType.Elliptical => typeof(Elliptical),
                    FitnessEquipment.FitnessEquipmentType.Rower => typeof(Rower),
                    FitnessEquipment.FitnessEquipmentType.Climber => typeof(Climber),
                    FitnessEquipment.FitnessEquipmentType.NordicSkier => typeof(NordicSkier),
                    FitnessEquipment.FitnessEquipmentType.TrainerStationaryBike => typeof(TrainerStationaryBike),
                    _ => null,
                },
                FitnessEquipment.DataPage.TreadmillData => typeof(Treadmill),
                FitnessEquipment.DataPage.EllipticalData => typeof(Elliptical),
                FitnessEquipment.DataPage.RowerData => typeof(Rower),
                FitnessEquipment.DataPage.ClimberData => typeof(Climber),
                FitnessEquipment.DataPage.NordicSkierData => typeof(NordicSkier),
                FitnessEquipment.DataPage.TrainerStationaryBikeData => typeof(TrainerStationaryBike),
                FitnessEquipment.DataPage.TrainerTorqueData => typeof(TrainerStationaryBike),
                _ => null,
            };
        }
    }
}
