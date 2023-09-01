using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class BicyclePowerTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Bicycle>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<ILogger<Bicycle>>(MockBehavior.Loose);
        }

        private Bicycle CreateBicyclePower()
        {
            return new Bicycle(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object);
        }

        [TestMethod]
        [DataRow(16, SensorType.Power)]
        [DataRow(17, SensorType.WheelTorque)]
        [DataRow(18, SensorType.CrankTorque)]
        [DataRow(32, SensorType.CrankTorqueFrequency)]
        public void Parse_CreateSensor_ExpectedSensor(int page, SensorType sensorType)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)page;

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(sensorType, bicyclePower.Sensor);
            switch (sensorType)
            {
                case SensorType.Power:
                    Assert.IsNotNull(bicyclePower.PowerSensor);
                    Assert.IsNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CrankTorqueSensor);
                    Assert.IsNull(bicyclePower.CTFSensor);
                    break;
                case SensorType.WheelTorque:
                    Assert.IsNotNull(bicyclePower.PowerSensor);
                    Assert.IsNotNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CrankTorqueSensor);
                    Assert.IsNull(bicyclePower.CTFSensor);
                    break;
                case SensorType.CrankTorque:
                    Assert.IsNotNull(bicyclePower.PowerSensor);
                    Assert.IsNotNull(bicyclePower.CrankTorqueSensor);
                    Assert.IsNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CTFSensor);
                    break;
                case SensorType.CrankTorqueFrequency:
                    Assert.IsNull(bicyclePower.PowerSensor);
                    Assert.IsNotNull(bicyclePower.CTFSensor);
                    Assert.IsNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CrankTorqueSensor);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }
}
