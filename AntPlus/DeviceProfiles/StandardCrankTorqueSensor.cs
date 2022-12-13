using System;

namespace DeviceProfiles
{
    public class StandardCrankTorqueSensor : TorqueSensor
    {
        public event EventHandler<StandardCrankTorqueSensor> CrankTorquePageChanged;

        /// <summary>Gets the average cadence in rotations per minute.</summary>
        /// <value>The average cadence.</value>
        public double AverageCadence { get; private set; }

        public override void ParseTorque(byte[] dataPage)
        {
            bool firstPage = isFirstDataMessage; // save first message flag for later use
            base.ParseTorque(dataPage);
            if (!firstPage)
            {
                AverageCadence = 60.0 * deltaEventCount / (deltaPeriod / 2048.0);
            }
            CrankTorquePageChanged?.Invoke(this, null);
        }
    }
}
