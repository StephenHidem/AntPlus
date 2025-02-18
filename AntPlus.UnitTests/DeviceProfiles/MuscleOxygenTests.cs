using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;

namespace AntPlus.UnitTests.DeviceProfiles
{
    public class MuscleOxygenTests
    {
        readonly ChannelId cid = new(0);
        private readonly MockRepository mockRepository;
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<MuscleOxygen>> mockLogger;

        public MuscleOxygenTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<MuscleOxygen>>(MockBehavior.Loose);
        }

        [Theory]
        [InlineData(new byte[] { 1, 1, 0, 0, 0, 0, 0, 0 }, false)]
        [InlineData(new byte[] { 1, 2, 1, 0, 0, 0, 0, 0 }, true)]
        public void Parse_Notifications_Match(byte[] dataPage, bool expected)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.Equal(expected, muscleOxygen.UtcTimeRequired);
        }

        [Theory]
        [InlineData(new byte[] { 1, 1, 0, 0, 0, 0, 0, 0 }, false, MeasurementInterval.None)]
        [InlineData(new byte[] { 1, 2, 0, 1, 0, 0, 0, 0 }, true, MeasurementInterval.None)]
        [InlineData(new byte[] { 1, 3, 0, 2, 0, 0, 0, 0 }, false, MeasurementInterval.QuarterSecond)]
        [InlineData(new byte[] { 1, 4, 0, 4, 0, 0, 0, 0 }, false, MeasurementInterval.HalfSecond)]
        [InlineData(new byte[] { 1, 5, 0, 6, 0, 0, 0, 0 }, false, MeasurementInterval.OneSecond)]
        [InlineData(new byte[] { 1, 6, 0, 8, 0, 0, 0, 0 }, false, MeasurementInterval.TwoSecond)]
        public void Parse_Capabilities_Match(byte[] dataPage, bool antFs, MeasurementInterval interval)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.Equal(antFs, muscleOxygen.SupportsAntFs);
            Assert.Equal(interval, muscleOxygen.Interval);
        }

        [Theory]
        [InlineData(new byte[] { 1, 1, 0, 0, 0xFF, 0x0F, 0, 0 }, double.NaN, MeasurementStatus.Invalid)]
        [InlineData(new byte[] { 1, 2, 0, 0, 0xFE, 0x0F, 0, 0 }, double.NaN, MeasurementStatus.AmbientLightTooHigh)]
        [InlineData(new byte[] { 1, 3, 0, 0, 0xD0, 0x07, 0, 0 }, 20.0, MeasurementStatus.Valid)]
        public void Parse_TotalHemoglobinConcentration_Match(byte[] dataPage, double concentration, MeasurementStatus status)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.Equal(concentration, muscleOxygen.TotalHemoglobinConcentration.Concentration);
            Assert.Equal(status, muscleOxygen.TotalHemoglobinConcentration.Status);
        }

        [Theory]
        [InlineData(new byte[] { 1, 1, 0, 0, 0, 0xF0, 0x3F, 0 }, double.NaN, MeasurementStatus.Invalid)]
        [InlineData(new byte[] { 1, 2, 0, 0, 0, 0xE0, 0x3F, 0 }, double.NaN, MeasurementStatus.AmbientLightTooHigh)]
        [InlineData(new byte[] { 1, 3, 0, 0, 0, 0x80, 0x0C, 0 }, 20.0, MeasurementStatus.Valid)]
        public void Parse_PreviousSaturatedHemoglobin_Match(byte[] dataPage, double concentration, MeasurementStatus status)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.Equal(concentration, muscleOxygen.PreviousSaturatedHemoglobin.PercentSaturated);
            Assert.Equal(status, muscleOxygen.PreviousSaturatedHemoglobin.Status);
        }

        [Theory]
        [InlineData(new byte[] { 1, 1, 0, 0, 0, 0, 0xC0, 0xFF }, double.NaN, MeasurementStatus.Invalid)]
        [InlineData(new byte[] { 1, 2, 0, 0, 0, 0, 0x80, 0xFF }, double.NaN, MeasurementStatus.AmbientLightTooHigh)]
        [InlineData(new byte[] { 1, 3, 0, 0, 0, 0, 0x00, 0x32 }, 20.0, MeasurementStatus.Valid)]
        public void Parse_CurrentSaturatedHemoglobin_Match(byte[] dataPage, double concentration, MeasurementStatus status)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.Equal(concentration, muscleOxygen.CurrentSaturatedHemoglobin.PercentSaturated);
            Assert.Equal(status, muscleOxygen.CurrentSaturatedHemoglobin.Status);
        }

        [Theory]
        [InlineData(CommandId.SetTime)]
        [InlineData(CommandId.StartSession)]
        [InlineData(CommandId.StopSession)]
        [InlineData(CommandId.Lap)]
        public async Task SendCommand_PageSent_PageMatches(CommandId command)
        {
            // Arrange
            TimeSpan localTimeOffset = new(6, 0, 0);
            DateTime currentTimeStamp = DateTime.UtcNow;
            mockAntChannel.Setup(s => s.SendExtAcknowledgedDataAsync(
                cid, It.Is<byte[]>(cmd => (CommandId)cmd[1] == command &&
                cmd[3] == localTimeOffset.TotalMinutes / 15 &&
                BitConverter.ToUInt32(cmd, 4) == (uint)(currentTimeStamp - new DateTime(1989, 12, 31)).TotalSeconds),
                It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            var muscleOxygen = new MuscleOxygen(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            var result = await muscleOxygen.SendCommand(
                command,
                localTimeOffset,
                currentTimeStamp);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
        }
    }
}
