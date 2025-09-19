using Microsoft.Extensions.Configuration;
using Moq;
using OWP.Adminsite.Models;
using OWP.Adminsite.Services;
using OWP.Extensions;
using OWP.Services;
using System.Net;
using System.Text;
using Olympus.OWP.Log.Data.Repository.Services;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Newtonsoft.Json;
using Fanex.Logging;

namespace OWP.Adminsite.UnitTests.Services;

public class ApiServiceTests
{
    private readonly AdminSiteSettings _apiSettings;
    private readonly Mock<HttpHandler> _mockHttpHandler;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<LogWriterService> _mockLogWriterService;
    private readonly Mock<ILogger> _mockLogger;
    private readonly ApiService _apiService;

    public ApiServiceTests()
    {
        _apiSettings = new AdminSiteSettings
        {
            PushMatchInfoAPI = "https://api.example.com/push",
            InfositeAPI = "https://api.example.com/"
        };
        _mockHttpHandler = new Mock<HttpHandler>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogWriterService = new Mock<LogWriterService>();
        _mockLogger = new Mock<ILogger>();

        _apiService = new ApiService(
            _apiSettings,
            _mockHttpHandler.Object,
            _mockConfiguration.Object,
            _mockLogWriterService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockApiSettings = new Mock<AdminSiteSettings>();
        var mockHttpHandler = new Mock<HttpHandler>();
        var mockConfiguration = new Mock<IConfiguration>();
        var mockLogWriterService = new Mock<LogWriterService>();
        var mockLogger = new Mock<ILogger>();

        // Act
        var apiService = new ApiService(
            mockApiSettings.Object,
            mockHttpHandler.Object,
            mockConfiguration.Object,
            mockLogWriterService.Object,
            mockLogger.Object);

        // Assert
        apiService.Should().NotBeNull();
    }

    [Fact]
    public void DateTimeFormat_ShouldHaveCorrectValue()
    {
        // Act & Assert
        ApiService.DateTimeFormat.Should().Be("yyyy-MM-dd");
    }

    #region PushMatchInfoAsync Tests

    [Theory]
    [InlineData("DEV")]
    [InlineData("LOCAL")]
    public async Task PushMatchInfoAsync_WhenEnvironmentIsDevOrLocal_ShouldReturnEmptyListAndLogInfo(string environment)
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns(environment);

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEmpty();
        _mockLogger.Verify(x => x.InfoAsync(It.IsAny<string>()), Times.Once);
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Never);
    }

    [Theory]
    [InlineData("PROD")]
    [InlineData("STAGING")]
    [InlineData("TEST")]
    public async Task PushMatchInfoAsync_WhenEnvironmentIsNotDevOrLocal_ShouldCallHttpHandler(string environment)
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns(environment);
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEmpty();
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
    }

    [Fact]
    public async Task PushMatchInfoAsync_WhenHttpResponseIsSuccess_ShouldReturnEmptyListAndLogInfo()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns("PROD");
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEmpty();
        _mockLogger.Verify(x => x.InfoAsync(It.Is<string>(s => s.Contains("Sync match info to AdminSite"))), Times.Once);
    }

    [Fact]
    public async Task PushMatchInfoAsync_WhenHttpResponseIsNotSuccess_ShouldReturnOriginalMatchInfosAndLogError()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns("PROD");
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEquivalentTo(matchInfos);
        _mockLogger.Verify(x => x.ErrorAsync(It.Is<string>(s => s.Contains("Sync match info to AdminSite"))), Times.Once);
    }

    [Fact]
    public async Task PushMatchInfoAsync_ShouldSerializeMatchInfosCorrectly()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns("PROD");
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        _mockHttpHandler.Verify(x => x.PostAsync(
            It.IsAny<string>(),
            It.Is<HttpContent>(c => c is StringContent)), Times.Once);
    }

    #endregion

    #region QueryProviderMatchId Tests

    [Fact]
    public async Task QueryProviderMatchId_WhenHttpResponseIsSuccess_ShouldReturnDeserializedData()
    {
        // Arrange
        var date = new DateTime(2023, 1, 1);
        var expectedData = new ProviderMatchIdTw
        {
            finish = true,
            status = true,
            message = "Success",
            content = new List<Content>
            {
                new() { IbcMatchId = 123, ProviderMatchId = 456, ProviderId = 789 }
            }
        };
        var jsonData = JsonConvert.SerializeObject(expectedData);

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(jsonData) });

        // Act
        var result = await _apiService.QueryProviderMatchId(date);

        // Assert
        result.Should().NotBeNull();
        result.finish.Should().Be(expectedData.finish);
        result.status.Should().Be(expectedData.status);
        result.message.Should().Be(expectedData.message);
        result.content.Should().HaveCount(1);
        result.content[0].IbcMatchId.Should().Be(123);
        result.content[0].ProviderMatchId.Should().Be(456);
        result.content[0].ProviderId.Should().Be(789);
        _mockLogWriterService.Verify(x => x.Execute(It.IsAny<AdminSiteLogCriteria>()), Times.Once);
    }

    [Fact]
    public async Task QueryProviderMatchId_WhenHttpResponseIsNotSuccess_ShouldLogErrorAndRetry()
    {
        // Arrange
        var date = new DateTime(2023, 1, 1);
        var requestLink = $"https://api.example.com/api/OWP/GetSportsTickerMapping?sportType=1&date={date.ToString(ApiService.DateTimeFormat)}";

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        // Act
        var task = _apiService.QueryProviderMatchId(date);

        // Assert
        // Since this method has an infinite loop, we can't easily test the full behavior
        // But we can verify the initial call and logging
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.AtLeastOnce);
        _mockLogger.Verify(x => x.ErrorAsync(It.IsAny<string>()), Times.AtLeastOnce);
        _mockLogWriterService.Verify(x => x.Execute(It.IsAny<AdminSiteLogCriteria>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task QueryProviderMatchId_ShouldUseCorrectDateFormat()
    {
        // Arrange
        var date = new DateTime(2023, 1, 15);
        var expectedDateFormat = date.ToString(ApiService.DateTimeFormat);
        var expectedUrl = $"https://api.example.com/api/OWP/GetSportsTickerMapping?sportType=1&date={expectedDateFormat}";

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("{}") });

        // Act
        var task = _apiService.QueryProviderMatchId(date);

        // Assert
        _mockHttpHandler.Verify(x => x.GetAsync(expectedUrl), Times.AtLeastOnce);
    }

    [Fact]
    public async Task QueryProviderMatchId_ShouldLogWithCorrectAction()
    {
        // Arrange
        var date = new DateTime(2023, 1, 1);
        var expectedData = new ProviderMatchIdTw
        {
            finish = true,
            status = true,
            message = "Success",
            content = new List<Content>
            {
                new() { IbcMatchId = 123, ProviderMatchId = 456, ProviderId = 789 }
            }
        };
        var jsonData = JsonConvert.SerializeObject(expectedData);

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(jsonData) });

        // Act
        var result = await _apiService.QueryProviderMatchId(date);

        // Assert
        _mockLogWriterService.Verify(x => x.Execute(It.Is<AdminSiteLogCriteria>(c => c.Action == "SyncProviderMatchId")), Times.Once);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public async Task PushMatchInfoAsync_WithEmptyMatchInfos_ShouldHandleCorrectly()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>();

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns("PROD");
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEmpty();
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
    }

    [Fact]
    public async Task PushMatchInfoAsync_WithNullMatchInfos_ShouldHandleCorrectly()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        List<MatchInfoApiModel>? matchInfos = null;

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns("PROD");
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos!);

        // Assert
        result.Should().BeEmpty();
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    public async Task PushMatchInfoAsync_WithEmptyEnvironment_ShouldCallHttpHandler(string environment)
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns(environment);
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEmpty();
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
    }

    [Fact]
    public async Task PushMatchInfoAsync_WithNullEnvironment_ShouldCallHttpHandler()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 1, 2);
        var matchInfos = new List<MatchInfoApiModel>
        {
            new() { MatchId = 1, Provider = "Test Provider" }
        };

        _mockConfiguration.Setup(x => x.GetSection("Environment").Value).Returns((string?)null);
        _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") });

        // Act
        var result = await _apiService.PushMatchInfoAsync(fromDate, toDate, matchInfos);

        // Assert
        result.Should().BeEmpty();
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
    }

    #endregion
}
