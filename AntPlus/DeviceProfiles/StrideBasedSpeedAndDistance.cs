using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;

namespace SmallEarthTech.AntPlus.DeviceProfiles
{
    /// <summary>
    /// The stride based speed and distance monitor (SDM) class. The SDM is a personal body-worn device that allows the
    /// wearer to measure the number of strides taken, the speed at which they are traveling and/or the
    /// distance they have covered based on stride measurements and calculations.Some examples of
    /// SDMs include the foot-worn pods that go on, or in, a shoe and reconstruct strides to compute speed
    /// and distance while walking or running.Similarly, pedometers that may be worn on the waist or
    ///elsewhere are also considered SDMs.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public class StrideBasedSpeedAndDistance : AntDevice
    {
        /// <summary>
        /// The SDM device class ID.
        /// </summary>
        public const byte DeviceClass = 124;
        private bool isFirstDefaultPage = true;
        private bool isFirstCalPage = true;
        private byte prevCals;
        private byte prevTime;
        private byte prevDistance;
        private byte prevStrideCount;

        /// <summary>
        /// SDM device data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Default data page.</summary>
            Default = 1,
            /// <summary>This is a template page for other supplementary pages.</summary>
            BasePage,
            /// <summary>Calories</summary>
            Calories,
            /// <summary>The distance and strides summary</summary>
            DistanceAndStridesSummary = 16,
            /// <summary>The capabilities page.
            /// Page 22 is used to indicate the specific broadcast data capabilities of a sensor.</summary>
            Capabilities = 22
        }

        /// <summary>SDM location on the body.</summary>
        public enum SDMLocation
        {
            /// <summary>Laces</summary>
            Laces,
            /// <summary>Midsole</summary>
            Midsole,
            /// <summary>Other or unknown</summary>
            Other,
            /// <summary>Ankle</summary>
            Ankle
        }

        /// <summary>SDM battery status.</summary>
        public enum BatteryStatus
        {
            /// <summary>New</summary>
            New,
            /// <summary>Good</summary>
            Good,
            /// <summary>Ok</summary>
            Ok,
            /// <summary>Low</summary>
            Low
        }

        /// <summary>SDM health status.</summary>
        public enum HealthStatus
        {
            /// <summary>Ok</summary>
            Ok,
            /// <summary>Error</summary>
            Error,
            /// <summary>Warning</summary>
            Warning,
            /// <summary>Reserved</summary>
            Reserved
        }

        /// <summary>SDM active state.</summary>
        public enum UseState
        {
            /// <summary>Inactive</summary>
            Inactive,
            /// <summary>Active</summary>
            Active,
            /// <summary>Reserved</summary>
            Reserved
        }

        /// <summary>Sensor broadcast capabilities.</summary>
        [Flags]
        public enum CapabilitiesFlags
        {
            /// <summary>Unknown</summary>
            Unknown = 0x00,
            /// <summary>Calories valid</summary>
            CaloriesValid = 0x20,
            /// <summary>Cadence valid</summary>
            CadenceValid = 0x10,
            /// <summary>Update latency valid</summary>
            LatencyValid = 0x08,
            /// <summary>Speed valid</summary>
            SpeedValid = 0x04,
            /// <summary>Distance valid</summary>
            DistanceValid = 0x02,
            /// <summary>Time valid</summary>
            TimeValid = 0x01,
        }

        /// <summary>The SDM status flags.</summary>
        public readonly struct StatusFlags
        {
            private readonly byte status;

            /// <summary>Gets the SDM location on the body.</summary>
            /// <value>The location.</value>
            public SDMLocation Location => (SDMLocation)(status >> 6);
            /// <summary>Gets the battery status.</summary>
            /// <value>The battery.</value>
            public BatteryStatus Battery => (BatteryStatus)((status & 0x30) >> 4);
            /// <summary>Gets the SDM health status.</summary>
            /// <value>The health status.</value>
            public HealthStatus Health => (HealthStatus)((status & 0x0C) >> 2);
            /// <summary>Gets the use state of the SDM sensor.</summary>
            /// <value>The use state. Either active or inactive.</value>
            public UseState State => (status & 0x03) > 1 ? UseState.Reserved : (UseState)(status & 0x03);

            internal StatusFlags(byte status)
            {
                this.status = status;
            }
        }

