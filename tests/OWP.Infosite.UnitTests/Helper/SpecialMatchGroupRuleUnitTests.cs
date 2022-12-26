using OWP.Infosite.Helper;
using OWP.Infosite.Models;
using Xunit;

namespace OWP.Infosite.UnitTests.Helper
{
    public class SpecialMatchGroupRuleUnitTests
    {
        private NextCornerRule firstRule;

        public SpecialMatchGroupRuleUnitTests()
        {
            firstRule = new NextCornerRule();
            var secondRule = new NextBookingRule();
            var thirdRule = new Specific15MinsRule();
            var fourthRule = new GoalKickRule();
            var fifthRule = new PenaltyToScoreRule();
            var sixthRule = new PenaltyAwardedRule();

            firstRule.NextType = secondRule;
            secondRule.NextType = thirdRule;
            thirdRule.NextType = fourthRule;
            fourthRule.NextType = fifthRule;
            fifthRule.NextType = sixthRule;
        }

        [Fact]
        public void NextCornerMatch()
        {
            var match = new MappedChildMatch
            {
                MinorBettypeId = -4,
                LeagueName = "CHINA FOOTBALL SUPER LEAGUE - CORNERS",
                HomeName = "Shandong Taishan 1st Corner",
                AwayName = "Cangzhou Mighty Lions 1st corner",
            };

            var actual = firstRule.GetMinorType(match);
            firstRule.GetHomeAwayName(match);

            Assert.Equal(-4, actual);
            Assert.Equal("Shandong Taishan Next Corners", match.HomeName);
            Assert.Equal("Cangzhou Mighty Lions Next Corners", match.AwayName);
            Assert.True(match.FirstMatch);
        }

        [Fact]
        public void NextBookingMatch()
        {
            var match = new MappedChildMatch
            {
                MinorBettypeId = -7,
                LeagueName = "CHINA FOOTBALL SUPER LEAGUE - BOOKING",
                HomeName = "Shandong Taishan 1st Booking",
                AwayName = "Cangzhou Mighty Lions 1st Booking",
            };

            var actual = firstRule.GetMinorType(match);
            firstRule.GetHomeAwayName(match);

            Assert.Equal(-7, actual);
            Assert.Equal("Shandong Taishan Next Booking", match.HomeName);
            Assert.Equal("Cangzhou Mighty Lions Next Booking", match.AwayName);
            Assert.True(match.FirstMatch);
        }

        [Fact]
        public void GoalKickMatch()
        {
            var match = new MappedChildMatch
            {
                MinorBettypeId = -100,
                LeagueName = "CHINA FOOTBALL SUPER LEAGUE - GOAL KICK",
                HomeName = "Shandong Taishan 2nd Goal Kick",
                AwayName = "Cangzhou Mighty Lions 2nd Goal Kick",
            };

            var actual = firstRule.GetMinorType(match);
            firstRule.GetHomeAwayName(match);

            Assert.Equal(-100, actual);
            Assert.Equal("Shandong Taishan Next Goal Kick", match.HomeName);
            Assert.Equal("Cangzhou Mighty Lions Next Goal Kick", match.AwayName);
            Assert.False(match.FirstMatch);
        }

        [Fact]
        public void PenaltyToScoreMatch()
        {
            var match = new MappedChildMatch
            {
                MinorBettypeId = -96,
                LeagueName = "*FIFA WORLD CUP 2022 (IN QATAR) - PENALTY",
                HomeName = "Argentina -vs- Poland 1st Penalty To Score - No",
                AwayName = "Argentina -vs- Poland 1st Penalty To Score - Yes",
            };

            var actual = firstRule.GetMinorType(match);
            firstRule.GetHomeAwayName(match);

            Assert.Equal(-96, actual);
            Assert.Equal("Argentina", match.HomeName);
            Assert.Equal("Poland 1st Penalty To Score - Yes", match.AwayName);
            Assert.True(match.FirstMatch);
        }

        [Fact]
        public void PenaltyAwardedMatch()
        {
            var match = new MappedChildMatch
            {
                MinorBettypeId = -97,
                LeagueName = "*FIFA WORLD CUP 2022 (IN QATAR) - PENALTY",
                HomeName = "Mexico -vs- Saudi Arabia 22nd Penalty Awarded - No",
                AwayName = "Mexico -vs- Saudi Arabia 22nd Penalty Awarded - Yes",
            };

            var actual = firstRule.GetMinorType(match);
            firstRule.GetHomeAwayName(match);

            Assert.Equal(-97, actual);
            Assert.Equal("Mexico", match.HomeName);
            Assert.Equal("Saudi Arabia 1st Penalty Awarded - Yes", match.AwayName);
            Assert.False(match.FirstMatch);
        }
    }
}
