using Fanex.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using OWP.Infosite.Settings;
using OWP.OtherSport.SyncLive.Model;
using OWP.OtherSport.SyncLive.Services;
using OWP.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace OWP.OtherSport.UnitTests.SyncLive.Services;

public class ApiServiceTests
{
    private readonly IOptions<InfoSiteSettings> _mockInfoSiteSettings;
    private readonly ILogger _mockLogger;
    private readonly HttpHandler _mockHttpHandler;
    private readonly ApiServiceImp _apiService;

    public ApiServiceTests()
    {
        _mockInfoSiteSettings = Substitute.For<IOptions<InfoSiteSettings>>();
        _mockLogger = Substitute.For<ILogger>();
        _mockHttpHandler = Substitute.For<HttpHandler>();

        var settings = new InfoSiteSettings
        {
            InfositeOsApi = "https://api.example.com/"
        };
        _mockInfoSiteSettings.Value.Returns(settings);

        _apiService = new ApiServiceImp(_mockInfoSiteSettings, _mockLogger, _mockHttpHandler);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockInfoSiteSettings = Substitute.For<IOptions<InfoSiteSettings>>();
        var mockLogger = Substitute.For<ILogger>();
        var mockHttpHandler = Substitute.For<HttpHandler>();

        var settings = new InfoSiteSettings
        {
            InfositeOsApi = "https://api.example.com/"
        };
        mockInfoSiteSettings.Value.Returns(settings);

        // Act
        var apiService = new ApiServiceImp(mockInfoSiteSettings, mockLogger, mockHttpHandler);

        // Assert
        Assert.NotNull(apiService);
    }

    [Fact]
    public async Task PostSyncLiveAsync_WithValidBody_ShouldCallHttpHandler()
    {
        // Arrange
        var body = new BodyModel
        {
            live_configs = new List<EditEvent>
            {
                new() { in_house_match_id = 123, is_live = true }
            }
        };

        _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Success") }));

        // Act
        await _apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockHttpHandler.Received(1).PostAsync(
            "https://api.example.com/api/EventSchedule/SetEventLiveByInHouseMatchId",
            Arg.Any<HttpContent>());
    }

    [Fact]
    public async Task PostSyncLiveAsync_WithValidBody_ShouldLogInfo()
    {
        // Arrange
        var body = new BodyModel
        {
            live_configs = new List<EditEvent>
            {
                new() { in_house_match_id = 123, is_live = true }
            }
        };

        _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Success") }));

        // Act
        await _apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockLogger.Received(1).InfoAsync(Arg.Is<string>(s => s.Contains("OSSyncLiveToInfosite")));
    }

    [Fact]
    public async Task PostSyncLiveAsync_WhenHttpHandlerThrowsException_ShouldLogError()
    {
        // Arrange
        var body = new BodyModel
        {
            live_configs = new List<EditEvent>
            {
                new() { in_house_match_id = 123, is_live = true }
            }
        };

        _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns(Task.FromException<HttpResponseMessage>(new HttpRequestException("Network error")));

        // Act
        await _apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockLogger.Received(1).ErrorAsync(Arg.Is<string>(s => s.Contains("OSSyncLiveToInfosite")));
    }

    [Fact]
    public async Task PostSyncLiveAsync_WhenInfositeDomainIsEmpty_ShouldReturnEarly()
    {
        // Arrange
        var settings = new InfoSiteSettings
        {
            InfositeOsApi = ""
        };
        _mockInfoSiteSettings.Value.Returns(settings);
        var apiService = new ApiServiceImp(_mockInfoSiteSettings, _mockLogger, _mockHttpHandler);

        var body = new BodyModel
        {
            live_configs = new List<EditEvent>
            {
                new() { in_house_match_id = 123, is_live = true }
            }
        };

        // Act
        await apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockHttpHandler.DidNotReceive().PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
    }

    [Fact]
    public async Task PostSyncLiveAsync_WhenInfositeDomainIsNull_ShouldReturnEarly()
    {
        // Arrange
        var settings = new InfoSiteSettings
        {
            InfositeOsApi = null
        };
        _mockInfoSiteSettings.Value.Returns(settings);
        var apiService = new ApiServiceImp(_mockInfoSiteSettings, _mockLogger, _mockHttpHandler);

        var body = new BodyModel
        {
            live_configs = new List<EditEvent>
            {
                new() { in_house_match_id = 123, is_live = true }
            }
        };

        // Act
        await apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockHttpHandler.DidNotReceive().PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
    }

    [Fact]
    public async Task PostSyncLiveAsync_WithEmptyBody_ShouldHandleCorrectly()
    {
        // Arrange
        var body = new BodyModel
        {
            live_configs = new List<EditEvent>()
        };

        _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Success") }));

        // Act
        await _apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockHttpHandler.Received(1).PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
        await _mockLogger.Received(1).InfoAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task PostSyncLiveAsync_WithNullBody_ShouldHandleCorrectly()
    {
        // Arrange
        BodyModel? body = null;

        _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns(Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Success") }));

        // Act
        await _apiService.PostSyncLiveAsync(body!);

        // Assert
        await _mockHttpHandler.Received(1).PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>());
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, "Success")]
    [InlineData(HttpStatusCode.BadRequest, "Bad Request")]
    [InlineData(HttpStatusCode.InternalServerError, "Server Error")]
    public async Task PostSyncLiveAsync_WithDifferentHttpStatusCodes_ShouldLogCorrectly(HttpStatusCode statusCode, string responseContent)
    {
        // Arrange
        var body = new BodyModel
        {
            live_configs = new List<EditEvent>
            {
                new() { in_house_match_id = 123, is_live = true }
            }
        };

        _mockHttpHandler.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage { StatusCode = statusCode, Content = new StringContent(responseContent) });

        // Act
        await _apiService.PostSyncLiveAsync(body);

        // Assert
        await _mockLogger.Received(1).InfoAsync(Arg.Any<string>());
    }
}