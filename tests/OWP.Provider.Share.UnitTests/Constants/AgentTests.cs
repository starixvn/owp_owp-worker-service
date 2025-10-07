using System.Collections.Immutable;
using OWP.Provider.Share.AgentConfirm.Constants;

namespace OWP.Provider.Share.UnitTests.Constants;

public class AgentTests
{
    [Fact]
    public void Streaming_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Agent.Streaming.Should().Be("Streaming");
    }

    [Fact]
    public void Cancel_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Agent.Cancel.Should().Be("Cancel");
    }

    [Fact]
    public void Betradar_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Agent.Betradar.Should().Be("Betradar");
    }

    [Fact]
    public void Betradar2_ShouldReturnCorrectValue()
    {
        // Act & Assert
        Agent.Betradar2.Should().Be("Betradar2");
    }

    [Fact]
    public void ProviderList_ShouldContainExpectedProviders()
    {
        // Arrange
        var expectedProviders = new[]
        {
            "365GV", "BG", "FC", "IMG", "RB2", "RB3", "RTS", "SLR", "WOO"
        };

        // Act & Assert
        Agent.ProviderList.Should().HaveCount(9);
        Agent.ProviderList.Should().Contain(expectedProviders);
    }

    [Fact]
    public void ProviderList_ShouldBeImmutable()
    {
        // Act & Assert
        Agent.ProviderList.Should().BeOfType<ImmutableList<string>>();
    }

    [Fact]
    public void InhouseProviderList_ShouldContainAllProviderListItems()
    {
        // Act & Assert
        Agent.InhouseProviderList.Should().HaveCount(11);
        Agent.InhouseProviderList.Should().Contain(Agent.ProviderList);
        Agent.InhouseProviderList.Should().Contain(Agent.Betradar);
        Agent.InhouseProviderList.Should().Contain(Agent.Betradar2);
    }

    [Fact]
    public void InhouseProviderList_ShouldBeImmutable()
    {
        // Act & Assert
        Agent.InhouseProviderList.Should().BeOfType<ImmutableList<string>>();
    }

    [Fact]
    public void InhouseProviderList_ShouldHaveCorrectOrder()
    {
        // Arrange
        var expectedOrder = Agent.ProviderList.Add(Agent.Betradar).Add(Agent.Betradar2);

        // Act & Assert
        Agent.InhouseProviderList.Should().Equal(expectedOrder);
    }

    [Fact]
    public void AllConstants_ShouldNotBeNullOrEmpty()
    {
        // Act & Assert
        Agent.Streaming.Should().NotBeNullOrEmpty();
        Agent.Cancel.Should().NotBeNullOrEmpty();
        Agent.Betradar.Should().NotBeNullOrEmpty();
        Agent.Betradar2.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AllConstants_ShouldBeUnique()
    {
        // Arrange
        var constants = new[] { Agent.Streaming, Agent.Cancel, Agent.Betradar, Agent.Betradar2 };

        // Act & Assert
        constants.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void ProviderList_ShouldNotContainNullOrEmpty()
    {
        // Act & Assert
        Agent.ProviderList.Should().NotContainNulls();
        Agent.ProviderList.Should().NotContain("");
    }

    [Fact]
    public void InhouseProviderList_ShouldNotContainNullOrEmpty()
    {
        // Act & Assert
        Agent.InhouseProviderList.Should().NotContainNulls();
        Agent.InhouseProviderList.Should().NotContain("");
    }
}

