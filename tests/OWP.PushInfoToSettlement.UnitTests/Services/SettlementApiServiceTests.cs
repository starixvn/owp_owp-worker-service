using Microsoft.Extensions.Configuration;
using NSubstitute;
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
        private readonly HttpHandler _mockHttpHandler;
        private readonly IConfiguration _mockConfiguration;
        private readonly ILogger _mockLogger;
        private readonly SettlementApiSetting _apiSetting;
        private readonly SettlementApiService _service;

        public SettlementApiServiceTests()
        {
            _mockHttpHandler = Substitute.For<HttpHandler>();
            _mockConfiguration = Substitute.For<IConfiguration>();
            _mockLogger = Substitute.For<ILogger>();
            _apiSetting = new SettlementApiSetting { Url = "https://api.test.com" };
            _service = new SettlementApiService(_mockHttpHandler, _apiSetting, _mockLogger);
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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.True(success);
            Assert.Equal("Success", message);
            _mockHttpHandler.Received(1).PostAsync(
                "https://api.test.com/api/match/change-instant-setting",
                Arg.Any<HttpContent>());
            _mockLogger.Received(1).InfoAsync(Arg.Any<string>());
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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Equal("Error", message);
            _mockHttpHandler.Received(1).PostAsync(
                "https://api.test.com/api/match/change-instant-setting",
                Arg.Any<HttpContent>());
            _mockLogger.Received(1).InfoAsync(Arg.Any<string>());
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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Contains("Unexpected character encountered while parsing value", message);
            _mockHttpHandler.Received(1).PostAsync(
                "https://api.test.com/api/match/change-instant-setting",
                Arg.Any<HttpContent>());
            _mockLogger.Received(1).InfoAsync(Arg.Any<string>());
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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Equal("HTTP Error: 400", message);
            _mockLogger.Received(1).ErrorAsync(Arg.Any<string>());
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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(Task.FromException<HttpResponseMessage>(new HttpRequestException("Network error")));

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.False(success);
            Assert.Equal("Exception: Network error", message);
            _mockLogger.Received(1).ErrorAsync(Arg.Any<string>());
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
            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(mockResponse)
                .AndDoes(callInfo => capturedContent = callInfo.Arg<HttpContent>());

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.True(success);
            Assert.Equal("Success", message);
            _mockHttpHandler.Received(1).PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());

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

            _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
                .Returns(mockResponse);

            // Act
            var (success, message) = await _service.ChangeInstantSettingAsync(matchItems, fromDate, toDate);

            // Assert
            Assert.True(success);
            Assert.Equal("Success", message);
            _mockLogger.Received(1).InfoAsync(Arg.Is<string>(log =>
                log.Contains("Push MatchInfo To Settlement fromDate:") &&
                log.Contains("toDate:")));
        }
    }
}
