using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The crank torque frequency sensor class.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class CrankTorqueFrequencySensor : INotifyPropertyChanged
    {
        /// <summary>Crank torque frequency defined IDs. These are used by methods that save the slope or serial number to the sensor flash or when the zero offset is reported by the sensor.</summary>
        public enum CTFDefinedId
        {
            /// <summary>The zero offset reported by sensor.</summary>
            ZeroOffset = 1,
            /// <summary>The slope ID.</summary>
            Slope = 2,
            /// <summary>The serial number ID.</summary>
            SerialNumber = 3,
            /// <summary>The acknowledgement message ID.</summary>
            Ack = 0xAC
        }

        private readonly Bicycle _bicycle;
        private readonly ILogger _logger;
        private bool isFirstPage = true;
        private byte prevUpdateEventCount;
        private ushort prevTimeStamp;
        private ushort prevTorqueTicks;

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Occurs when slope or serial number save acknowledged.</summary>
        public event EventHandler<CTFDefinedId> SaveAcknowledged;

        /// <summary>Gets the zero offset in Hz.</summary>
        /// <remarks>Offset is only received from the sensor when a calibration is requested or bike has been coasting.</remarks>
        public ushort Offset { get; private set; } = 0;
        /// <summary>Gets the slope. Slope ranges in value from 10.0Nm/Hz to 50.0Nm/Hz. Resolution is 0.1 Nm/Hz.</summary>
        public double Slope { get; private set; }
        /// <summary>Gets the cadence in revolutions per minute.</summary>
        public double Cadence { get; private set; }
        /// <summary>Gets the torque in Nm.</summary>
        public double Torque { get; private set; }
        /// <summary>Gets the power in watts.</summary>
        public double Power { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="CrankTorqueFrequencySensor" /> class.</summary>
        /// <param name="bicycle">The <see cref="Bicycle"/> instance.</param>
        /// <param name="logger">The logger to use.</param>
        public CrankTorqueFrequencySensor(Bicycle bicycle, ILogger logger)
        {
            _bicycle = bicycle;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Bike Power (CTF)";
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            byte updateEventCount = dataPage[1];

            // the data is in big endian order; this also flips the order of the fields
            byte[] data = dataPage.Skip(2).Reverse().ToArray();
            ushort torqueTicks = BitConverter.ToUInt16(data, 0);
            ushort timeStamp = BitConverter.ToUInt16(data, 2);
            Slope = BitConverter.ToUInt16(data, 4) / 10.0;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Slope)));

            if (isFirstPage)
            {
                isFirstPage = false;
                prevUpdateEventCount = updateEventCount;
                prevTimeStamp = timeStamp;
                prevTorqueTicks = torqueTicks;
                return;
            }

            if (prevUpdateEventCount != updateEventCount && prevTorqueTicks != torqueTicks)
            {
                // calculations
                double elapsedTime = Utils.CalculateDelta(timeStamp, ref prevTimeStamp) * 0.0005;
                double cadencePeriod = elapsedTime / Utils.CalculateDelta(updateEventCount, ref prevUpdateEventCount);
                Cadence = 60.0 / cadencePeriod;
                double torqueFreq = (1.0 / (elapsedTime / Utils.CalculateDelta(torqueTicks, ref prevTorqueTicks))) - Offset;
                Torque = torqueFreq / Slope;
                Power = Torque * Cadence * Math.PI / 30.0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cadence)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Torque)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Power)));
            }
        }

        /// <summary>Parses the calibration message.</summary>
        /// <param name="message">The message.</param>
        internal void ParseCalibrationMessage(byte[] message)
        {
            _logger.LogDebug("ParseCalibrationMessage: {Msg}", (CTFDefinedId)message[2]);
            switch ((CTFDefinedId)message[2])
            {
                case CTFDefinedId.ZeroOffset:
                    Offset = BitConverter.ToUInt16(message.Skip(6).Reverse().ToArray(), 0);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offset)));
                    break;
                case CTFDefinedId.Ack:
                    // TODO: NEED TO REPORT STATUS OF CTF SAVE FUNCS
                    SaveAcknowledged?.Invoke(this, (CTFDefinedId)message[3]);
                    switch ((CTFDefinedId)message[3])
                    {
                        case CTFDefinedId.Slope:
                            break;
                        case CTFDefinedId.SerialNumber:
                            break;
                        default:
                            _logger.LogWarning("Unexpected CTF acknowledged ID = {ID}", message[3]);
                            break;
                    }
                    break;
                default:
                    _logger.LogWarning("Unexpected CTF message ID = {ID}", message[2]);
                    break;
            }
        }

        /// <summary>Saves the slope to flash.</summary>
        /// <param name="slope">The slope. Valid range is 10.0 to 50.0 Nm/Hz. Resolution is 0.1 Nm/Hz.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid slope value.</exception>
        public async Task<MessagingReturnCode> SaveSlopeToFlash(double slope)
        {
            // check range
            if (slope < 10.0 || slope > 50.0)
            {
                throw new ArgumentOutOfRangeException(nameof(slope));
            }

            ushort slp = Convert.ToUInt16(slope * 10.0);    // scale by resolution

            byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.Slope, 0xFF, 0xFF, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(slp).Reverse()).ToArray();
            return await _bicycle.SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Saves the serial number to flash.</summary>
        /// <param name="serialNumber">The serial number.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SaveSerialNumberToFlash(ushort serialNumber)
        {
            byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.SerialNumber, 0xFF, 0xFF, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(serialNumber).Reverse()).ToArray();
            return await _bicycle.SendExtAcknowledgedMessage(msg);
        }
    }
}
