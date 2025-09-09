using Moq;
using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using Fanex.Caching;
using Xunit;
using Newtonsoft.Json;

namespace OWP.PushInfoToSettlement.UnitTests.Services
{
    public class CacheManipulatorTests
    {
        private readonly Mock<ICache> _mockCache;
        private readonly CacheManipulatorImp _service;

        public CacheManipulatorTests()
        {
            _mockCache = new Mock<ICache>();
            _service = new CacheManipulatorImp(_mockCache.Object);
        }

        private List<EventData> CreateSampleEvents()
        {
            return new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Test remark 1", EventDateTime = DateTime.Today.AddHours(14) },
                new() { OWPMatchId = 2, AdminSiteId = 101, Remark = "Test remark 2", EventDateTime = DateTime.Today.AddHours(16) }
            };
        }

        [Fact]
        public async Task SaveMatchSnapshot_WithValidData_SavesToCache()
        {
            // Arrange
            var events = CreateSampleEvents();
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";
            var expectedJson = JsonConvert.SerializeObject(events);

            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CacheItemOptions>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveMatchSnapshot(events, eventDate);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                expectedCacheKey,
                expectedJson,
                It.IsAny<CacheItemOptions>()), Times.Once);
        }

        [Fact]
        public async Task SaveMatchSnapshot_WithEmptyList_SavesToCache()
        {
            // Arrange
            var events = new List<EventData>();
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";

            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CacheItemOptions>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveMatchSnapshot(events, eventDate);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                expectedCacheKey,
                "[]",
                It.IsAny<CacheItemOptions>()), Times.Once);
        }

        [Fact]
        public async Task SaveMatchSnapshot_WithNullEvents_SavesToCache()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";

            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CacheItemOptions>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveMatchSnapshot(null, eventDate);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                expectedCacheKey,
                "null",
                It.IsAny<CacheItemOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetMatchesByDate_WhenCacheHasData_ReturnsDeserializedEvents()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";
            var events = CreateSampleEvents();
            var eventsJson = JsonConvert.SerializeObject(events);

            _mockCache.Setup(x => x.TryGetAsync<string>(expectedCacheKey))
                .ReturnsAsync((true, eventsJson));

            // Act
            var result = await _service.GetMatchesByDate(eventDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].OWPMatchId);
            Assert.Equal(100, result[0].AdminSiteId);
            Assert.Equal("Test remark 1", result[0].Remark);
            Assert.Equal(2, result[1].OWPMatchId);
            Assert.Equal(101, result[1].AdminSiteId);
            Assert.Equal("Test remark 2", result[1].Remark);
        }

        [Fact]
        public async Task GetMatchesByDate_WhenCacheIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";

            _mockCache.Setup(x => x.TryGetAsync<string>(expectedCacheKey))
                .ReturnsAsync((false, string.Empty));

            // Act
            var result = await _service.GetMatchesByDate(eventDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMatchesByDate_WhenCacheHasNullValue_ReturnsEmptyList()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";

            _mockCache.Setup(x => x.TryGetAsync<string>(expectedCacheKey))
                .ReturnsAsync((true, null));

            // Act
            var result = await _service.GetMatchesByDate(eventDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMatchesByDate_WhenCacheHasInvalidJson_ThrowsException()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var expectedCacheKey = $"Match-OldSnapshot-SendToSettlement-{eventDate:d}";

            _mockCache.Setup(x => x.TryGetAsync<string>(expectedCacheKey))
                .ReturnsAsync((true, "invalid json"));

            // Act & Assert
            await Assert.ThrowsAsync<Newtonsoft.Json.JsonReaderException>(() =>
                _service.GetMatchesByDate(eventDate));
        }

        [Fact]
        public async Task GetLastProcessedDate_WhenCacheHasData_ReturnsParsedDate()
        {
            // Arrange
            var expectedDate = DateTime.Today;
            var dateString = expectedDate.ToString("yyyy-MM-dd");

            _mockCache.Setup(x => x.TryGetAsync<string>("LastProcessedDate-SendToSettlement"))
                .ReturnsAsync((true, dateString));

            // Act
            var result = await _service.GetLastProcessedDate();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDate.Date, result.Value.Date);
        }

        [Fact]
        public async Task GetLastProcessedDate_WhenCacheIsEmpty_ReturnsNull()
        {
            // Arrange
            _mockCache.Setup(x => x.TryGetAsync<string>("LastProcessedDate-SendToSettlement"))
                .ReturnsAsync((false, string.Empty));

            // Act
            var result = await _service.GetLastProcessedDate();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLastProcessedDate_WhenCacheHasNullValue_ReturnsNull()
        {
            // Arrange
            _mockCache.Setup(x => x.TryGetAsync<string>("LastProcessedDate-SendToSettlement"))
                .ReturnsAsync((true, null));

            // Act
            var result = await _service.GetLastProcessedDate();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLastProcessedDate_WhenCacheHasInvalidDate_ThrowsException()
        {
            // Arrange
            _mockCache.Setup(x => x.TryGetAsync<string>("LastProcessedDate-SendToSettlement"))
                .ReturnsAsync((true, "invalid-date"));

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(() =>
                _service.GetLastProcessedDate());
        }

        [Fact]
        public async Task SaveLastProcessedDate_WithValidDate_SavesToCache()
        {
            // Arrange
            var date = DateTime.Today;
            var expectedDateString = date.ToString("yyyy-MM-dd");

            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CacheItemOptions>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveLastProcessedDate(date);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                "LastProcessedDate-SendToSettlement",
                expectedDateString,
                It.IsAny<CacheItemOptions>()), Times.Once);
        }

        [Fact]
        public async Task SaveLastProcessedDate_WithDifferentDateFormats_SavesCorrectly()
        {
            // Arrange
            var date = new DateTime(2024, 12, 25, 15, 30, 45); // Time should be ignored
            var expectedDateString = "2024-12-25";

            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CacheItemOptions>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveLastProcessedDate(date);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                "LastProcessedDate-SendToSettlement",
                expectedDateString,
                It.IsAny<CacheItemOptions>()), Times.Once);
        }

        [Theory]
        [InlineData("2024-01-01")]
        [InlineData("2024-12-31")]
        [InlineData("2024-02-29")] // Leap year
        public async Task GetLastProcessedDate_WithValidDateStrings_ReturnsCorrectDate(string dateString)
        {
            // Arrange
            var expectedDate = DateTime.Parse(dateString);

            _mockCache.Setup(x => x.TryGetAsync<string>("LastProcessedDate-SendToSettlement"))
                .ReturnsAsync((true, dateString));

            // Act
            var result = await _service.GetLastProcessedDate();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDate.Date, result.Value.Date);
        }

        [Fact]
        public async Task SaveMatchSnapshot_UsesCorrectCacheKeyFormat()
        {
            // Arrange
            var events = CreateSampleEvents();
            var eventDate = new DateTime(2024, 3, 15);
            var expectedCacheKey = "Match-OldSnapshot-SendToSettlement-3/15/2024";

            _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CacheItemOptions>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveMatchSnapshot(events, eventDate);

            // Assert
            _mockCache.Verify(x => x.SetAsync(
                expectedCacheKey,
                It.IsAny<string>(),
                It.IsAny<CacheItemOptions>()), Times.Once);
        }

        [Fact]
        public async Task GetMatchesByDate_UsesCorrectCacheKeyFormat()
        {
            // Arrange
            var eventDate = new DateTime(2024, 3, 15);
            var expectedCacheKey = "Match-OldSnapshot-SendToSettlement-3/15/2024";

            _mockCache.Setup(x => x.TryGetAsync<string>(expectedCacheKey))
                .ReturnsAsync((false, string.Empty));

            // Act
            await _service.GetMatchesByDate(eventDate);

            // Assert
            _mockCache.Verify(x => x.TryGetAsync<string>(expectedCacheKey), Times.Once);
        }
    }
}
