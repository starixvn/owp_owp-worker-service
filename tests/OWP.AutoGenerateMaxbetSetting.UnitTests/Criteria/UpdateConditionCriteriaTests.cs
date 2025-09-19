using OWP.AutoGenerateMaxbetSetting.Criteria;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Criteria;

public class UpdateConditionCriteriaTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var criteria = new UpdateConditionCriteria();

        // Assert
        criteria.Should().NotBeNull();
        criteria.ActorId.Should().Be(0);
        criteria.LeagueSettingId.Should().Be(0);
        criteria.HasTv.Should().BeFalse();
        criteria.Tv.Should().BeNull();
        criteria.Round.Should().BeNull();
        criteria.Division.Should().BeNull();
        criteria.FullConditionDescription.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 100, true, "TV1", "Round1", "Division1", "Full Description")]
    [InlineData(2, 200, false, "", "", "", "")]
    [InlineData(0, 0, false, "", "", "", "")]
    public void Properties_ShouldSetAndGetCorrectValues(int actorId, int leagueSettingId, bool hasTv,
        string tv, string round, string division, string fullConditionDescription)
    {
        // Arrange
        var criteria = new UpdateConditionCriteria();

        // Act
        criteria.ActorId = actorId;
        criteria.LeagueSettingId = leagueSettingId;
        criteria.HasTv = hasTv;
        criteria.Tv = tv;
        criteria.Round = round;
        criteria.Division = division;
        criteria.FullConditionDescription = fullConditionDescription;

        // Assert
        criteria.ActorId.Should().Be(actorId);
        criteria.LeagueSettingId.Should().Be(leagueSettingId);
        criteria.HasTv.Should().Be(hasTv);
        criteria.Tv.Should().Be(tv);
        criteria.Round.Should().Be(round);
        criteria.Division.Should().Be(division);
        criteria.FullConditionDescription.Should().Be(fullConditionDescription);
    }

    [Fact]
    public void GetSettingKey_ShouldReturnCorrectValue()
    {
        // Arrange
        var criteria = new UpdateConditionCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        result.Should().Be("OWP_NewLeagueSetting_UpdateCondition");
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsValid_ShouldReturnCorrectResult(int leagueSettingId, bool expected)
    {
        // Arrange
        var criteria = new UpdateConditionCriteria
        {
            LeagueSettingId = leagueSettingId
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Act
        var criteria = new UpdateConditionCriteria
        {
            ActorId = 1,
            LeagueSettingId = 100,
            HasTv = true,
            Tv = "TV1",
            Round = "Round1",
            Division = "Division1",
            FullConditionDescription = "Full Description"
        };

        // Assert
        criteria.ActorId.Should().Be(1);
        criteria.LeagueSettingId.Should().Be(100);
        criteria.HasTv.Should().BeTrue();
        criteria.Tv.Should().Be("TV1");
        criteria.Round.Should().Be("Round1");
        criteria.Division.Should().Be("Division1");
        criteria.FullConditionDescription.Should().Be("Full Description");
        criteria.GetSettingKey().Should().Be("OWP_NewLeagueSetting_UpdateCondition");
        criteria.IsValid().Should().BeTrue();
    }
}
