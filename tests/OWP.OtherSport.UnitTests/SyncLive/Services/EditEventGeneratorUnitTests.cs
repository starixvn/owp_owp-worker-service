using NSubstitute;
using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;
using OWP.OtherSport.SyncLive.Services;
using System.Collections.Generic;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.OtherSport.UnitTests.SyncLive.Services
{
    public class EditEventGeneratorUnitTests
    {
        private readonly EditEventGeneratorImp service;

        public EditEventGeneratorUnitTests()
        {
            service = new EditEventGeneratorImp();
        }

        [Fact]
        public void CreateModel_()
        {
            changedDetector.FilterMatchHasAdminSiteId(Arg.Any<List<Match>>()).Returns(new List<Match>());
            changedDetector.GetInsertedLiveAdminSiteIds(Arg.Any<List<Match>>(), Arg.Any<List<Match>>()).Returns(new List<Match>{
                new(){ OSMatchId = 333, AdminSiteMatchId = 444, OfferLive = "Complete" }
            });
            changedDetector.GetDeletedLiveAdminSiteIds(Arg.Any<List<Match>>(), Arg.Any<List<Match>>()).Returns(new List<Match>{
                new(){ OSMatchId = 111, AdminSiteMatchId = 333, OfferLive = "Live" }
            });
            changedDetector.GetChangedLiveAdminSiteIds(Arg.Any<List<Match>>(), Arg.Any<List<Match>>()).Returns(new List<Match>{
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = "Complete" },
               new(){ OSMatchId = 333, AdminSiteMatchId = 555, OfferLive = "live" }
            });

            var expected = new List<EditEvent>
            {
                new() { Matchid = 444,  OfferLive = false },
                new() { Matchid = 333,  OfferLive = false },
                new() { Matchid = 222,  OfferLive = false },
                new() { Matchid = 555,  OfferLive = true },
            };

            var actual = service.CreateModel(new List<Match>(), new List<Match>());

            DeepAssert.Equal(expected, actual);
        }
    }
}
