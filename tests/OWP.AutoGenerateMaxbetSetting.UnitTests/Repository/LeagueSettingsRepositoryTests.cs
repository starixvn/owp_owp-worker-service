using Fanex.Data.Repository;
using NSubstitute;
using OWP.AutoGenerateMaxbetSetting;
using OWP.AutoGenerateMaxbetSetting.Models;
using Match = OWP.AutoGenerateMaxbetSetting.Models.Match;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Repository;

public class LeagueSettingsRepositoryTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();

        // Act
        var repository = new LeagueSettingsRepositoryImp(mockRepository);

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void GetMatchesAndSettings_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new LeagueSettingsRepositoryImp(mockRepository);
        var eventDate = new DateTime(2024, 1, 15);

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(LeagueSettingsRepositoryImp).GetMethod("GetMatchesAndSettings");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>>));
    }

    [Fact]
    public void InsertNewLeaguesSettings_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new LeagueSettingsRepositoryImp(mockRepository);
        var leagueSettings = new List<LeagueSetting>();

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(LeagueSettingsRepositoryImp).GetMethod("InsertNewLeaguesSettings");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public async Task InsertNewLeaguesSettings_ShouldHandleNullList()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new LeagueSettingsRepositoryImp(mockRepository);
        List<LeagueSetting>? nullSettings = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repository.InsertNewLeaguesSettings(nullSettings!));
    }
}
