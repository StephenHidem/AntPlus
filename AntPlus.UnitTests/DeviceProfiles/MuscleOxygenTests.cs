using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen.MuscleOxygen;

namespace AntPlus.UnitTests.DeviceProfiles
{
    [TestClass]
    public class MuscleOxygenTests
    {
        ChannelId cid = new(0);

        [TestMethod]
        [DataRow(new byte[] { 1, 1, 0, 0, 0, 0, 0, 0 }, false)]
        [DataRow(new byte[] { 1, 2, 1, 0, 0, 0, 0, 0 }, true)]
        public void Parse_Notifications_Match(byte[] dataPage, bool expected)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expected, muscleOxygen.UtcTimeRequired);
        }

        [TestMethod]
        [DataRow(new byte[] { 1, 1, 0, 0, 0, 0, 0, 0 }, false, MeasurementInterval.None)]
        [DataRow(new byte[] { 1, 2, 0, 1, 0, 0, 0, 0 }, true, MeasurementInterval.None)]
        [DataRow(new byte[] { 1, 3, 0, 2, 0, 0, 0, 0 }, false, MeasurementInterval.QuarterSecond)]
        [DataRow(new byte[] { 1, 4, 0, 4, 0, 0, 0, 0 }, false, MeasurementInterval.HalfSecond)]
        [DataRow(new byte[] { 1, 5, 0, 6, 0, 0, 0, 0 }, false, MeasurementInterval.OneSecond)]
        [DataRow(new byte[] { 1, 6, 0, 8, 0, 0, 0, 0 }, false, MeasurementInterval.TwoSecond)]
        public void Parse_Capabilities_Match(byte[] dataPage, bool antFs, MeasurementInterval interval)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(antFs, muscleOxygen.SupportsAntFs);
            Assert.AreEqual(interval, muscleOxygen.Interval);
        }

        [TestMethod]
        [DataRow(new byte[] { 1, 1, 0, 0, 0xFF, 0x0F, 0, 0 }, double.NaN, MeasuremantStatus.Invalid)]
        [DataRow(new byte[] { 1, 2, 0, 0, 0xFE, 0x0F, 0, 0 }, double.NaN, MeasuremantStatus.AmbientLightTooHigh)]
        [DataRow(new byte[] { 1, 3, 0, 0, 0xD0, 0x07, 0, 0 }, 20.0, MeasuremantStatus.Valid)]
        public void Parse_TotalHemoglobinConcentration_Match(byte[] dataPage, double concentration, MeasuremantStatus status)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(concentration, muscleOxygen.TotalHemoglobinConcentration.Concentration);
            Assert.AreEqual(status, muscleOxygen.TotalHemoglobinConcentration.Status);
        }

        [TestMethod]
        [DataRow(new byte[] { 1, 1, 0, 0, 0, 0xF0, 0x3F, 0 }, double.NaN, MeasuremantStatus.Invalid)]
        [DataRow(new byte[] { 1, 2, 0, 0, 0, 0xE0, 0x3F, 0 }, double.NaN, MeasuremantStatus.AmbientLightTooHigh)]
        [DataRow(new byte[] { 1, 3, 0, 0, 0, 0x80, 0x0C, 0 }, 20.0, MeasuremantStatus.Valid)]
        public void Parse_PreviousSaturatedHemoglobin_Match(byte[] dataPage, double concentration, MeasuremantStatus status)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(concentration, muscleOxygen.PreviousSaturatedHemoglobin.PercentSaturated);
            Assert.AreEqual(status, muscleOxygen.PreviousSaturatedHemoglobin.Status);
        }

        [TestMethod]
        [DataRow(new byte[] { 1, 1, 0, 0, 0, 0, 0xC0, 0xFF }, double.NaN, MeasuremantStatus.Invalid)]
        [DataRow(new byte[] { 1, 2, 0, 0, 0, 0, 0x80, 0xFF }, double.NaN, MeasuremantStatus.AmbientLightTooHigh)]
        [DataRow(new byte[] { 1, 3, 0, 0, 0, 0, 0x00, 0x32 }, 20.0, MeasuremantStatus.Valid)]
        public void Parse_CurrentSaturatedHemoglobin_Match(byte[] dataPage, double concentration, MeasuremantStatus status)
        {
            // Arrange
            var muscleOxygen = new MuscleOxygen(cid, null);

            // Act
            muscleOxygen.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(concentration, muscleOxygen.CurrentSaturatedHemoglobin.PercentSaturated);
            Assert.AreEqual(status, muscleOxygen.CurrentSaturatedHemoglobin.Status);
        }

        [TestMethod]
        [DataRow(CommandId.SetTime)]
        [DataRow(CommandId.StartSession)]
        [DataRow(CommandId.StopSession)]
        [DataRow(CommandId.Lap)]
        public void SendCommand_StateUnderTest_ExpectedBehavior(CommandId command)
        {
            // Arrange
            TimeSpan localTimeOffest = new(6, 0, 0);
            DateTime currentTimeStamp = DateTime.UtcNow;
            Mock<IAntChannel> antChannel = new();
            antChannel.Setup(s => s.SendExtAcknowledgedData(
                cid, It.Is<byte[]>(cmd => (CommandId)cmd[1] == command &&
                cmd[3] == localTimeOffest.TotalMinutes / 15 &&
                BitConverter.ToInt32(cmd, 4) == (int)(currentTimeStamp - new DateTime(1989, 12, 31)).TotalSeconds),
                It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);
            var muscleOxygen = new MuscleOxygen(cid, antChannel.Object);

            // Act
            var result = muscleOxygen.SendCommand(
                command,
                localTimeOffest,
                currentTimeStamp);

            // Assert
            Assert.IsTrue(result == MessagingReturnCode.Pass);
        }
    }
}
