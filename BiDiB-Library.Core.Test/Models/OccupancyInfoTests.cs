using System;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Models.BiDiB;
using Timer = System.Timers.Timer;

namespace org.bidib.netbidibc.core.Test.Models
{
    [TestClass]
    [TestCategory(TestCategory.UnitTest)]
    public class OccupancyInfoTests : TestClass<OccupancyInfo>
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();

            Target = new OccupancyInfo();
        }

        [TestMethod]
        [DataRow(nameof(OccupancyInfo.Address), (ushort)1)]
        [DataRow(nameof(OccupancyInfo.Container1), 1)]
        [DataRow(nameof(OccupancyInfo.Container2), 1)]
        [DataRow(nameof(OccupancyInfo.Container3), 1)]
        [DataRow(nameof(OccupancyInfo.Direction), DecoderDirection.BackwardDirection)]
        [DataRow(nameof(OccupancyInfo.Speed), 20)]
        [DataRow(nameof(OccupancyInfo.Temperature), 20)]
        [DataRow(nameof(OccupancyInfo.Quality), 20)]
        public void PropertyChange_ShouldSetIsAliveAndLastUpdate(string propertyName, object value)
        {
            // Arrange
            PropertyInfo pi = typeof(OccupancyInfo).GetProperty(propertyName);
            pi.Should().NotBeNull();

            // Act
            pi.SetValue(Target, value);

            // Assert
            Target.IsAlive.Should().BeTrue();
            Target.LastUpdate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TimerEvent_ShouldSetIsAliveToFalse()
        {
            // Arrange
            ((Timer)Target.GetType().GetField("resetTimer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Target)).Interval = 50;

            // Act
            Target.Container1 = 20;
            Thread.Sleep(100);

            // Assert
            Target.IsAlive.Should().BeFalse();
        }

        [TestMethod]
        public void ToString_ShouldReturnAddressValue()
        {
            // Arrange
            Target.Address = 100;

            // Act
            string stringValue = Target.ToString();

            // Assert
            stringValue.Should().Be("OccupancyInfo A:100");
        }

        [TestMethod]
        public void Dispose_ShouldDisposeTimer()
        {
            // Arrange
            var timer = ((Timer)Target.GetType().GetField("resetTimer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Target));
            var timerDisposed = false;
            timer.Disposed += (_, _) => timerDisposed = true;

            // Act
            Target.Dispose();

            // Assert
            timerDisposed.Should().BeTrue();
        }
    }
}
