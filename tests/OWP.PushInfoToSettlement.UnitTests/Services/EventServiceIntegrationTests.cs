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

        [Fact]
        public void EventServiceImp_ImplementsEventServiceInterface()
        {
            // Arrange
            var service = new EventServiceImp(null);

            // Assert
            Assert.IsAssignableFrom<EventService>(service);
        }

        [Fact]
        public void EventData_CanBeSerializedAndDeserialized()
        {
            // Arrange
            var originalEvent = new EventData
            {
                OWPMatchId = 123,
                AdminSiteId = 456,
                Remark = "Test remark for serialization",
                EventDateTime = DateTime.Today.AddHours(14)
            };

            // Act
            var json = System.Text.Json.JsonSerializer.Serialize(originalEvent);
            var deserializedEvent = System.Text.Json.JsonSerializer.Deserialize<EventData>(json);

            // Assert
            Assert.NotNull(deserializedEvent);
            Assert.Equal(originalEvent.OWPMatchId, deserializedEvent.OWPMatchId);
            Assert.Equal(originalEvent.AdminSiteId, deserializedEvent.AdminSiteId);
            Assert.Equal(originalEvent.Remark, deserializedEvent.Remark);
            Assert.Equal(originalEvent.EventDateTime, deserializedEvent.EventDateTime);
        }

        [Fact]
        public void EventData_WithNullRemark_HandlesCorrectly()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = null,
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Null(eventData.Remark);
        }

        [Fact]
        public void EventData_WithEmptyRemark_HandlesCorrectly()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "",
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal("", eventData.Remark);
        }

        [Fact]
        public void EventData_WithWhitespaceRemark_HandlesCorrectly()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "   ",
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal("   ", eventData.Remark);
        }

        [Fact]
        public void EventData_WithSpecialCharactersInRemark_HandlesCorrectly()
        {
            // Arrange
            var specialRemark = "Test remark with special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";
            var eventData = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = specialRemark,
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal(specialRemark, eventData.Remark);
        }

        [Fact]
        public void EventData_WithMinMaxDateTimeValues_HandlesCorrectly()
        {
            // Arrange
            var eventDataMin = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "Min date",
                EventDateTime = DateTime.MinValue
            };

            var eventDataMax = new EventData
            {
                OWPMatchId = 2,
                AdminSiteId = 101,
                Remark = "Max date",
                EventDateTime = DateTime.MaxValue
            };

            // Act & Assert
            Assert.Equal(DateTime.MinValue, eventDataMin.EventDateTime);
            Assert.Equal(DateTime.MaxValue, eventDataMax.EventDateTime);
        }

        [Fact]
        public void EventData_WithNegativeIds_HandlesCorrectly()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = -1,
                AdminSiteId = -100,
                Remark = "Negative IDs",
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal(-1, eventData.OWPMatchId);
            Assert.Equal(-100, eventData.AdminSiteId);
        }

        [Fact]
        public void EventData_WithZeroIds_HandlesCorrectly()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = 0,
                AdminSiteId = 0,
                Remark = "Zero IDs",
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal(0, eventData.OWPMatchId);
            Assert.Equal(0, eventData.AdminSiteId);
        }
    }
}
