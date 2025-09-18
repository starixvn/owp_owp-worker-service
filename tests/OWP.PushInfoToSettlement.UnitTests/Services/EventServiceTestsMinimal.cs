using Moq;
using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using Fanex.Data.Repository;
using Xunit;

namespace OWP.PushInfoToSettlement.UnitTests.Services
{
    public class EventServiceTestsMinimal
    {
        [Fact]
        public void EventServiceImp_Constructor_WithNullRepository_DoesNotThrow()
        {
            // Act & Assert - The constructor doesn't validate null parameters
            var exception = Record.Exception(() => new EventServiceImp(null));
            Assert.Null(exception);
        }

        [Fact]
        public void EventServiceImp_Constructor_WithValidRepository_DoesNotThrow()
        {
            // Arrange
            var mockRepository = new Mock<IDynamicRepository>();

            // Act & Assert
            var exception = Record.Exception(() => new EventServiceImp(mockRepository.Object));
            Assert.Null(exception);
        }

        [Fact]
        public void EventServiceImp_ImplementsEventServiceInterface()
        {
            // Arrange
            var mockRepository = new Mock<IDynamicRepository>();

            // Act
            var service = new EventServiceImp(mockRepository.Object);

            // Assert
            Assert.IsAssignableFrom<EventService>(service);
        }

        [Fact]
        public void EventServiceImp_CanBeInstantiated()
        {
            // Arrange
            var mockRepository = new Mock<IDynamicRepository>();

            // Act
            var service = new EventServiceImp(mockRepository.Object);

            // Assert
            Assert.NotNull(service);
        }
    }
}
