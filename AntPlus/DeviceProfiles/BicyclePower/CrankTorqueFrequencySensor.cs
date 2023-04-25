using System;
using System.ComponentModel;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The crank torque frequency sensor class.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class CrankTorqueFrequencySensor : INotifyPropertyChanged
    {
        private enum CTFDefinedId
        {
            ZeroOffset = 1,
            Slope = 2,
            SerialNumber = 3,
            Ack = 0xAC
        }

        private readonly BicyclePower bp;

        private bool isFirstPage = true;
        private byte prevUpdateEventCount;
        private ushort prevTimeStamp;
        private ushort prevTorqueTicks;

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the zero offset in Hz.</summary>
        public ushort Offset { get; private set; } = 0;
        /// <summary>Gets the slope. Slope ranges in value from 10.0Nm/Hz to 50.0Nm/Hz.</summary>
        public double Slope { get; private set; }
        /// <summary>Gets the cadence in revolutions per minute.</summary>
        public double Cadence { get; private set; }
        /// <summary>Gets the torque in Nm.</summary>
        public double Torque { get; private set; }
        /// <summary>Gets the power in watts.</summary>
        public double Power { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="CrankTorqueFrequencySensor" /> class.</summary>
        /// <param name="bp">The bp.</param>
        public CrankTorqueFrequencySensor(BicyclePower bp)
        {
            this.bp = bp;
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
                Torque = torqueFreq / (Slope / 10.0);
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
            switch ((CTFDefinedId)message[2])
            {
                case CTFDefinedId.ZeroOffset:
                    Offset = BitConverter.ToUInt16(message.Skip(6).Reverse().ToArray(), 0);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offset)));
                    break;
                case CTFDefinedId.Ack:
                    // TODO: NEED TO REPORT STATUS OF CTF SAVE FUNCS
                    switch ((CTFDefinedId)message[3])
                    {
                        case CTFDefinedId.Slope:
                            break;
                        case CTFDefinedId.SerialNumber:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>Requests manual calibration.</summary>
        public void RequestCalibration()
        {
            // TODO: FIX REQUEST
            //bp.Calibration.RequestManualCalibration();
        }

        /// <summary>Saves the slope to flash.</summary>
        /// <param name="slope">The slope.</param>
        public void SaveSlopeToFlash(ushort slope)
        {
            byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.Slope, 0xFF, 0xFF, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(slope).Reverse()).ToArray();
            bp.SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Saves the serial number to flash.</summary>
        /// <param name="serialNumber">The serial number.</param>
        public void SaveSerialNumberToFlash(ushort serialNumber)
        {
            byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.SerialNumber, 0xFF, 0xFF, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(serialNumber).Reverse()).ToArray();
            bp.SendExtAcknowledgedMessage(msg);
        }
    }
}
