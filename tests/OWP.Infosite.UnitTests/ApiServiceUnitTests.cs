using OWP.Infosite.Services;

namespace OWP.Infosite.UnitTests;

using Fanex.Caching;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using NSubstitute;
using OWP.Services;
using RabbitMq;
using Services;
using Settings;
using System.Net;
using Xunit;
using Xunit.Asserts.Compare;

public class ApiServiceUnitTests
{
    private readonly IOptions<InfoSiteSettings> appSettings;

    private readonly HttpHandler httpHandler;

    private readonly ILogger logger;

    private const int ExpectedNumberOfMatchesAllDays = 1;

    private readonly DateTime fromDate = new(2018, 1, 2);

    private readonly DateTime toDate = new(2018, 1, 4);

    private readonly DateTime lastUpdatedTime = new(2018, 1, 1);

    public ApiServiceUnitTests()
    {
        appSettings = Options.Create(new InfoSiteSettings());
        httpHandler = Substitute.For<HttpHandler>();
        logger = Substitute.For<ILogger>();
        Substitute.For<RabbitMqClient>();
        Substitute.For<ICache>();
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
        Assert.Equal([], actualResult);
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

        var mockContent = JsonConvert.SerializeObject(SampleDataTests.ListCommonMatches);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.QueryAsync(param);

        // Assert
        var actualNumberOfMatches = actualResult.Count;
        Assert.Equal(ExpectedNumberOfMatchesAllDays, actualNumberOfMatches);
    }

    [Fact]
    public async Task GetInvalidEventKeysByDateAsync_FromDate_ToDate_ListStringInfoSiteMatchId()
    {
        // Arrange

        var mockContent = JsonConvert.SerializeObject(new List<string> { "1423405-5-False" });
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.GetInvalidEventKeysByDateAsync(fromDate);

        // Assert
        const int expectedResultNumberOfMatchIds = 1;
        var actualNumberOfMatches = actualResult.Count;
        Assert.Equal(expectedResultNumberOfMatchIds, actualNumberOfMatches);
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

        Assert.Equal([], actualResult);
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

        Assert.Equal([], actualResult);
    }

    [Fact]
    public async Task QueryInfoSiteMatchFor180Day_HttpStatusOK_ListMatchModelQuery180Day()
    {
        // Arrange
        var mockContent = JsonConvert.SerializeObject(SampleDataTests.ListMatchModelQuery180Day);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);
        // Act
        var actualResult = await apiService.QueryInfoSiteMatchFor180Day();

