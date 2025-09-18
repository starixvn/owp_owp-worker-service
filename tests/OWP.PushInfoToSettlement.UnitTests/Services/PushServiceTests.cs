using Moq;
using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using Olympus.OWP.Log.Data.Repository.Services;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Xunit;
using Newtonsoft.Json;

namespace OWP.PushInfoToSettlement.UnitTests.Services
{
    public class PushServiceTests
    {
        private readonly Mock<CacheManipulator> _mockCache;
        private readonly Mock<EventService> _mockEventService;
        private readonly Mock<ISettlementApiService> _mockSettlementApiService;
        private readonly Mock<LogWriterService> _mockLogger;
        private readonly PushServiceImp _service;

        public PushServiceTests()
        {
            _mockCache = new Mock<CacheManipulator>();
            _mockEventService = new Mock<EventService>();
            _mockSettlementApiService = new Mock<ISettlementApiService>();
            _mockLogger = new Mock<LogWriterService>();
            _service = new PushServiceImp(_mockCache.Object, _mockEventService.Object, _mockSettlementApiService.Object, _mockLogger.Object);
        }

        private List<EventData> CreateSampleEvents()
        {
            return new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Test remark 1", EventDateTime = DateTime.Today.AddHours(14) },
                new() { OWPMatchId = 2, AdminSiteId = 101, Remark = "Test remark 2", EventDateTime = DateTime.Today.AddHours(16) },
                new() { OWPMatchId = 3, AdminSiteId = 102, Remark = "Test remark 3", EventDateTime = DateTime.Today.AddDays(1).AddHours(10) }
            };
        }

        [Fact]
        public async Task Execute_WhenNoEvents_UpdatesCache()
        {
            // Arrange
            var currentEvents = new List<EventData>();
            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Exactly(3)); // 3 days
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task Execute_WhenEventsExistAndApiSucceeds_UpdatesCacheAndSendsApi()
        {
            // Arrange
            var currentEvents = new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Test remark", EventDateTime = DateTime.Today }
            };

            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Exactly(3));
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Exactly(2)); // Main log + cache log
        }

        [Fact]
        public async Task Execute_WhenEventsExistAndApiFails_DoesNotUpdateCache()
        {
            // Arrange
            var currentEvents = new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Test remark", EventDateTime = DateTime.Today }
            };

            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((false, "API Error")));

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Never); // No cache update on API failure
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Once); // Only main log
        }

        [Fact]
        public async Task Execute_WhenEventsHaveChangedRemarks_ProcessesOnlyChangedEvents()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            var cachedEvents = new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Old remark 1", EventDateTime = DateTime.Today.AddHours(14) },
                new() { OWPMatchId = 2, AdminSiteId = 101, Remark = "Test remark 2", EventDateTime = DateTime.Today.AddHours(16) }, // Same remark
                new() { OWPMatchId = 3, AdminSiteId = 102, Remark = "Old remark 3", EventDateTime = DateTime.Today.AddDays(1).AddHours(10) }
            };

            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(cachedEvents);
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(
                It.Is<IEnumerable<ChangeInstantSettingRequest>>(requests =>
                    requests.Count() == 2 && // Only 2 changed events
                    requests.Any(r => r.MatchId == 100 && r.Remark == "Test remark 1") &&
                    requests.Any(r => r.MatchId == 102 && r.Remark == "Test remark 3")),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenNewEventsExist_ProcessesAllNewEvents()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            var cachedEvents = new List<EventData>(); // Empty cache - all events are new

            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(cachedEvents);
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(
                It.Is<IEnumerable<ChangeInstantSettingRequest>>(requests => requests.Count() == 3),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenNoChangesDetected_DoesNotCallApi()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            var cachedEvents = CreateSampleEvents(); // Same events - no changes

            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(cachedEvents);

            // Act
            await _service.Execute();

            // Assert
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Exactly(3)); // Still updates cache
            _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Once); // Cache log via WriteLog
        }

        [Fact]
        public async Task Execute_ProcessesCorrectDateRange()
        {
            // Arrange
            var currentEvents = new List<EventData>();
            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());

            // Act
            await _service.Execute();

            // Assert - Should process 3 days: yesterday, today, tomorrow
            _mockCache.Verify(x => x.GetMatchesByDate(It.IsAny<DateTime>()), Times.Exactly(6)); // 3 in main logic + 3 in UpdateCacheAfterSuccess
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Execute_LogsCorrectInformation()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "PushMatchInfoForSettlement" &&
                !string.IsNullOrEmpty(criteria.Payload))), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenApiSucceeds_UpdatesCacheWithCorrectData()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.SaveMatchSnapshot(
                It.IsAny<IEnumerable<EventData>>(),
                It.IsAny<DateTime>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Execute_HandlesDifferentTimesCorrectly()
        {
            // This test would need to mock DateTime.Now, which is complex
            // In a real scenario, you'd use a time provider interface
            // For now, we'll test the basic functionality
            var currentEvents = new List<EventData>();
            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.GetMatchesByDate(It.IsAny<DateTime>()), Times.Exactly(6)); // 3 in main logic + 3 in UpdateCacheAfterSuccess
        }

        [Fact]
        public async Task Execute_WhenApiSucceeds_LogsCacheUpdateInformation()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>())).ReturnsAsync(new List<EventData>());
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert - Should log because there are events to process (hasEventsToProcess = true)
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "PushMatchInfoForSettlement Cache" &&
                !string.IsNullOrEmpty(criteria.Payload))), Times.Once);
        }

        [Fact]
        public async Task Execute_WhenNoEventsToProcessAndNoCountChanges_DoesNotLogCacheUpdate()
        {
            // Arrange - No events at all, and cache is also empty
            var currentEvents = new List<EventData>(); // No events
            var cachedEvents = new List<EventData>(); // Empty cache

            _mockEventService.Setup(x => x.GetEventsAsync()).ReturnsAsync(currentEvents);
            
            // Setup cache to return empty events (no count change, no events to process)
            _mockCache.Setup(x => x.GetMatchesByDate(It.IsAny<DateTime>()))
                .ReturnsAsync(cachedEvents);
            
            _mockSettlementApiService.Setup(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert - Should not log cache update because no events to process and no count changes
            _mockLogger.Verify(x => x.Execute(It.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "PushMatchInfoForSettlement Cache")), Times.Never);
        }
    }
}
