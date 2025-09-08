using Moq;
using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using Olympus.OWP.Log.Data.Repository.Services;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Xunit;

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
                .ReturnsAsync(true);

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Exactly(3));
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Once);
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
                .ReturnsAsync(false);

            // Act
            await _service.Execute();

            // Assert
            _mockCache.Verify(x => x.SaveMatchSnapshot(It.IsAny<IEnumerable<EventData>>(), It.IsAny<DateTime>()), Times.Never);
            _mockSettlementApiService.Verify(x => x.ChangeInstantSettingAsync(It.IsAny<IEnumerable<ChangeInstantSettingRequest>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Once);
        }


    }
}
