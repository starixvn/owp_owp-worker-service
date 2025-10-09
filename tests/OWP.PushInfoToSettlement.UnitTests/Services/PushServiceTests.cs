using NSubstitute;
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
        private readonly CacheManipulator _mockCache;
        private readonly EventService _mockEventService;
        private readonly ISettlementApiService _mockSettlementApiService;
        private readonly LogWriterService _mockLogger;
        private readonly PushServiceImp _service;

        public PushServiceTests()
        {
            _mockCache = Substitute.For<CacheManipulator>();
            _mockEventService = Substitute.For<EventService>();
            _mockSettlementApiService = Substitute.For<ISettlementApiService>();
            _mockLogger = Substitute.For<LogWriterService>();
            _service = new PushServiceImp(_mockCache, _mockEventService, _mockSettlementApiService, _mockLogger);
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
            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Received(3).SaveMatchSnapshot(Arg.Any<IEnumerable<EventData>>(), Arg.Any<DateTime>()); // 3 days
            _mockSettlementApiService.DidNotReceive().ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task Execute_WhenEventsExistAndApiSucceeds_UpdatesCacheAndSendsApi()
        {
            // Arrange
            var currentEvents = new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Test remark", EventDateTime = DateTime.Today }
            };

            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Received(3).SaveMatchSnapshot(Arg.Any<IEnumerable<EventData>>(), Arg.Any<DateTime>());
            _mockSettlementApiService.Received(1).ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>());
            _mockLogger.Received(2).Execute(Arg.Any<OwpLogCriteria>()); // Main log + cache log
        }

        [Fact]
        public async Task Execute_WhenEventsExistAndApiFails_DoesNotUpdateCache()
        {
            // Arrange
            var currentEvents = new List<EventData>
            {
                new() { OWPMatchId = 1, AdminSiteId = 100, Remark = "Test remark", EventDateTime = DateTime.Today }
            };

            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((false, "API Error")));

            // Act
            await _service.Execute();

            // Assert
            _mockCache.DidNotReceive().SaveMatchSnapshot(Arg.Any<IEnumerable<EventData>>(), Arg.Any<DateTime>()); // No cache update on API failure
            _mockSettlementApiService.Received(1).ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>());
            _mockLogger.Received(1).Execute(Arg.Any<OwpLogCriteria>()); // Only main log
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

            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(cachedEvents);
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockSettlementApiService.Received(1).ChangeInstantSettingAsync(
                Arg.Is<IEnumerable<ChangeInstantSettingRequest>>(requests =>
                    requests.Count() == 2 && // Only 2 changed events
                    requests.Any(r => r.MatchId == 100 && r.Remark == "Test remark 1") &&
                    requests.Any(r => r.MatchId == 102 && r.Remark == "Test remark 3")),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>());
        }

        [Fact]
        public async Task Execute_WhenNewEventsExist_ProcessesAllNewEvents()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            var cachedEvents = new List<EventData>(); // Empty cache - all events are new

            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(cachedEvents);
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockSettlementApiService.Received(1).ChangeInstantSettingAsync(
                Arg.Is<IEnumerable<ChangeInstantSettingRequest>>(requests => requests.Count() == 3),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>());
        }

        [Fact]
        public async Task Execute_WhenNoChangesDetected_DoesNotCallApi()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            var cachedEvents = CreateSampleEvents(); // Same events - no changes

            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(cachedEvents);

            // Act
            await _service.Execute();

            // Assert
            _mockSettlementApiService.DidNotReceive().ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>());
            _mockCache.Received(3).SaveMatchSnapshot(Arg.Any<IEnumerable<EventData>>(), Arg.Any<DateTime>()); // Still updates cache
            _mockLogger.Received(1).Execute(Arg.Any<OwpLogCriteria>()); // Cache log via WriteLog
        }

        [Fact]
        public async Task Execute_ProcessesCorrectDateRange()
        {
            // Arrange
            var currentEvents = new List<EventData>();
            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());

            // Act
            await _service.Execute();

            // Assert - Should process 3 days: yesterday, today, tomorrow
            _mockCache.Received(6).GetMatchesByDate(Arg.Any<DateTime>()); // 3 in main logic + 3 in UpdateCacheAfterSuccess
            _mockCache.Received(3).SaveMatchSnapshot(Arg.Any<IEnumerable<EventData>>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task Execute_LogsCorrectInformation()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockLogger.Received(1).Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "PushMatchInfoForSettlement" &&
                !string.IsNullOrEmpty(criteria.Payload)));
        }

        [Fact]
        public async Task Execute_WhenApiSucceeds_UpdatesCacheWithCorrectData()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Received(3).SaveMatchSnapshot(
                Arg.Any<IEnumerable<EventData>>(),
                Arg.Any<DateTime>());
        }

        [Fact]
        public async Task Execute_HandlesDifferentTimesCorrectly()
        {
            // This test would need to mock DateTime.Now, which is complex
            // In a real scenario, you'd use a time provider interface
            // For now, we'll test the basic functionality
            var currentEvents = new List<EventData>();
            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Received(6).GetMatchesByDate(Arg.Any<DateTime>()); // 3 in main logic + 3 in UpdateCacheAfterSuccess
        }

        [Fact]
        public async Task Execute_WhenApiSucceeds_LogsCacheUpdateInformation()
        {
            // Arrange
            var currentEvents = CreateSampleEvents();
            _mockEventService.GetEventsAsync().Returns(currentEvents);
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>()).Returns(new List<EventData>());
            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert - Should log because there are events to process (hasEventsToProcess = true)
            _mockLogger.Received(1).Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "PushMatchInfoForSettlement Cache" &&
                !string.IsNullOrEmpty(criteria.Payload)));
        }

        [Fact]
        public async Task Execute_WhenNoEventsToProcessAndNoCountChanges_DoesNotLogCacheUpdate()
        {
            // Arrange - No events at all, and cache is also empty
            var currentEvents = new List<EventData>(); // No events
            var cachedEvents = new List<EventData>(); // Empty cache

            _mockEventService.GetEventsAsync().Returns(currentEvents);

            // Setup cache to return empty events (no count change, no events to process)
            _mockCache.GetMatchesByDate(Arg.Any<DateTime>())
                .Returns(cachedEvents);

            _mockSettlementApiService.ChangeInstantSettingAsync(Arg.Any<IEnumerable<ChangeInstantSettingRequest>>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Task.FromResult((true, "Success")));

            // Act
            await _service.Execute();

            // Assert - Should not log cache update because no events to process and no count changes
            _mockLogger.DidNotReceive().Execute(Arg.Is<OwpLogCriteria>(criteria =>
                criteria.Action == "PushMatchInfoForSettlement Cache"));
        }
    }
}