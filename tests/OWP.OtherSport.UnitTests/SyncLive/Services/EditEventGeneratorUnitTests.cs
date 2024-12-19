namespace OWP.OtherSport.UnitTests.SyncLive.Services;

using OtherSport.SyncAdminSiteId.Models;
using OtherSport.SyncLive.Model;
using OtherSport.SyncLive.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Asserts.Compare;

public class EditEventGeneratorUnitTests
{
    private readonly EditEventGeneratorImp service = new();

    [Fact]
    public void CreateModel_GetChangedOfferLiveMatches_ReturnExpectedResult()
    {
        var adminSiteMatches = new List<Match>
        {
            new(){ OsMatchId = 1, AdminSiteMatchId = 111, Sport = "Basketball", OfferLive = true  },
            new(){ OsMatchId = 2, AdminSiteMatchId = 222, Sport = "Basketball", OfferLive = false },
            new(){ OsMatchId = 3, AdminSiteMatchId = 333, Sport = "Basketball", OfferLive = true },
            new(){ OsMatchId = 4, AdminSiteMatchId = 555, Sport = "Basketball", OfferLive = true },
            new(){ OsMatchId = 5, AdminSiteMatchId = 666, Sport = "Tennis", OfferLive = true },
            new(){ OsMatchId = 6, AdminSiteMatchId = null, Sport = "Tennis", OfferLive = true }
        };

        var owpMatches = new List<Match>
        {
            new(){ AdminSiteMatchId = 111, Sport = "Basketball", OfferLive = true },
            new(){ AdminSiteMatchId = 222, Sport = "Basketball", OfferLive = true },
            new(){ AdminSiteMatchId = 444, Sport = "Basketball", OfferLive = false },
            new(){ AdminSiteMatchId = 555, Sport = "Basketball", OfferLive = false },
            new(){ AdminSiteMatchId = 666, Sport = "Tennis", OfferLive = false },
            new(){ AdminSiteMatchId = null, Sport = "Tennis", OfferLive = false }
        };

        var expected = new List<EditEvent>
        {
            new() { in_house_match_id = 222,  is_live = true },
            new() { in_house_match_id = 555,  is_live = false },
            new() { in_house_match_id = 666,  is_live = false }
        };

        var actual = service.GetChangedOfferLiveMatches(adminSiteMatches, owpMatches).ToList();

        DeepAssert.Equal(expected, actual);
    }
}