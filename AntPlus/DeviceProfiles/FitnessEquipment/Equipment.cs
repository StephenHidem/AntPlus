using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This is the primary support class for fitness equipment sensors.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public class Equipment : AntDevice
    {
        /// <summary>
        /// The fitness equipment device class ID.
        /// </summary>
        public const byte DeviceClass = 17;

        /// <summary>
        /// General main data pages.
        /// </summary>
        private enum DataPage
        {
            GeneralFEData = 0x10,
            GeneralSettings = 0x11,
            GeneralMetabolicData = 0x12,
            TreadmillData = 0x13,
            EllipticalData = 0x14,
            RowerData = 0x16,
            ClimberData = 0x17,
            NordicSkierData = 0x18,
            TrainerStationaryBikeData = 0x19,
            TrainerTorqueData = 0x1A,
            BasicResistance = 0x30,
            TargetPower = 0x31,
            WindResistance = 0x32,
            TrackResistance = 0x33,
            FECapabilities = 0x36,
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
            /// <summary>Basic resistance</summary>
            BasicResistance = 0x01,
            /// <summary>Target power</summary>
            TargetPower = 0x02,
            /// <summary>Simulation</summary>
            Simulation = 0x04
        }

        /// <summary>This class supports fitness equipment general data pages.</summary>
        public class GeneralDataPage
        {
            private bool isFirstDataMessage = true;
            private byte prevElapsedTime;
            private byte prevDistance;

            /// <summary>Gets the type of the equipment.</summary>
            public FitnessEquipmentType EquipmentType { get; private set; }
            /// <summary>Gets the elapsed time.</summary>
            public TimeSpan ElapsedTime { get; private set; }
            /// <summary>Gets the distance traveled in meters.</summary>
            public int DistanceTraveled { get; private set; }
            /// <summary>Gets the instantaneous speed.</summary>
            /// <value>The instantaneous speed in meters per second.</value>
            public double InstantaneousSpeed { get; private set; }
            /// <summary>Gets the instantaneous heart rate.</summary>
            public byte InstantaneousHeartRate { get; private set; }
            /// <summary>Gets the heart rate source.</summary>
            public HRDataSource HeartRateSource { get; private set; }
            /// <summary>Gets the virtual speed flag.</summary>
            public bool VirtualSpeedFlag { get; private set; }
            /// <summary>Gets the distance traveled enabled flag.</summary>
            public bool DistanceTraveledEnabled { get; private set; }

            /// <summary>Parses the specified data page.</summary>
            /// <param name="dataPage">The data page.</param>
            internal void Parse(byte[] dataPage)
            {
                EquipmentType = (FitnessEquipmentType)dataPage[1];
                if (!isFirstDataMessage)
                {
                    ElapsedTime += TimeSpan.FromSeconds(Utils.CalculateDelta(dataPage[2], ref prevElapsedTime) * 0.25);
                    DistanceTraveled += Utils.CalculateDelta(dataPage[3], ref prevDistance);
                }
                else
                {
                    prevElapsedTime = dataPage[2];
                    prevDistance = dataPage[3];
                    isFirstDataMessage = false;
                }
                InstantaneousSpeed = BitConverter.ToUInt16(dataPage, 4) * 0.001;
                InstantaneousHeartRate = dataPage[6];
                HeartRateSource = (HRDataSource)(dataPage[7] & 0x03);
                DistanceTraveledEnabled = (dataPage[7] & 0x04) != 0;
                VirtualSpeedFlag = (dataPage[7] & 0x08) != 0;
            }
        }

        /// <summary>This class supports fitness equipment general settings pages.</summary>
        public class GeneralSettingsPage
        {
            /// <summary>Gets the length of the cycle.</summary>
            public double CycleLength { get; private set; }
            /// <summary>Gets the incline.</summary>
            public double Incline { get; private set; }
            /// <summary>Gets the resistance level.</summary>
            public double ResistanceLevel { get; private set; }

            internal void Parse(byte[] dataPage)
            {
                CycleLength = dataPage[3] * 0.01;
                Incline = BitConverter.ToInt16(dataPage, 4) * 0.01;
                ResistanceLevel = dataPage[6] * 0.5;
            }
        }

        /// <summary>This class supports fitness equipment general metabolic pages.</summary>
        public class GeneralMetabolicPage
        {
            private bool isFirstDataMessage = true;
            private byte prevCal = 0;

            /// <summary>Gets the instantaneous metabolic equivalent.</summary>
            public double InstantaneousMET { get; private set; }
            /// <summary>Gets the caloric burn rate.</summary>
            public double CaloricBurnRate { get; private set; }
            /// <summary>Gets the accumulated calories.</summary>
            public int AccumulatedCalories { get; private set; }

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


        /// <summary>Gets the equipment state.</summary>
        public FEState State { get; private set; }
        /// <summary>Gets a value indicating whether [lap toggle].</summary>
        public bool LapToggle { get; private set; }

        /// <summary>Gets the general data.</summary>
        public GeneralDataPage GeneralData { get; private set; }
        /// <summary>Gets the general settings.</summary>
        public GeneralSettingsPage GeneralSettings { get; private set; }
        /// <summary>Gets the general metabolic reports.</summary>
        public GeneralMetabolicPage GeneralMetabolic { get; private set; }
        /// <summary>Gets the treadmill equipment class if detected, else null.</summary>
        public Treadmill Treadmill { get; private set; }
        /// <summary>Gets the elliptical equipment class if detected, else null.</summary>
        public Elliptical Elliptical { get; private set; }
        /// <summary>Gets the rower equipment class if detected, else null.</summary>
        public Rower Rower { get; private set; }
        /// <summary>Gets the climber equipment class if detected, else null.</summary>
        public Climber Climber { get; private set; }
        /// <summary>Gets the Nordic skier equipment class if detected, else null.</summary>
        public NordicSkier NordicSkier { get; private set; }
        /// <summary>Gets the stationary bike equipment class if detected, else null.</summary>
        public TrainerStationaryBike TrainerStationaryBike { get; private set; }
        /// <summary>Gets the maximum trainer resistance.</summary>
        public ushort MaxTrainerResistance { get; private set; }
        /// <summary>Gets the supported capabilities.</summary>
        public SupportedTrainingModes Capabilities { get; private set; }
        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(Equipment).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.FE-C.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="Equipment"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public Equipment(ChannelId channelId, IAntChannel antChannel, ILogger<Equipment> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
            GeneralData = new GeneralDataPage();
            GeneralSettings = new GeneralSettingsPage();
            GeneralMetabolic = new GeneralMetabolicPage();
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            switch ((DataPage)dataPage[0])
            {
                // handle general pages
                case DataPage.GeneralFEData:
                    HandleFEState(dataPage[7]);
                    GeneralData.Parse(dataPage);
                    if (!IsKnownEquipmentType(GeneralData.EquipmentType))
                    {
                        CreateSpecificEquipment(GeneralData.EquipmentType);
                    }
                    RaisePropertyChange(nameof(GeneralData));
                    break;
                case DataPage.GeneralSettings:
                    HandleFEState(dataPage[7]);
                    GeneralSettings.Parse(dataPage);
                    RaisePropertyChange(nameof(GeneralSettings));
                    break;
                case DataPage.GeneralMetabolicData:
                    HandleFEState(dataPage[7]);
                    GeneralMetabolic.Parse(dataPage);
                    RaisePropertyChange(nameof(GeneralMetabolic));
                    break;
                // handle specific FE pages
                case DataPage.TreadmillData:
                    HandleFEState(dataPage[7]);
                    Treadmill?.Parse(dataPage);
                    break;
                case DataPage.EllipticalData:
                    HandleFEState(dataPage[7]);
                    Elliptical?.Parse(dataPage);
                    break;
                case DataPage.RowerData:
                    HandleFEState(dataPage[7]);
                    Rower?.Parse(dataPage);
                    break;
                case DataPage.ClimberData:
                    HandleFEState(dataPage[7]);
                    Climber?.Parse(dataPage);
                    break;
                case DataPage.NordicSkierData:
                    HandleFEState(dataPage[7]);
                    NordicSkier?.Parse(dataPage);
                    break;
                case DataPage.TrainerStationaryBikeData:
                    HandleFEState(dataPage[7]);
                    TrainerStationaryBike?.Parse(dataPage);
                    break;
                case DataPage.TrainerTorqueData:
                    HandleFEState(dataPage[7]);
                    TrainerStationaryBike?.TrainerTorque.Parse(dataPage);
                    break;
                case DataPage.FECapabilities:
                    MaxTrainerResistance = BitConverter.ToUInt16(dataPage, 5);
                    Capabilities = (SupportedTrainingModes)dataPage[7];
                    RaisePropertyChange(nameof(MaxTrainerResistance));
                    RaisePropertyChange(nameof(Capabilities));
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Fitness Equipment";
        }

        private void HandleFEState(byte state)
        {
            var st = (state & 0x70) >> 4;
            // check for valid state
            if (Enum.IsDefined(typeof(FEState), st))
            {
                // state changed?
                if (State != (FEState)st)
                {
                    State = (FEState)st;
                    RaisePropertyChange(nameof(State));
                }
            }
            else
            {
                logger.LogWarning("Invalid state. Received {State}", st);
            }
            LapToggle = (state & 0x80) == 0x80;
            RaisePropertyChange(nameof(LapToggle));
        }

        /// <summary>Sets the percentage of maximum resistance resistance.</summary>
        /// <param name="resistance">The resistance.</param>
        public void SetBasicResistance(double resistance)
        {
            byte[] res = new byte[] { (byte)(resistance / 0.5) };
            byte[] msg = (new byte[] { (byte)DataPage.BasicResistance, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(res).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the target power in wats.</summary>
        /// <param name="power">The power in watts. Resolution is 0.25 watt.</param>
        public void SetTargetPower(double power)
        {
            byte[] pow = BitConverter.GetBytes((ushort)(power / 0.25));
            byte[] msg = (new byte[] { (byte)DataPage.TargetPower, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(pow).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the wind resistance.</summary>
        /// <param name="windResistanceCoefficient">The wind resistance coefficient. Resolution is 0.01 kg/m.</param>
        /// <param name="windSpeed">The wind speed in km/h.</param>
        /// <param name="draftingFactor">The drafting scale factor. Range is 0 to 1.00.</param>
        public void SetWindResistance(double windResistanceCoefficient, sbyte windSpeed, double draftingFactor)
        {
            byte[] wrc = new byte[] { (byte)(windResistanceCoefficient / 0.01) };
            byte[] df = new byte[] { (byte)(draftingFactor / 0.01) };
            byte[] msg = (new byte[] { (byte)DataPage.WindResistance, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(wrc).
                Concat(new byte[] { (byte)(windSpeed + 127) }).
                Concat(df).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the track resistance.</summary>
        /// <param name="grade">The grade. Set as a percentage of vertical displacement to horizontal displacement.</param>
        /// <param name="rollingResistanceCoefficient">The rolling resistance coefficient.</param>
        public void SetTrackResistance(double grade, double rollingResistanceCoefficient = 0.004)
        {
            byte[] grd = BitConverter.GetBytes((ushort)((grade + 200) / 0.01));
            byte[] rrc = new byte[] { (byte)(rollingResistanceCoefficient / 0.00005) };
            byte[] msg = (new byte[] { (byte)DataPage.TrackResistance, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(grd).
                Concat(rrc).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Sets the user configuration.</summary>
        /// <param name="userWeight">The user weight in kg.</param>
        /// <param name="wheelDiameterOffset">The wheel diameter offset in mm.</param>
        /// <param name="bikeWeight">The bike weight in kg.</param>
        /// <param name="wheelDiameter">The wheel diameter in meters.</param>
        /// <param name="gearRatio">The gear ratio.</param>
        public void SetUserConfiguration(double userWeight, byte wheelDiameterOffset, double bikeWeight, double wheelDiameter, double gearRatio)
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
            SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Requests the fitness equipment capabilities.</summary>
        public void RequestFECapabilities()
        {
            RequestDataPage(DataPage.FECapabilities);
        }

        private bool IsKnownEquipmentType(FitnessEquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    return Treadmill != null;
                case FitnessEquipmentType.Elliptical:
                    return Elliptical != null;
                case FitnessEquipmentType.Rower:
                    return Rower != null;
                case FitnessEquipmentType.Climber:
                    return Climber != null;
                case FitnessEquipmentType.NordicSkier:
                    return NordicSkier != null;
                case FitnessEquipmentType.TrainerStationaryBike:
                    return TrainerStationaryBike != null;
                default:
                    return false;
            }
        }

        private void CreateSpecificEquipment(FitnessEquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    Treadmill = new Treadmill();
                    break;
                case FitnessEquipmentType.Elliptical:
                    Elliptical = new Elliptical();
                    break;
                case FitnessEquipmentType.Rower:
                    Rower = new Rower();
                    break;
                case FitnessEquipmentType.Climber:
                    Climber = new Climber();
                    break;
                case FitnessEquipmentType.NordicSkier:
                    NordicSkier = new NordicSkier();
                    break;
                case FitnessEquipmentType.TrainerStationaryBike:
                    TrainerStationaryBike = new TrainerStationaryBike();
                    break;
                default:
                    break;
            }
        }
    }
}
