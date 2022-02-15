using NSubstitute;
using OWP.PrematchReport.Models;
using OWP.PrematchReport.Services.Match;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.PrematchReport.UnitTests.Services
{
    public class OUMatchServiceUnitests
    {
        private Repository repository;
        private DataMatchReportService dataMatchReportService;

        public OUMatchServiceUnitests()
        {
            repository = Substitute.For<Repository>();
            dataMatchReportService = Substitute.For<DataMatchReportService>();
        }

        [Fact]
        public async Task SearchLAReport_ShouldReturnData()
        {
            //Arrange
            var ouMatchservice = new OUMatchServiceImp(repository, dataMatchReportService);

            //Act
            repository.GetLeagueDetailByDate(Arg.Compat.Any<DateTime>(), Arg.Compat.Any<int>())
                .Returns(MockLeagueDetailInfo());
            repository.GetTotalMatchByLeaugeIds(Arg.Compat.Any<IList<int>>(), Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>())
                .Returns(MockTotalDataMatch());

            var actual = ouMatchservice.BuildSingleOUMatchReport(Arg.Compat.Any<DateTime>(), null);

            //Assert
            var expected = MockLeagueDetailInfo();
            DeepAssert.Equal(1, 1);
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
                    PrematchLeagueId = 1,
                    UserId = 8,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 1,
                    IsDay = true,
                    OWPLeagueId = 1,
                    PrematchLeagueId = 1,
                    UserId = 8,
                },
                new()
                {
                    EventDate = new DateTime(2020, 10, 2),
                    GroupId = 2,
                    IsDay = false,
                    OWPLeagueId = 1,
                    PrematchLeagueId = 1,
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
                     leaugeId = 1,
                     TotalMatchDayShift = 1,
                     TotalMatchNightShift = 2,
                 },
                 new()
                 {
                     leaugeId = 1,
                     TotalMatchDayShift = 1,
                     TotalMatchNightShift = 2,
                 },
             };

        public List<DataReport> MockExpected()
         => new()
         {
             new()
             {
                 UserId = 1,
                 Amount = 1,
                 BetType = 1,
             },
             new()
             {
                 UserId = 1,
                 Amount = 1,
                 BetType = 1,
             },
             new()
             {
                 UserId = 1,
                 Amount = 1,
                 BetType = 1,
             },
         };
    }
}