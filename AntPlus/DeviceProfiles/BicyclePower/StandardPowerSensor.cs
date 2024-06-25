using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard power sensor class. Note that torque sensors report this data page for
    /// displays that may not handle torque sensor messages.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class StandardPowerSensor : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastEventCount;
        private int deltaEventCount;
        private ushort lastPower;
        private int deltaPower;

        /// <summary>Pedal power differentiation.</summary>
        public enum PedalDifferentiation
        {
            /// <summary>Right pedal power contribution.</summary>
            RightPedal,
            /// <summary>Unknown pedal power contribution.</summary>
            Unknown,
            /// <summary>Pedal power not used.</summary>
            Unused
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>Raises the property change.</summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Gets the average power in watts.</summary>
        public double AveragePower { get; private set; }
        /// <summary>The pedal power data field provides the user’s power contribution (as a percentage) between the left and right pedals, as
        /// measured by a pedal power sensor. </summary>
        /// <value>The pedal power.</value>
        public byte PedalPower { get; private set; }
        /// <summary>Gets the pedal power contribution.</summary>
        /// <value>The pedal differentiation.</value>
        public PedalDifferentiation PedalContribution { get; private set; }
        /// <summary>Gets the instantaneous pedaling cadence.</summary>
        public byte InstantaneousCadence { get; protected set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        public ushort InstantaneousPower { get; private set; }

        /// <summary>Gets the parameters.</summary>
        public Parameters Parameters { get; private set; }
        /// <summary>Gets the torque effectiveness and pedal smoothness.</summary>
        public TorqueEffectivenessAndPedalSmoothness TorqueEffectiveness { get; private set; } = new TorqueEffectivenessAndPedalSmoothness();
        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardPowerSensor"/> class.
        /// </summary>
        /// <param name="bicycle">The <see cref="Bicycle"/>.</param>
        /// <param name="logger">Logger to use.</param>
        public StandardPowerSensor(Bicycle bicycle, ILogger logger)
        {
            CommonDataPages = new CommonDataPages(logger);
            Parameters = new Parameters(bicycle, logger);
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            PedalPower = (byte)(dataPage[2] & 0x7F);
            PedalContribution = dataPage[2] == 0xFF
                ? PedalDifferentiation.Unused
                : (dataPage[2] & 0x80) != 0 ? PedalDifferentiation.RightPedal : PedalDifferentiation.Unknown;
            InstantaneousCadence = dataPage[3];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 6);
            RaisePropertyChange(nameof(PedalPower));
            RaisePropertyChange(nameof(PedalContribution));
            RaisePropertyChange(nameof(InstantaneousCadence));
            RaisePropertyChange(nameof(InstantaneousPower));

            if (isFirstDataMessage)
            {
                // initialize if first data message
                isFirstDataMessage = false;
                lastEventCount = dataPage[1];
                lastPower = BitConverter.ToUInt16(dataPage, 4);
                return;
            }

            if (dataPage[1] != lastEventCount)
            {
                // handle new events
                deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                deltaPower = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPower);
                AveragePower = deltaPower / deltaEventCount;
                RaisePropertyChange(nameof(AveragePower));
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Bike Power (Power Only)";
        }
    }
}
