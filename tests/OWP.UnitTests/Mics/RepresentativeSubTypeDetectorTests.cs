using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Mics;

public class RepresentativeSubTypeDetectorTests
{
    [Fact]
    public void RepresentativeSubTypeDetector_WhenNextTypeIsNull_ShouldReturnOriginalSubType()
    {
        // Arrange
        var detector = new RepresentativeSubTypeDetector();
        var subType = 100;

        // Act
        var result = detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(subType);
    }

    [Fact]
    public void RepresentativeSubTypeDetector_WhenNextTypeIsSet_ShouldDelegateToNextType()
    {
        // Arrange
        var detector = new RepresentativeSubTypeDetector();
        var nextDetector = new MockRepresentativeSubTypeDetector(999);
        detector.NextType = nextDetector;
        var subType = 100;

        // Act
        var result = detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(999);
    }

    [Fact]
    public void RepresentativeSubTypeDetector_NextType_ShouldBeSettable()
    {
        // Arrange
        var detector = new RepresentativeSubTypeDetector();
        var nextDetector = new MockRepresentativeSubTypeDetector(500);

        // Act
        detector.NextType = nextDetector;

        // Assert
        detector.NextType.Should().Be(nextDetector);
    }

    private class MockRepresentativeSubTypeDetector : RepresentativeSubTypeDetector
    {
        private readonly int _returnValue;

        public MockRepresentativeSubTypeDetector(int returnValue)
        {
            _returnValue = returnValue;
        }

        public override int GetRepresentativeSubtype(int subType)
        {
            return _returnValue;
        }
    }
}
