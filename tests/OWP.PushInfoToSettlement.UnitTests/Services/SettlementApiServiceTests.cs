using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using OWP.Services;
using System.Net;
using System.Text;
using Fanex.Logging;
using Xunit;

namespace OWP.PushInfoToSettlement.UnitTests.Services
{
    public class SettlementApiServiceTests
    {
        private readonly Mock<HttpHandler> _mockHttpHandler;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger> _mockLogger;
        private readonly SettlementApiSetting _apiSetting;
        private readonly SettlementApiService _service;

        public SettlementApiServiceTests()
        {
            _mockHttpHandler = new Mock<HttpHandler>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger>();
            _apiSetting = new SettlementApiSetting { Url = "https://api.test.com" };
            _service = new SettlementApiService(_mockHttpHandler.Object, _apiSetting, _mockLogger.Object);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_WhenSuccess_ReturnsTrue()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>
            {
                new() { MatchId = 1, Remark = "NO instant settlement" }
            };
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"errorCode\": 0, \"errorMsg\": \"Success\", \"isFail\": false}", Encoding.UTF8, "application/json")
            };

            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.True(success);
            Assert.Equal("Success", message);
            _mockHttpHandler.Verify(x => x.PostAsync(
                "https://api.test.com/api/match/change-instant-setting123123", 
                It.IsAny<HttpContent>()), Times.Once);
            _mockLogger.Verify(x => x.InfoAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_WhenApiReturnsIsFailTrue_ReturnsFalse()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>
            {
                new() { MatchId = 1, Remark = "NO instant settlement" }
            };
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"errorCode\": 1, \"errorMsg\": \"Error\", \"isFail\": true}", Encoding.UTF8, "application/json")
            };

            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Equal("Error", message);
            _mockHttpHandler.Verify(x => x.PostAsync(
                "https://api.test.com/api/match/change-instant-setting123123", 
                It.IsAny<HttpContent>()), Times.Once);
            _mockLogger.Verify(x => x.InfoAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_WhenInvalidJsonResponse_ReturnsFalse()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>
            {
                new() { MatchId = 1, Remark = "NO instant settlement" }
            };
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
            };

            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Contains("Unexpected character encountered while parsing value", message);
            _mockHttpHandler.Verify(x => x.PostAsync(
                "https://api.test.com/api/match/change-instant-setting123123",
                It.IsAny<HttpContent>()), Times.Once);
            _mockLogger.Verify(x => x.InfoAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_WhenHttpError_ReturnsFalse()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>
            {
                new() { MatchId = 1, Remark = "NO instant settlement" }
            };
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request", Encoding.UTF8, "application/json")
            };

            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Equal("HTTP Error: 400", message);
            _mockLogger.Verify(x => x.ErrorAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_WhenException_ReturnsFalse()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>
            {
                new() { MatchId = 1, Remark = "NO instant settlement" }
            };
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Equal("Exception: Network error", message);
            _mockLogger.Verify(x => x.ErrorAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_SerializesCorrectJson()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>
            {
                new() { MatchId = 1, Remark = "NO instant settlement" },
                new() { MatchId = 2, Remark = "Instant settlement allowed" }
            };
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"errorCode\": 0, \"errorMsg\": \"Success\", \"isFail\": false}", Encoding.UTF8, "application/json")
            };

            HttpContent capturedContent = null;
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((uri, content) => capturedContent = content)
                .ReturnsAsync(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.True(success);
            Assert.Equal("Success", message);
            _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);

            Assert.NotNull(capturedContent);
            var contentString = await capturedContent.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<dynamic>(contentString);
            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.matchItems);
            Assert.Equal(2, deserialized.matchItems.Count);
        }

        [Fact]
        public async Task ChangeInstantSettingAsync_LogsCorrectJobName()
        {
            // Arrange
            var matchItems = new List<ChangeInstantSettingRequest>();
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"errorCode\": 0, \"errorMsg\": \"Success\", \"isFail\": false}", Encoding.UTF8, "application/json")
            };

            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.True(success);
            Assert.Equal("Success", message);
            _mockLogger.Verify(x => x.InfoAsync(It.Is<string>(log =>
                log.Contains("Push MatchInfo To Settlement fromDate:") &&
                log.Contains("toDate:"))), Times.Once);
        }
    }
}
