using AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This is the primary support class for fitness equipment sensors.
    /// </summary>
    /// <seealso cref="AntPlus.AntDevice" />
    public class FitnessEquipment : AntDevice
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
            Treadmill = 0x13,
            Elliptical = 0x14,
            Rower = 0x16,
            Climber = 0x17,
            NordicSkier = 0x18,
            TrainerStationaryBike = 0x19
        }

        public enum HRDataSource
        {
            Invalid,
            HeartRateMonitor,
            EMHeartRateMonitor,
            HandContactSensors
        }

        public enum FEState
        {
            AsleepOrOff = 1,
            Ready,
            InUse,
            FinishedOrPaused
        }

        [Flags]
        public enum SupportedTrainingModes
        {
            BasicResistance = 0x01,
            TargetPower = 0x02,
            Simulation = 0x04
        }

        public class GeneralDataPage
        {
            private bool isFirstDataMessage = true;
            private byte prevElapsedTime;
            private byte prevDistance;

            public FitnessEquipmentType EquipmentType { get; private set; }
            public TimeSpan ElapsedTime { get; private set; }
            public int DistanceTraveled { get; private set; }
            /// <summary>Gets the instantaneous speed.</summary>
            /// <value>The instantaneous speed in meters per second.</value>
            public double InstantaneousSpeed { get; private set; }
            public byte InstantaneousHeartRate { get; private set; }
            public HRDataSource HeartRateSource { get; private set; }
            public bool VirtualSpeedFlag { get; private set; }

            public void Parse(byte[] dataPage)
            {
                EquipmentType = (FitnessEquipmentType)dataPage[1];
                ElapsedTime += TimeSpan.FromSeconds(Utils.CalculateDelta(dataPage[2], ref prevElapsedTime) * 0.25);
                if ((dataPage[7] & 0x04) != 0)
                {
                    if (!isFirstDataMessage)
                    {
                        DistanceTraveled += Utils.CalculateDelta(dataPage[3], ref prevDistance);
                    }
                    else
                    {
                        prevDistance = dataPage[3];
                        isFirstDataMessage = false;
                    }
                }
                InstantaneousSpeed = BitConverter.ToInt16(dataPage, 4) * 0.001;
                InstantaneousHeartRate = dataPage[6];
                HeartRateSource = (HRDataSource)(dataPage[7] & 0x03);
                VirtualSpeedFlag = (dataPage[7] & 0x08) != 0;
            }
        }

        public class GeneralSettingsPage
        {
            public double CycleLength { get; private set; }
            public double Incline { get; private set; }
            public double ResistanceLevel { get; private set; }

            public void Parse(byte[] dataPage)
            {
                CycleLength = dataPage[3] * 0.01;
                Incline = BitConverter.ToInt16(dataPage, 4) * 0.01;
                ResistanceLevel = dataPage[6] * 0.5;
            }
        }

        public class GeneralMetabolicPage
        {
            private bool isFirstDataMessage = true;
            private byte prevCal = 0;

            public double InstantaneousMET { get; private set; }
            public double CaloricBurnRate { get; private set; }
            public int AccumulatedCalories { get; private set; }

            public void Parse(byte[] dataPage)
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


        public FEState State { get; private set; }
        public bool LapToggle { get; private set; }
        public GeneralDataPage GeneralData { get; private set; }
        public GeneralSettingsPage GeneralSettings { get; private set; }
        public GeneralMetabolicPage GeneralMetabolic { get; private set; }
        public Treadmill Treadmill { get; private set; }
        public Elliptical Elliptical { get; private set; }
        public Rower Rower { get; private set; }
        public Climber Climber { get; private set; }
        public NordicSkier NordicSkier { get; private set; }
        public TrainerStationaryBike TrainerStationaryBike { get; private set; }
        public ushort MaxTrainerResistance { get; private set; }
        public SupportedTrainingModes Capabilities { get; private set; }
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();

        public FitnessEquipment(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            GeneralData = new GeneralDataPage();
            GeneralSettings = new GeneralSettingsPage();
            GeneralMetabolic = new GeneralMetabolicPage();
        }

        public override void Parse(byte[] dataPage)
        {
            State = (FEState)((dataPage[7] & 0x70) >> 4);
            LapToggle = (dataPage[7] & 0x80) == 0x80;

            switch ((DataPage)dataPage[0])
            {
                // handle general pages
                case DataPage.GeneralFEData:
                    GeneralData.Parse(dataPage);
                    if (!IsKnownEquipmentType(GeneralData.EquipmentType))
                    {
                        CreateSpecificEquipment(GeneralData.EquipmentType);
                    }
                    RaisePropertyChange(nameof(GeneralData));
                    break;
                case DataPage.GeneralSettings:
                    GeneralSettings.Parse(dataPage);
                    RaisePropertyChange(nameof(GeneralSettings));
                    break;
                case DataPage.GeneralMetabolicData:
                    GeneralMetabolic.Parse(dataPage);
                    RaisePropertyChange(nameof(GeneralMetabolic));
                    break;
                // handle specific FE pages
                case DataPage.TreadmillData:
                    Treadmill?.Parse(dataPage);
                    break;
                case DataPage.EllipticalData:
                    Elliptical?.Parse(dataPage);
                    break;
                case DataPage.RowerData:
                    Rower?.Parse(dataPage);
                    break;
                case DataPage.ClimberData:
                    Climber?.Parse(dataPage);
                    break;
                case DataPage.NordicSkierData:
                    NordicSkier?.Parse(dataPage);
                    break;
                case DataPage.TrainerStationaryBikeData:
                    TrainerStationaryBike?.Parse(dataPage);
                    break;
                case DataPage.TrainerTorqueData:
                    TrainerStationaryBike?.ParseTorque(dataPage);
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

        public void SetBasicResistance(double resistance)
        {
            byte[] res = new byte[] { (byte)(resistance / 0.5) };
            byte[] msg = (new byte[] { (byte)DataPage.BasicResistance, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(res).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

        public void SetTargetPower(double power)
        {
            byte[] pow = BitConverter.GetBytes((ushort)(power / 0.25));
            byte[] msg = (new byte[] { (byte)DataPage.TargetPower, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(pow).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

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

        public void SetTrackResistance(double grade, double rollingResistanceCoefficient = 0.004)
        {
            byte[] grd = BitConverter.GetBytes((ushort)((grade + 200) / 0.01));
            byte[] rrc = new byte[] { (byte)(rollingResistanceCoefficient / 0.00005) };
            byte[] msg = (new byte[] { (byte)DataPage.TrackResistance, 0xFF, 0xFF, 0xFF, 0xFF }).
                Concat(grd).
                Concat(rrc).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

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
