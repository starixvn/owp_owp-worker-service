using NSubstitute;
using OWP.PrematchReport.Models;
using OWP.PrematchReport.Services;
using OWP.PrematchReport.Services.Match;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.PrematchReport.UnitTests.Services.Match
{
    public class DataMatchReportServiceUnitests
    {
        private Repository repository;
        private DbWriterService dbWriterService;

        public DataMatchReportServiceUnitests()
        {
            repository = Substitute.For<Repository>();
            dbWriterService = Substitute.For<DbWriterService>();
        }

        [Fact]
        public async Task BuildMatchReport_Always_ReturnDataRePorts()
        {
            //Arrange
            var matchservice = new DataMatchReportServiceImp(repository, dbWriterService);

            //Act
            repository.GetTotalMatchByLeaugeIds(Arg.Compat.Any<IList<int>>(), Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>())
                .Returns(MockTotalDataMatch());
            var leagueDetailInfos = MockLeagueDetailInfo();
            var actual = await matchservice.BuildMatchReport(DateTime.Now, null, leagueDetailInfos, 1);

            //Assert
            var expected = MockExpected();
            DeepAssert.Equal(expected, actual);
        }

        public List<LeagueDetailInfo> MockLeagueDetailInfo()
            => new()
            {
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 0,
                    IsDay = true,
                    OWPLeagueId = 1,
                    PreMatchLeagueId = 1,
                    UserId = 8,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 0,
                    IsDay = true,
                    OWPLeagueId = 1,
                    PreMatchLeagueId = 1,
                    UserId = 12,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 0,
                    IsDay = false,
                    OWPLeagueId = 10,
                    PreMatchLeagueId = 1,
                    UserId = 8,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 0,
                    IsDay = true,
                    OWPLeagueId = 2,
                    PreMatchLeagueId = 1,
                    UserId = 8,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 1,
                    IsDay = true,
                    OWPLeagueId = 3,
                    PreMatchLeagueId = 1,
                    UserId = 8,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 2,
                    IsDay = false,
                    OWPLeagueId = 4,
                    PreMatchLeagueId = 3,
                    UserId = 8,
                },
            };

        public List<TotalDataMatch> MockTotalDataMatch()
             => new()
             {
                 new()
                 {
                     leaugeId = 1,
                     TotalMatchDayShift = 1,
                     TotalMatchNightShift = 2,
                 },
                 new()
                 {
                     leaugeId = 10,
                     TotalMatchDayShift = 8,
                     TotalMatchNightShift = 9,
                 },
                 new()
                 {
                     leaugeId = 2,
                     TotalMatchDayShift = 1,
                     TotalMatchNightShift = 2,
                 },
                 new()
                 {
                     leaugeId = 3,
                     TotalMatchDayShift = 1,
                     TotalMatchNightShift = 2,
                 },
                 new()
                 {
                     leaugeId = 4,
                     TotalMatchDayShift = 2,
                     TotalMatchNightShift = 3,
                 },
             };

        public List<DataReport> MockExpected()
         => new()
         {
             new()
             {
                 UserId = 8,
                 Amount = 11,
                 BetType = 1,
                 EventDate = new DateTime(2020, 10, 2),
                 GroupId = 0,
             },
             new()
             {
                 UserId = 8,
                 Amount = 1,
                 BetType = 1,
                 EventDate = new DateTime(2020, 10, 2),
                 GroupId = 1,
             },
             new()
             {
                 UserId = 8,
                 Amount = 3,
                 BetType = 1,
                 GroupId = 2,
                 EventDate = new DateTime(2020, 10, 2)
             },
             new()
             {
                 UserId = 12,
                 Amount = 1,
                 BetType = 1,
                 GroupId = 2,
                 EventDate = new DateTime(2020, 10, 2)
             },
         };
    }
}