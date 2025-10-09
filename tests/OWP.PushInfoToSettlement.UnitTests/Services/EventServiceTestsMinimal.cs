using NSubstitute;
using OWP.PushInfoToSettlement.Model;
using OWP.PushInfoToSettlement.Services;
using Fanex.Data.Repository;
using Xunit;

namespace OWP.PushInfoToSettlement.UnitTests.Services
{
    public class EventServiceTestsMinimal
    {
        [Fact]
        public async Task EventServiceImp_Constructor_WithNullRepository_DoesNotThrow()
        {
            // Act & Assert - The constructor doesn't validate null parameters
            var exception = await Record.ExceptionAsync(async () => new EventServiceImp(null));
            Assert.Null(exception);
        }

        [Fact]
        public async Task EventServiceImp_Constructor_WithValidRepository_DoesNotThrow()
        {
            // Arrange
            var mockRepository = Substitute.For<IDynamicRepository>();

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => new EventServiceImp(mockRepository));
            Assert.Null(exception);
        }

        [Fact]
        public void EventServiceImp_ImplementsEventServiceInterface()
        {
            // Arrange
            var mockRepository = Substitute.For<IDynamicRepository>();

            // Act
            var service = new EventServiceImp(mockRepository);

            // Assert
            Assert.IsAssignableFrom<EventService>(service);
        }

        [Fact]
        public void EventServiceImp_CanBeInstantiated()
        {
            // Arrange
            var mockRepository = Substitute.For<IDynamicRepository>();

            // Act
            var service = new EventServiceImp(mockRepository);

            // Assert
            Assert.NotNull(service);
        }
    }
}