        // Assert
        var actualNumberOfMatches = actualResult.Count;
        Assert.Equal(ExpectedNumberOfMatchesAllDays, actualNumberOfMatches);
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
        var mockContent = JsonConvert.SerializeObject(SampleDataTests.ListMatchRelation);
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
        DeepAssert.Equal([], actualResult);
    }

    [Fact]
    public async Task QueryDeletedChildMatch_HttpStatusOK_ListDeleteChildMatch()
    {
        // Arrange
        var fromDate = new DateTime(2018, 1, 2);
        var toDate = new DateTime(2018, 1, 4);
        var mockContent = JsonConvert.SerializeObject(SampleDataTests.ListDeleteChildMatch);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.QueryDeletedChildMatch(fromDate, toDate);

        // Assert
        DeepAssert.Equal<List<DeleteChildMatch>>(SampleDataTests.ListDeleteChildMatch, actualResult);
    }

    [Fact]
    public async Task QueryDeletedChildMatch_HttpStatusCodeError_EmptyList()
    {
        // Arrange
        var fromDate = new DateTime(2018, 1, 2);
        var toDate = new DateTime(2018, 1, 4);
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.QueryDeletedChildMatch(fromDate, toDate);

        // Assert
        Assert.Equal([], actualResult);
    }

    [Fact]
    public async Task QueryRound_HttpStatusOK_RoundTwInfo()
    {
        // Arrange
        var fromDate = new DateTime(2018, 1, 2);
        var toDate = new DateTime(2018, 1, 4);
        var mockContent = JsonConvert.SerializeObject(SampleDataTests.RoundTwInfo);
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.QueryRound(fromDate, toDate);

        // Assert
        DeepAssert.Equal<RoundTwInfo>(SampleDataTests.RoundTwInfo, actualResult);
    }

    [Fact]
    public async Task QueryRound_HttpStatusCodeError_EmptyRoundTwInfo()
    {
        // Arrange
        var fromDate = new DateTime(2018, 1, 2);
        var toDate = new DateTime(2018, 1, 4);
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        httpHandler.GetAsync(Arg.Compat.Any<string>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.QueryRound(fromDate, toDate);

        // Assert
        Assert.NotNull(actualResult);
        Assert.False(actualResult.finish);
        Assert.False(actualResult.status);
        Assert.Null(actualResult.message);
        Assert.Null(actualResult.content);
    }

    [Fact]
    public async Task SetLiveMatches_HttpStatusOK_IsLiveSynchronizedResult()
    {
        // Arrange
        var liveMatches = new List<(int, int?)> { (123, 1), (456, null) };
        var mockContent = "[]";
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.PostAsync(Arg.Compat.Any<string>(), Arg.Compat.Any<HttpContent>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.SetLiveMatches(liveMatches);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(liveMatches, actualResult.SuccessfulMatches);
        Assert.Empty(actualResult.FailedMatches);
        Assert.Empty(actualResult.ErrorMessage);
    }

    [Fact]
    public async Task SetLiveMatches_HttpStatusCodeError_IsLiveSynchronizedResult()
    {
        // Arrange
        var liveMatches = new List<(int, int?)> { (123, 1), (456, null) };
        var mockContent = "Error message";
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.BadRequest };
        httpHandler.PostAsync(Arg.Compat.Any<string>(), Arg.Compat.Any<HttpContent>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.SetLiveMatches(liveMatches);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(liveMatches, actualResult.FailedMatches);
        Assert.Empty(actualResult.SuccessfulMatches);
        Assert.Equal("Error message", actualResult.ErrorMessage);
    }

    [Fact]
    public async Task SetNonLiveMatches_HttpStatusOK_IsLiveSynchronizedResult()
    {
        // Arrange
        var nonLiveMatches = new List<(int, int?)> { (123, 1), (456, null) };
        var mockContent = "[]";
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.OK };
        httpHandler.PostAsync(Arg.Compat.Any<string>(), Arg.Compat.Any<HttpContent>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.SetNonLiveMatches(nonLiveMatches);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(nonLiveMatches, actualResult.SuccessfulMatches);
        Assert.Empty(actualResult.FailedMatches);
        Assert.Empty(actualResult.ErrorMessage);
    }

    [Fact]
    public async Task SetNonLiveMatches_HttpStatusCodeError_IsLiveSynchronizedResult()
    {
        // Arrange
        var nonLiveMatches = new List<(int, int?)> { (123, 1), (456, null) };
        var mockContent = "Error message";
        var httpContent = new StringContent(mockContent);

        var httpResponseMessage = new HttpResponseMessage { Content = httpContent, StatusCode = HttpStatusCode.BadRequest };
        httpHandler.PostAsync(Arg.Compat.Any<string>(), Arg.Compat.Any<HttpContent>()).Returns(httpResponseMessage);

        var apiService = new ApiServiceImp(httpHandler, logger, appSettings);

        // Act
        var actualResult = await apiService.SetNonLiveMatches(nonLiveMatches);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(nonLiveMatches, actualResult.FailedMatches);
        Assert.Empty(actualResult.SuccessfulMatches);
        Assert.Equal("Error message", actualResult.ErrorMessage);
    }

    [Fact]
    public void ParseSubtype_WithValidString_ShouldReturnCorrectValue()
    {
        // Arrange
        var result = "subType 5 from";

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Equal(5, actualResult);
    }

    [Fact]
    public void ParseSubtype_WithEmptyString_ShouldReturnNull()
    {
        // Arrange
        var result = "";

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void ParseSubtype_WithNullString_ShouldReturnNull()
    {
        // Arrange
        string result = null;

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void ParseSubtype_WithInvalidFormat_ShouldReturnNull()
    {
        // Arrange
        var result = "invalid format";

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void ParseSubtype_WithMissingFromText_ShouldReturnNull()
    {
        // Arrange
        var result = "subType 5";

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void ParseSubtype_WithMissingSubtypeText_ShouldReturnNull()
    {
        // Arrange
        var result = "5 from";

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void ParseSubtype_WithWhitespaceSubtype_ShouldReturnNull()
    {
        // Arrange
        var result = "subType   from";

        // Act
        var actualResult = ApiServiceImp.ParseSubtype(result);

        // Assert
        Assert.Null(actualResult);
    }
}