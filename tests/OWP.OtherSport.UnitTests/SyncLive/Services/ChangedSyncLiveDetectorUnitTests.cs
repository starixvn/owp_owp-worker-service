using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.OtherSport.UnitTests.SyncLive.Services
{
    public class ChangedSyncLiveDetectorUnitTests
    {
        private readonly ChangedSyncLiveDetectorImp service;

        public ChangedSyncLiveDetectorUnitTests()
        {
            service = new ChangedSyncLiveDetectorImp();
        }

        [Fact]
        public void FilterMatchHasAdminSiteId_ReturnCorrectFilter()
        {
            var matches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111 },
                new(){ OSMatchId = 222, AdminSiteMatchId = null },
                new(){ OSMatchId = 333, AdminSiteMatchId = 111 }
            };

            var expected = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111 },
                new(){ OSMatchId = 333, AdminSiteMatchId = 111 }
            };

            var actual = service.FilterMatchHasAdminSiteId(matches);

            DeepAssert.Equal(expected, actual);
        }

        [Fact]
        public void GetDeletedLiveAdminSiteIds_ReturnCorrectFilter()
        {
            var oldMatches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111, OfferLive = true },
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = true },
                new(){ OSMatchId = 333, AdminSiteMatchId = 333, OfferLive = true }
            };

            var newMatches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111, OfferLive = true },
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = true },
                new(){ OSMatchId = 333, AdminSiteMatchId = 444, OfferLive = false }
            };

            var expected = new List<Match>
            {
               new(){ OSMatchId = 333, AdminSiteMatchId = 333, OfferLive = false }
            };

            var actual = service.GetDeletedLiveAdminSiteIds(oldMatches, newMatches);

            DeepAssert.Equal(expected, actual);
        }

        [Fact]
        public void GetInsertedLiveAdminSiteIds_ReturnCorrectFilter()
        {
            var oldMatches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111, OfferLive = "Live" },
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = "Live" },
                new(){ OSMatchId = 333, AdminSiteMatchId = 333, OfferLive = "live" }
            };

            var newMatches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111, OfferLive = "Live" },
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = "Live" },
                new(){ OSMatchId = 333, AdminSiteMatchId = 444, OfferLive = "Complete" }
            };

            var expected = new List<Match>
            {
               new(){ OSMatchId = 333, AdminSiteMatchId = 444, OfferLive = "Complete" }
            };

            var actual = service.GetInsertedLiveAdminSiteIds(oldMatches, newMatches);

            DeepAssert.Equal(expected, actual);
        }

        [Fact]
        public void GetChangedLiveAdminSiteIds_ReturnCorrectFilter()
        {
            var oldMatches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111, OfferLive = "Live" },
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = "Live" },
                new(){ OSMatchId = 333, AdminSiteMatchId = 333, OfferLive = "complete" }
            };

            var newMatches = new List<Match>
            {
                new(){ OSMatchId = 111, AdminSiteMatchId = 111, OfferLive = "Live" },
                new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = "Complete" },
                new(){ OSMatchId = 333, AdminSiteMatchId = 333, OfferLive = "live" }
            };

            var expected = new List<Match>
            {
               new(){ OSMatchId = 222, AdminSiteMatchId = 222, OfferLive = "Complete" },
               new(){ OSMatchId = 333, AdminSiteMatchId = 333, OfferLive = "live" }
            };

            var actual = service.GetChangedLiveAdminSiteIds(oldMatches, newMatches).ToList();

            DeepAssert.Equal(expected, actual);
        }
    }
}
