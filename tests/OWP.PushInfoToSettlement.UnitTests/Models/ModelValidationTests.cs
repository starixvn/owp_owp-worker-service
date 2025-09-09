using OWP.PushInfoToSettlement.Model;
using Xunit;

namespace OWP.PushInfoToSettlement.UnitTests.Models
{
    public class ModelValidationTests
    {
        [Fact]
        public void EventData_DefaultConstructor_SetsCorrectDefaultValues()
        {
            // Act
            var eventData = new EventData();

            // Assert
            Assert.Equal(0, eventData.OWPMatchId);
            Assert.Equal(0, eventData.AdminSiteId);
            Assert.Equal(string.Empty, eventData.Remark);
            Assert.Equal(DateTime.MinValue, eventData.EventDateTime);
        }

        [Fact]
        public void EventData_PropertySetters_WorkCorrectly()
        {
            // Arrange
            var eventData = new EventData();
            var testDate = DateTime.Today.AddHours(14);

            // Act
            eventData.OWPMatchId = 123;
            eventData.AdminSiteId = 456;
            eventData.Remark = "Test remark";
            eventData.EventDateTime = testDate;

            // Assert
            Assert.Equal(123, eventData.OWPMatchId);
            Assert.Equal(456, eventData.AdminSiteId);
            Assert.Equal("Test remark", eventData.Remark);
            Assert.Equal(testDate, eventData.EventDateTime);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void EventData_OWPMatchId_AcceptsAllIntValues(int value)
        {
            // Arrange
            var eventData = new EventData();

            // Act
            eventData.OWPMatchId = value;

            // Assert
            Assert.Equal(value, eventData.OWPMatchId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void EventData_AdminSiteId_AcceptsAllIntValues(int value)
        {
            // Arrange
            var eventData = new EventData();

            // Act
            eventData.AdminSiteId = value;

            // Assert
            Assert.Equal(value, eventData.AdminSiteId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Test remark")]
        [InlineData("Very long remark that might contain special characters !@#$%^&*()_+-=[]{}|;':\",./<>?")]
        [InlineData("Remark with\nnewlines")]
        [InlineData("Remark with\ttabs")]
        public void EventData_Remark_AcceptsAllStringValues(string value)
        {
            // Arrange
            var eventData = new EventData();

            // Act
            eventData.Remark = value;

            // Assert
            Assert.Equal(value, eventData.Remark);
        }

        [Theory]
        [InlineData("2024-01-01T00:00:00")]
        [InlineData("2024-12-31T23:59:59")]
        [InlineData("2024-02-29T12:00:00")] // Leap year
        [InlineData("1900-01-01T00:00:00")]
        [InlineData("2100-12-31T23:59:59")]
        public void EventData_EventDateTime_AcceptsValidDateTimeValues(string dateTimeString)
        {
            // Arrange
            var eventData = new EventData();
            var testDateTime = DateTime.Parse(dateTimeString);

            // Act
            eventData.EventDateTime = testDateTime;

            // Assert
            Assert.Equal(testDateTime, eventData.EventDateTime);
        }

        [Fact]
        public void EventData_WithNullRemark_HandlesCorrectly()
        {
            // Arrange
            var eventData = new EventData();

            // Act
            eventData.Remark = null;

            // Assert
            Assert.Null(eventData.Remark);
        }

        [Fact]
        public void ChangeInstantSettingRequest_DefaultConstructor_SetsCorrectDefaultValues()
        {
            // Act
            var request = new ChangeInstantSettingRequest();

            // Assert
            Assert.Equal(0, request.MatchId);
            Assert.Equal(string.Empty, request.Remark);
        }

        [Fact]
        public void ChangeInstantSettingRequest_PropertySetters_WorkCorrectly()
        {
            // Arrange
            var request = new ChangeInstantSettingRequest();

            // Act
            request.MatchId = 789;
            request.Remark = "NO instant settlement";

            // Assert
            Assert.Equal(789, request.MatchId);
            Assert.Equal("NO instant settlement", request.Remark);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void ChangeInstantSettingRequest_MatchId_AcceptsAllIntValues(int value)
        {
            // Arrange
            var request = new ChangeInstantSettingRequest();

            // Act
            request.MatchId = value;

            // Assert
            Assert.Equal(value, request.MatchId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("NO instant settlement")]
        [InlineData("Instant settlement allowed")]
        [InlineData("Very long setting description that might contain special characters !@#$%^&*()_+-=[]{}|;':\",./<>?")]
        public void ChangeInstantSettingRequest_Remark_AcceptsAllStringValues(string value)
        {
            // Arrange
            var request = new ChangeInstantSettingRequest();

            // Act
            request.Remark = value;

            // Assert
            Assert.Equal(value, request.Remark);
        }

        [Fact]
        public void ChangeInstantSettingRequest_WithNullRemark_HandlesCorrectly()
        {
            // Arrange
            var request = new ChangeInstantSettingRequest();

            // Act
            request.Remark = null;

            // Assert
            Assert.Null(request.Remark);
        }

        [Fact]
        public void SettlementApiSetting_DefaultConstructor_SetsCorrectDefaultValues()
        {
            // Act
            var setting = new SettlementApiSetting();

            // Assert
            Assert.Equal(string.Empty, setting.Url);
        }

        [Fact]
        public void SettlementApiSetting_PropertySetter_WorksCorrectly()
        {
            // Arrange
            var setting = new SettlementApiSetting();

            // Act
            setting.Url = "https://api.settlement.com";

            // Assert
            Assert.Equal("https://api.settlement.com", setting.Url);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("https://api.settlement.com")]
        [InlineData("http://localhost:5000")]
        [InlineData("https://api.settlement.com/api/v1")]
        [InlineData("https://api.settlement.com:8080/api/v1/endpoint")]
        public void SettlementApiSetting_Url_AcceptsAllStringValues(string value)
        {
            // Arrange
            var setting = new SettlementApiSetting();

            // Act
            setting.Url = value;

            // Assert
            Assert.Equal(value, setting.Url);
        }

        [Fact]
        public void SettlementApiSetting_WithNullUrl_HandlesCorrectly()
        {
            // Arrange
            var setting = new SettlementApiSetting();

            // Act
            setting.Url = null;

            // Assert
            Assert.Null(setting.Url);
        }

        [Fact]
        public void EventData_Equality_WithSameValues_AreEqual()
        {
            // Arrange
            var eventData1 = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "Test remark",
                EventDateTime = DateTime.Today
            };

            var eventData2 = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "Test remark",
                EventDateTime = DateTime.Today
            };

            // Act & Assert
            Assert.Equal(eventData1.OWPMatchId, eventData2.OWPMatchId);
            Assert.Equal(eventData1.AdminSiteId, eventData2.AdminSiteId);
            Assert.Equal(eventData1.Remark, eventData2.Remark);
            Assert.Equal(eventData1.EventDateTime, eventData2.EventDateTime);
        }

        [Fact]
        public void ChangeInstantSettingRequest_Equality_WithSameValues_AreEqual()
        {
            // Arrange
            var request1 = new ChangeInstantSettingRequest
            {
                MatchId = 100,
                Remark = "NO instant settlement"
            };

            var request2 = new ChangeInstantSettingRequest
            {
                MatchId = 100,
                Remark = "NO instant settlement"
            };

            // Act & Assert
            Assert.Equal(request1.MatchId, request2.MatchId);
            Assert.Equal(request1.Remark, request2.Remark);
        }

        [Fact]
        public void EventData_ToString_ReturnsExpectedFormat()
        {
            // Arrange
            var eventData = new EventData
            {
                OWPMatchId = 1,
                AdminSiteId = 100,
                Remark = "Test remark",
                EventDateTime = new DateTime(2024, 1, 1, 12, 0, 0)
            };

            // Act
            var result = eventData.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("EventData", result);
        }

        [Fact]
        public void ChangeInstantSettingRequest_ToString_ReturnsExpectedFormat()
        {
            // Arrange
            var request = new ChangeInstantSettingRequest
            {
                MatchId = 100,
                Remark = "NO instant settlement"
            };

            // Act
            var result = request.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("ChangeInstantSettingRequest", result);
        }
    }
}
