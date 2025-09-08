using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using Xunit;

namespace OWP.PushInfoToSettlement.UnitTests.Services
{
    public class EventServiceIntegrationTests
    {
        [Fact]
        public void EventServiceImp_CanBeInstantiated()
        {
            // Arrange & Act
            var service = new EventServiceImp(null);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void EventData_PropertiesWorkCorrectly()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "Test remark",
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal(1, eventData.OWPMatchId);
            Assert.Equal(100, eventData.AdminSiteId);
            Assert.Equal("Test remark", eventData.Remark);
            Assert.Equal(DateTime.Today, eventData.EventDateTime);
        }

        [Fact]
        public void EventData_DefaultValuesAreCorrect()
        {
            // Arrange & Act
            var eventData = new EventData();

            // Assert
            Assert.Equal(0, eventData.OWPMatchId);
            Assert.Equal(0, eventData.AdminSiteId);
            Assert.Equal(string.Empty, eventData.Remark);
            Assert.Equal(DateTime.MinValue, eventData.EventDateTime);
        }
    }
}
