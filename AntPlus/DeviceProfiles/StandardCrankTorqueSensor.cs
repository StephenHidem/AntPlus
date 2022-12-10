namespace DeviceProfiles
{
    public class StandardCrankTorqueSensor : TorqueSensor
    {
        /// <summary>Gets the average cadence in rotations per minute.</summary>
        /// <value>The average cadence.</value>
        public double AverageCadence { get; private set; }

        public override void Parse(byte[] dataPage)
        {
            bool firstPage = isFirstDataMessage; // save first message flag for later use
            base.Parse(dataPage);
            if (!firstPage)
            {
                AverageCadence = 60.0 * deltaEventCount / (deltaPeriod / 2048.0);
            }
        }
    }
}
