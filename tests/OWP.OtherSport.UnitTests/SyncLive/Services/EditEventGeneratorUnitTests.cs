using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;
using OWP.OtherSport.SyncLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void CreateModel_GetChangedOfferLiveMatches_ReturnExpectedResult()
        {
            var adminSiteMatches = new List<Match>
            {
                new(){ OSMatchId = 1, AdminSiteMatchId = 111, Sport = "Basketball", OfferLive = true  },
                new(){ OSMatchId = 2, AdminSiteMatchId = 222, Sport = "Basketball", OfferLive = false },
                new(){ OSMatchId = 3, AdminSiteMatchId = 333, Sport = "Basketball", OfferLive = true },
                new(){ OSMatchId = 4, AdminSiteMatchId = 555, Sport = "Basketball", OfferLive = true },
                new(){ OSMatchId = 5, AdminSiteMatchId = 666, Sport = "Tennis", OfferLive = true },
                new(){ OSMatchId = 6, AdminSiteMatchId = null, Sport = "Tennis", OfferLive = true },
            };

            var owpMatches = new List<Match>
            {
                new(){ AdminSiteMatchId = 111, Sport = "Basketball", OfferLive = true },
                new(){ AdminSiteMatchId = 222, Sport = "Basketball", OfferLive = true },
                new(){ AdminSiteMatchId = 444, Sport = "Basketball", OfferLive = false },
                new(){ AdminSiteMatchId = 555, Sport = "Basketball", OfferLive = false }, 
                new(){ AdminSiteMatchId = 666, Sport = "Tennis", OfferLive = false },
                new(){ AdminSiteMatchId = null, Sport = "Tennis", OfferLive = false },
            };

            var expected = new List<EditEvent>
            {
                new() { Matchid = 222,  OfferLive = true },
                new() { Matchid = 555,  OfferLive = false },
            };

            var actual = service.GetChangedOfferLiveMatches(adminSiteMatches, owpMatches).ToList();

            DeepAssert.Equal(expected, actual);
        }
    }
}
