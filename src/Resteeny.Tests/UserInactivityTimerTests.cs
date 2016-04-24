using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Resteeny.Tests
{
    [TestClass]
    public class UserInactivityTimerTests
    {
        [TestMethod]
        public void RaisesEventWhenTimerElapsed()
        {
            // Arrange
            var inactivityTimeMock = new Mock<IUserIdleTime>();
            inactivityTimeMock.Setup(it => it.Get()).Returns(TimeSpan.FromMilliseconds(2000));

            var sut = new UserInactivityTimer(TimeSpan.FromMilliseconds(2000), inactivityTimeMock.Object);
            var raised = false;
            sut.Elapsed += (o, e) => raised = true;

            // To avoid having the unit test to run forever if the tested event is never raised,
            // create a fallback timer.
            var fallbackTimer = new System.Timers.Timer(3000);
            var fallbackRaised = false;
            fallbackTimer.Elapsed += (o, e) => fallbackRaised = true;

            // Act
            sut.Start();
            fallbackTimer.Start();
            while (!fallbackRaised || !raised)
            {
            }
            fallbackTimer.Stop();
            sut.Stop();

            // Assert
            Assert.IsTrue(raised);
        }
    }
}