using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This is the primary support class for fitness equipment sensors.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public abstract partial class FitnessEquipment : AntDevice
    {
        /// <summary>
        /// The device type value transmitted in the channel ID.
        /// </summary>
        public const byte DeviceClass = 17;

        /// <inheritdoc/>
        public override int ChannelCount => 8192;

        /// <summary>
        /// Flag indicating the data page was handled by <see cref="Parse(byte[])"/>.
        /// </summary>
        protected bool handledPage;

        /// <summary>
        /// Fitness equipment data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Invalid page</summary>
            None = 0,
            /// <summary>Calibration request/response page</summary>
            CalRequestResponse = 0x01,
            /// <summary>Calibration progress page</summary>
            CalProgress = 0x02,
            /// <summary>General data page</summary>
            GeneralFEData = 0x10,
            /// <summary>General settings page</summary>
            GeneralSettings = 0x11,
            /// <summary>General metabolic data page</summary>
            GeneralMetabolicData = 0x12,
            /// <summary>Treadmill data page</summary>
            TreadmillData = 0x13,
            /// <summary>Elliptical data page</summary>
            EllipticalData = 0x14,
            /// <summary>Rower data page</summary>
            RowerData = 0x16,
            /// <summary>Climber data page</summary>
            ClimberData = 0x17,
            /// <summary>Nordic Skier data page</summary>
            NordicSkierData = 0x18,
            /// <summary>Trainer stationary bike data page</summary>
            TrainerStationaryBikeData = 0x19,
            /// <summary>Trainer torque data page</summary>
            TrainerTorqueData = 0x1A,
            /// <summary>Basic resistance page</summary>
            BasicResistance = 0x30,
            /// <summary>Target power page</summary>
            TargetPower = 0x31,
            /// <summary>Wind resistance page</summary>
            WindResistance = 0x32,
            /// <summary>Track resistance page</summary>
            TrackResistance = 0x33,
            /// <summary>Equipment capabilities page</summary>
            FECapabilities = 0x36,
            /// <summary>User configuration page</summary>
            UserConfiguration = 0x37
        }

        /// <summary>
        /// Fitness equipment type.
        /// </summary>
        public enum FitnessEquipmentType
        {
            /// <summary>Treadmill</summary>
            Treadmill = 0x13,
            /// <summary>Elliptical</summary>
            Elliptical = 0x14,
            /// <summary>Rower</summary>
            Rower = 0x16,
            /// <summary>Climber</summary>
            Climber = 0x17,
            /// <summary>Nordic skier</summary>
            NordicSkier = 0x18,
            /// <summary>Trainer stationary bike</summary>
            TrainerStationaryBike = 0x19
        }

        /// <summary>Heart rate data source</summary>
        public enum HRDataSource
        {
            /// <summary>Invalid</summary>
            Invalid,
            /// <summary>Heart rate monitor</summary>
            HeartRateMonitor,
            /// <summary>EM heart rate monitor</summary>
            EMHeartRateMonitor,
            /// <summary>Hand contact sensors</summary>
            HandContactSensors
        }

        /// <summary>Equipment state</summary>
        public enum FEState
        {
            /// <summary>Unknown</summary>
            Unknown = 0,
            /// <summary>Asleep or off</summary>
            AsleepOrOff = 1,
            /// <summary>Ready</summary>
            Ready,
            /// <summary>In use</summary>
            InUse,
            /// <summary>Finished or paused</summary>
            FinishedOrPaused
        }

        /// <summary>Supported training modes</summary>
        [Flags]
        public enum SupportedTrainingModes
        {
            /// <summary>No training modes supported</summary>
            None = 0x00,
            /// <summary>Basic resistance</summary>
            BasicResistance = 0x01,
            /// <summary>Target power</summary>
            TargetPower = 0x02,
            /// <summary>Simulation</summary>
            Simulation = 0x04
        }

        /// <summary>This class supports fitness equipment general data pages.</summary>
        public partial class GeneralDataPage : ObservableObject
        {
            private bool isFirstDataMessage = true;
            private byte prevElapsedTime;
            private byte prevDistance;

            /// <summary>Gets the type of the equipment.</summary>
            [ObservableProperty]
            private FitnessEquipmentType equipmentType;
            /// <summary>Gets the elapsed time.</summary>
            [ObservableProperty]
            private TimeSpan elapsedTime;
            /// <summary>Gets the distance traveled in meters. This property is only valid when
            /// <see cref="DistanceTraveledEnabled"/> is enabled.</summary>
            [ObservableProperty]
            private int distanceTraveled;
            /// <summary>Gets the instantaneous speed.</summary>
            /// <value>The instantaneous speed in meters per second.</value>
            [ObservableProperty]
            private double instantaneousSpeed;
            /// <summary>Gets the instantaneous heart rate.</summary>
            [ObservableProperty]
            private byte instantaneousHeartRate;
            /// <summary>Gets the heart rate source.</summary>
            [ObservableProperty]
            private HRDataSource heartRateSource;
            /// <summary>Gets the virtual speed flag.</summary>
            [ObservableProperty]
            private bool virtualSpeedFlag;
            /// <summary>Gets the distance traveled enabled flag.</summary>
            [ObservableProperty]
            private bool distanceTraveledEnabled;

            /// <summary>Parses the specified data page.</summary>
            /// <param name="dataPage">The data page.</param>
            internal void Parse(byte[] dataPage)
            {
                EquipmentType = (FitnessEquipmentType)dataPage[1];
                InstantaneousSpeed = BitConverter.ToUInt16(dataPage, 4) * 0.001;
                InstantaneousHeartRate = dataPage[6];
                HeartRateSource = (HRDataSource)(dataPage[7] & 0x03);
                DistanceTraveledEnabled = (dataPage[7] & 0x04) != 0;
                VirtualSpeedFlag = (dataPage[7] & 0x08) != 0;

                // update ElapseTime and DistanceTraveled
                if ((FEState)((dataPage[7] & 0x70) >> 4) == FEState.InUse)
                {
                    // in use, accumulate
                    if (!isFirstDataMessage)
                    {
                        ElapsedTime += TimeSpan.FromSeconds(Utils.CalculateDelta(dataPage[2], ref prevElapsedTime) * 0.25);
                        if (DistanceTraveledEnabled)
                        {
                            // distance enabled, accumulate
                            DistanceTraveled += Utils.CalculateDelta(dataPage[3], ref prevDistance);
                        }
                    }
                    else
                    {
                        // initialize state
                        prevElapsedTime = dataPage[2];
                        if (DistanceTraveledEnabled)
                        {
                            prevDistance = dataPage[3];
                        }
                        isFirstDataMessage = false;
                    }
                }
                else
                {
                    // not in use, stop accumulating
                    isFirstDataMessage = true;
                }
            }
        }

        /// <summary>This class supports fitness equipment general settings pages.</summary>
        public partial class GeneralSettingsPage : ObservableObject
        {
            /// <summary>Gets the length of the cycle.</summary>
            [ObservableProperty]
            private double cycleLength;
            /// <summary>Gets the incline.</summary>
            [ObservableProperty]
            private double incline;
            /// <summary>Gets the resistance level.</summary>
            [ObservableProperty]
            private double resistanceLevel;

            internal void Parse(byte[] dataPage)
            {
                CycleLength = dataPage[3] * 0.01;
                Incline = BitConverter.ToInt16(dataPage, 4) * 0.01;
                ResistanceLevel = dataPage[6] * 0.5;
            }
        }

        /// <summary>This class supports fitness equipment general metabolic pages.</summary>
        public partial class GeneralMetabolicPage : ObservableObject
        {
            private bool isFirstDataMessage = true;
            private byte prevCal = 0;

            /// <summary>Gets the instantaneous metabolic equivalent.</summary>
            [ObservableProperty]
            private double instantaneousMET;
            /// <summary>Gets the caloric burn rate.</summary>
            [ObservableProperty]
            private double caloricBurnRate;
            /// <summary>Gets the accumulated calories.</summary>
            [ObservableProperty]
            private int accumulatedCalories;

            internal void Parse(byte[] dataPage)
            {
                InstantaneousMET = BitConverter.ToUInt16(dataPage, 2) * 0.01;
                CaloricBurnRate = BitConverter.ToUInt16(dataPage, 4) * 0.1;
                if (!isFirstDataMessage)
                {
                    AccumulatedCalories += Utils.CalculateDelta(dataPage[6], ref prevCal);
                }
                else
                {
                    prevCal = dataPage[6];
                    isFirstDataMessage = false;
                }
            }
        }

        private bool lapToggleState;
        /// <summary>Occurs when a lap toggle is signaled by the device.</summary>
        /// <remarks>
        /// The event itself does not convey any information. It is the responsibility of the application/view model to
        /// capture any relevant state from the equipment and/or specific equipment for user consumption. For
        /// example, it may be useful to capture elapsed time and accumulated distance from the class.
        /// </remarks>
        public event EventHandler? LapToggled;

        /// <summary>Gets the equipment state.</summary>
        [ObservableProperty]
        private FEState state;
        /// <summary>Gets the general data.</summary>
        [ObservableProperty]
        private GeneralDataPage generalData = new GeneralDataPage();
        /// <summary>Gets the general settings.</summary>
        [ObservableProperty]
        private GeneralSettingsPage generalSettings = new GeneralSettingsPage();
        /// <summary>Gets the general metabolic reports.</summary>
        [ObservableProperty]
        private GeneralMetabolicPage generalMetabolic = new GeneralMetabolicPage();
        /// <summary>Gets the maximum trainer resistance.</summary>
        [ObservableProperty]
        private ushort maxTrainerResistance;
        /// <summary>Gets the supported training modes.</summary>
        [ObservableProperty]
        private SupportedTrainingModes trainingModes;

        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(FitnessEquipment).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.FE-C.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessEquipment"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        protected FitnessEquipment(ChannelId channelId, IAntChannel antChannel, ILogger logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessEquipment"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        protected FitnessEquipment(ChannelId channelId, IAntChannel antChannel, ILogger logger, TimeoutOptions? options)
            : base(channelId, antChannel, logger, options)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            handledPage = true;
            switch ((DataPage)dataPage[0])
            {
                // handle general pages
                case DataPage.GeneralFEData:
                    HandleFEState(dataPage[7]);
                    GeneralData.Parse(dataPage);

                    // check for lap toggle
                    if (lapToggleState != ((dataPage[7] & 0x80) == 0x80))
                    {
                        lapToggleState = (dataPage[7] & 0x80) == 0x80;
                        LapToggled?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case DataPage.GeneralSettings:
                    HandleFEState(dataPage[7]);
                    GeneralSettings.Parse(dataPage);
                    break;
                case DataPage.GeneralMetabolicData:
                    HandleFEState(dataPage[7]);
                    GeneralMetabolic.Parse(dataPage);
                    break;
                // handle specific FE pages
                case DataPage.FECapabilities:
                    MaxTrainerResistance = BitConverter.ToUInt16(dataPage, 5);
                    TrainingModes = (SupportedTrainingModes)dataPage[7];
                    break;
                default:
                    handledPage = false;
                    break;
            }
        }

        /// <summary>Handles the state of the fitness equipment.</summary>
        /// <param name="state">The state.</param>
        protected void HandleFEState(byte state)
        {
            var st = (state & 0x70) >> 4;
            // check for valid state
            if (Enum.IsDefined(typeof(FEState), st))
            {
                // state changed?
                if (State != (FEState)st)
                {
                    State = (FEState)st;
                }
            }
            else
            {
                _logger.LogWarning("Invalid state. Received {State}", st);
            }
        }

        /// <summary>Sets the percentage of maximum resistance resistance.</summary>
        /// <param name="resistance">The resistance as a percentage of the maximum resistance.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetBasicResistance(double resistance)
        {
            byte[] res = new byte[] { (byte)(resistance / 0.5) };
            byte[] msg = (new byte[] { (byte)DataPage.BasicResistance, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(res).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the target power in wats.</summary>
        /// <param name="power">The power in watts. Resolution is 0.25 watt.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetTargetPower(double power)
        {
            byte[] pow = BitConverter.GetBytes((ushort)(power / 0.25));
            byte[] msg = (new byte[] { (byte)DataPage.TargetPower, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(pow).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the wind resistance.</summary>
        /// <param name="windResistanceCoefficient">The wind resistance coefficient. Resolution is 0.01 kg/m.</param>
        /// <param name="windSpeed">The wind speed in km/h.</param>
        /// <param name="draftingFactor">The drafting scale factor. Range is 0 to 1.00.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetWindResistance(double windResistanceCoefficient, sbyte windSpeed, double draftingFactor)
        {
            byte[] wrc = new byte[] { (byte)(windResistanceCoefficient / 0.01) };
            byte[] df = new byte[] { (byte)(draftingFactor / 0.01) };
            byte[] msg = (new byte[] { (byte)DataPage.WindResistance, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(wrc).
                Concat(new byte[] { (byte)(windSpeed + 127) }).
                Concat(df).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the track resistance.</summary>
        /// <param name="grade">The grade. Set as a percentage of vertical displacement to horizontal displacement.</param>
        /// <param name="rollingResistanceCoefficient">The rolling resistance coefficient.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetTrackResistance(double grade, double rollingResistanceCoefficient = 0.004)
        {
            byte[] grd = BitConverter.GetBytes((ushort)((grade + 200) / 0.01));
            byte[] rrc = new byte[] { (byte)(rollingResistanceCoefficient / 0.00005) };
            byte[] msg = (new byte[] { (byte)DataPage.TrackResistance, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(grd).
                Concat(rrc).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the user configuration.</summary>
        /// <param name="userWeight">The user weight in kg.</param>
        /// <param name="wheelDiameterOffset">The wheel diameter offset in mm.</param>
        /// <param name="bikeWeight">The bike weight in kg.</param>
        /// <param name="wheelDiameter">The wheel diameter in meters.</param>
        /// <param name="gearRatio">The gear ratio.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetUserConfiguration(double userWeight, byte wheelDiameterOffset, double bikeWeight, double wheelDiameter, double gearRatio)
        {
            byte[] uw = BitConverter.GetBytes((ushort)(userWeight / 0.01));
            byte[] wdo_bw = BitConverter.GetBytes((ushort)((ushort)(bikeWeight / 0.05) << 4 | (wheelDiameterOffset & 0x0F)));
            byte[] wd = new byte[] { (byte)(wheelDiameter / 0.01) };
            byte[] gr = new byte[] { (byte)(gearRatio / 0.03) };
            byte[] msg = (new byte[] { (byte)DataPage.UserConfiguration }).
                Concat(uw).
                Concat(new byte[] { 0xFF }).
                Concat(wdo_bw).
                Concat(wd).
                Concat(gr).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Requests the fitness equipment capabilities.</summary>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> RequestFECapabilities()
        {
            return await RequestDataPage(DataPage.FECapabilities);
        }

        /// <summary>Gets the fitness equipment if the equipment type is known.</summary>
        /// <remarks>
        /// The specific fitness equipment type can be determined from the general data page or the FE specific data pages. Null will be
        /// returned for all other pages. Therefore, callers of this method need to check for a null return type.
        /// </remarks>
        /// <param name="dataPage">The data page.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ant channel.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>One of the known fitness equipment classes if known, otherwise null.</returns>
        public static FitnessEquipment? GetFitnessEquipment(byte[] dataPage, ChannelId channelId, IAntChannel antChannel, ILoggerFactory loggerFactory, int timeout)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.GeneralFEData:
                    switch ((FitnessEquipmentType)dataPage[1])
                    {
                        case FitnessEquipmentType.Treadmill:
                            return new Treadmill(channelId, antChannel, loggerFactory.CreateLogger<Treadmill>(), timeout);
                        case FitnessEquipmentType.Elliptical:
                            return new Elliptical(channelId, antChannel, loggerFactory.CreateLogger<Elliptical>(), timeout);
                        case FitnessEquipmentType.Rower:
                            return new Rower(channelId, antChannel, loggerFactory.CreateLogger<Rower>(), timeout);
                        case FitnessEquipmentType.Climber:
                            return new Climber(channelId, antChannel, loggerFactory.CreateLogger<Climber>(), timeout);
                        case FitnessEquipmentType.NordicSkier:
                            return new NordicSkier(channelId, antChannel, loggerFactory.CreateLogger<NordicSkier>(), timeout);
                        case FitnessEquipmentType.TrainerStationaryBike:
                            return new TrainerStationaryBike(channelId, antChannel, loggerFactory.CreateLogger<TrainerStationaryBike>(), timeout);
                        default:
                            loggerFactory.CreateLogger<FitnessEquipment>().LogError("Unknown equipment type = {EquipmentType}", dataPage[1]);
                            break;
                    }
                    break;
                case DataPage.TreadmillData:
                    return new Treadmill(channelId, antChannel, loggerFactory.CreateLogger<Treadmill>(), timeout);
                case DataPage.EllipticalData:
                    return new Elliptical(channelId, antChannel, loggerFactory.CreateLogger<Elliptical>(), timeout);
                case DataPage.RowerData:
                    return new Rower(channelId, antChannel, loggerFactory.CreateLogger<Rower>(), timeout);
                case DataPage.ClimberData:
                    return new Climber(channelId, antChannel, loggerFactory.CreateLogger<Climber>(), timeout);
                case DataPage.NordicSkierData:
                    return new NordicSkier(channelId, antChannel, loggerFactory.CreateLogger<NordicSkier>(), timeout);
                case DataPage.TrainerStationaryBikeData:
                    return new TrainerStationaryBike(channelId, antChannel, loggerFactory.CreateLogger<TrainerStationaryBike>(), timeout);
                case DataPage.TrainerTorqueData:
                    return new TrainerStationaryBike(channelId, antChannel, loggerFactory.CreateLogger<TrainerStationaryBike>(), timeout);
                default:
                    loggerFactory.CreateLogger<FitnessEquipment>().LogError("Unknown equipment type. Data page = {DataPage}", dataPage);
                    break;
            }
            return null;
        }
    }
}
