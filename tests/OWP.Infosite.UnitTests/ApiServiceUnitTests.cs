using Fanex.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using OWP.Infosite.Settings;
using System.Net;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.Infosite.UnitTests;

public class ApiServiceUnitTests
{
    private readonly IOptions<InfoSiteSettings> appSettings;

    private readonly HttpHandler httpHandler;

    private readonly ILogger logger;

    private const int expectedNumberOfMatchsAllDays = 1;

    private readonly DateTime fromDate = new DateTime(2018, 1, 2);

    private readonly DateTime toDate = new DateTime(2018, 1, 4);

    private readonly DateTime lastUpdatedTime = new DateTime(2018, 1, 1);

    public ApiServiceUnitTests()
    {
        appSettings = Options.Create(new InfoSiteSettings());
        httpHandler = Substitute.For<HttpHandler>();
        logger = Substitute.For<ILogger>();
    }

    [Fact]
    public async Task QueryAsync_HttpStatusCodeError_EmptyList()
    {
        // Arrange
        var param = new ParamsQueryModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            LastUpdatedTime = lastUpdatedTime
        };
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.QueryAsync(param);

        // Assert
        Assert.Equal(new List<CommonMatchModel>(), actualResult);
    }

    [Fact]
    public async Task QueryAsync_ParamsQueryModel_ListCommonMatchExs()
    {
        // Arrange
        var param = new ParamsQueryModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            LastUpdatedTime = lastUpdatedTime
        };

        var mockContent = Newtonsoft.Json.JsonConvert.SerializeObject(SampleDataTests.ListCommonMatches);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.QueryAsync(param);

        // Assert
        var actualNumberOfMatchs = actualResult.Count;
        Assert.Equal(expectedNumberOfMatchsAllDays, actualNumberOfMatchs);
    }

    [Fact]
    public async Task GetInvalidEventKeysByDateAsync_FromDate_ToDate_ListStringInfoSiteMatchId()
    {
        // Arrange

        var mockContent = Newtonsoft.Json.JsonConvert.SerializeObject(new List<string> { "1423405-5-False" });
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.GetInvalidEventKeysByDateAsync(fromDate);

        // Assert
        const int expectedResultNumberOfMatchIds = 1;
        var actualNumberOfMatchs = actualResult.Count;
        Assert.Equal(expectedResultNumberOfMatchIds, actualNumberOfMatchs);
    }

    [Fact]
    public async Task GetInvalidEventKeysByDateAsync_HttpStatusCodeError_EmptyList()
    {
        // Arrange

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Assert
        var actualResult = await apiService.GetInvalidEventKeysByDateAsync(fromDate);

        Assert.Equal(new List<string>(), actualResult);
    }

    [Fact]
    public async Task QueryInfoSiteMatchFor180Day_HttpStatusCodeError_EmptyList()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act

        // Assert
        var actualResult = await apiService.QueryInfoSiteMatchFor180Day();

        Assert.Equal(new List<MatchModelQuery180Day>(), actualResult);
    }

    [Fact]
    public async Task QueryInfoSiteMatchFor180Day_HttpStatusOK_ListMatchModelQuery180Day()
    {
        // Arrange
        var mockContent = Newtonsoft.Json.JsonConvert.SerializeObject(SampleDataTests.ListMatchModelQuery180Day);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.QueryInfoSiteMatchFor180Day();

        // Assert
        var actualNumberOfMatchs = actualResult.Count;
        Assert.Equal(expectedNumberOfMatchsAllDays, actualNumberOfMatchs);
    }

    [Fact]
    public async Task QueryMatchesRelation__HttpStatusOK_ListMatchRelation()
    {
        // Arrange
        var param = new ParamsQueryModel
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        var mockContent = Newtonsoft.Json.JsonConvert.SerializeObject(SampleDataTests.ListMatchRelation);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.QueryMatchesRelation(param);

        // Assert
        DeepAssert.Equal(SampleDataTests.ListMatchRelation, actualResult);
    }

    [Fact]
    public async Task QueryMatchesRelation__HttpStatusCodeError_ListMatchRelation()
    {
        // Arrange
        var param = new ParamsQueryModel
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.QueryMatchesRelation(param);

        // Assert
        DeepAssert.Equal(new List<MatchRelation>(), actualResult);
    }
}