        /// <summary>Gets the accumulated time duration since SDM sensor was turned on until it is turned off.</summary>
        /// <value>The accumulated time.</value>
        public double AccumulatedTime { get; private set; }
        /// <summary>Gets the cadence in strides per minute.</summary>
        /// <value>The stride cadence.</value>
        public double InstantaneousCadence { get; private set; }
        /// <summary>Gets the accumulated stride count.</summary>
        /// <value>The accumulated strides.</value>
        public int AccumulatedStrideCount { get; private set; }
        /// <summary>Gets the accumulated distance in meters.</summary>
        /// <value>The accumulated distance.</value>
        public double AccumulatedDistance { get; private set; }
        /// <summary>Gets the instantaneous speed in meters per second.</summary>
        /// <value>The instantaneous speed.</value>
        public double InstantaneousSpeed { get; private set; }
        /// <summary>Gets the update latency.
        /// The time elapsed between the last speed and distance computation and the transmission of this message.</summary>
        /// <value>The update latency in seconds.</value>
        public double UpdateLatency { get; private set; }
        /// <summary>Gets the status of the SDM sensor.</summary>
        /// <value>The status.</value>
        public StatusFlags Status { get; private set; }
        /// <summary>Gets the accumulated calories in kcals.</summary>
        /// <value>The accumulated calories.</value>
        public int AccumulatedCalories { get; private set; }
        /// <summary>Gets the sensor broadcast capabilities.</summary>
        /// <value>The capabilities.</value>
        public CapabilitiesFlags Capabilities { get; private set; } = CapabilitiesFlags.Unknown;
        /// <summary>Gets the accumulated strides since battery change.</summary>
        /// <value>The stride count summary.</value>
        public uint StrideCountSummary { get; private set; }
        /// <summary>Gets the accumulated distance since battery change.</summary>
        /// <value>The distance summary in meters.</value>
        public double DistanceSummary { get; private set; }

        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(StrideBasedSpeedAndDistance).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.SDM.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="StrideBasedSpeedAndDistance"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public StrideBasedSpeedAndDistance(ChannelId channelId, IAntChannel antChannel, int timeout = 2000) : base(channelId, antChannel, timeout)
        {
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            switch ((DataPage)dataPage[0])
            {
                case DataPage.Default:
                    if (isFirstDefaultPage)
                    {
                        isFirstDefaultPage = false;
                        prevTime = dataPage[2];
                        prevDistance = dataPage[3];
                        prevStrideCount = dataPage[6];
                    }
                    else
                    {
                        AccumulatedTime += Utils.CalculateDelta(dataPage[2], ref prevTime) + (dataPage[1] / 200.0);
                        AccumulatedDistance += Utils.CalculateDelta(dataPage[3], ref prevDistance) + (dataPage[4] >> 4) / 16.0;
                        AccumulatedStrideCount += Utils.CalculateDelta(dataPage[6], ref prevStrideCount);
                        RaisePropertyChange(nameof(AccumulatedTime));
                        RaisePropertyChange(nameof(AccumulatedDistance));
                        RaisePropertyChange(nameof(AccumulatedStrideCount));
                    }
                    InstantaneousSpeed = (dataPage[4] & 0x0F) + (dataPage[5] / 256.0);
                    UpdateLatency = dataPage[7] / 32.0;
                    RaisePropertyChange(nameof(InstantaneousSpeed));
                    RaisePropertyChange(nameof(UpdateLatency));
                    break;
                case DataPage.BasePage:
                    InstantaneousCadence = dataPage[3] + (dataPage[4] >> 4) / 16.0;
                    InstantaneousSpeed = (dataPage[4] & 0x0F) + (dataPage[5] / 256.0);
                    Status = new StatusFlags(dataPage[7]);
                    RaisePropertyChange(nameof(InstantaneousCadence));
                    RaisePropertyChange(nameof(InstantaneousSpeed));
                    RaisePropertyChange(nameof(Status));
                    break;
                case DataPage.Calories:
                    if (isFirstCalPage)
                    {
                        isFirstCalPage = false;
                        prevCals = dataPage[6];
                    }
                    else
                    {
                        AccumulatedCalories += Utils.CalculateDelta(dataPage[6], ref prevCals);
                        RaisePropertyChange(nameof(AccumulatedCalories));
                    }
                    InstantaneousCadence = dataPage[3] + (dataPage[4] >> 4) / 16.0;
                    InstantaneousSpeed = (dataPage[4] & 0x0F) + (dataPage[5] / 256.0);
                    Status = new StatusFlags(dataPage[7]);
                    RaisePropertyChange(nameof(InstantaneousCadence));
                    RaisePropertyChange(nameof(InstantaneousSpeed));
                    RaisePropertyChange(nameof(Status));
                    break;
                case DataPage.DistanceAndStridesSummary:
                    StrideCountSummary = BitConverter.ToUInt32(dataPage, 1) & 0x00FFFFFF;
                    DistanceSummary = BitConverter.ToUInt32(dataPage, 4) / 256.0;
                    RaisePropertyChange(nameof(StrideCountSummary));
                    RaisePropertyChange(nameof(DistanceSummary));
                    break;
                case DataPage.Capabilities:
                    Capabilities = (CapabilitiesFlags)dataPage[1];
                    RaisePropertyChange(nameof(Capabilities));
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        /// <summary>
        /// Requests the total cumulative distance and stride count since the last battery change.
        /// </summary>
        public void RequestSummaryPage()
        {
            RequestDataPage(DataPage.DistanceAndStridesSummary);
        }

        /// <summary>
        /// Requests the SDM broadcast capabilities page.
        /// </summary>
        /// <remarks>Page 22 is used to indicate the specific broadcast data capabilities of a sensor.
        /// The SDM shall be able to transmit the capabilities page upon request.
        /// Note that legacy SDMs were only required to support Page 22 if any
        /// of Calories, Cadence, Latency, Speed, Distance, or Time Stamp fields were not supported; therefore,
        /// displays shall gracefully handle not receiving a capabilities page when requested.If a
        /// capabilities page is not received, the display should assume that all the prior mentioned capabilities
        /// are supported and nothing else.
        /// </remarks>
        public void RequestBroadcastCapabilities()
        {
            RequestDataPage(DataPage.Capabilities);
        }
    }
}
