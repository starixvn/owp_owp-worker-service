namespace OWP.OtherSport.UnitTests.SyncAdminSiteId.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using OtherSport.SyncAdminSiteId.Helpers;
using OtherSport.SyncAdminSiteId.Models;
using Xunit;
using Xunit.Asserts.Compare;

public class MatchHelperUnitTests
{
    [Fact]
    public void GetNewLinkedMatches_ReturnUnlinkedMatches()
    {
        var expected = new List<LinkedMatch>
            {
                new()
                {
                    AdminSiteMatchId = 123,
                    OsMatchId = 1,
                    OwpLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    OwpHome = "Dalian Pro FC (N)1",
                    OwpAway = "Guangzhou R&F1",
                    AdminSiteLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    AdminSiteHome = "Dalian Pro FC (N)1",
                    AdminSiteAway = "Guangzhou R&F1",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18, 20, 22, 33),
                    OldEventDate = new DateTime(2021, 03, 18)
                },
                new()
                {
                    AdminSiteMatchId = 234,
                    OsMatchId = 53,
                    OwpLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    OwpHome = "Dalian Pro FC (N)2",
                    OwpAway = "Guangzhou R&F2",
                    AdminSiteLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    AdminSiteHome = "Dalian Pro FC (N)2",
                    AdminSiteAway = "Guangzhou R&F2",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18),
                    OldEventDate = new DateTime(2021, 03, 18)
                },
                new()
                {
                    AdminSiteMatchId = 345,
                    OsMatchId = 54,
                    OwpLeague = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    OwpHome = "Dalian Pro FC (N)3",
                    OwpAway = "Guangzhou R&F3",
                    AdminSiteLeague = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    AdminSiteHome = "Dalian Pro FC (N)3",
                    AdminSiteAway = "Guangzhou R&F3",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18, 11, 22, 33),
                    OldEventDate = new DateTime(2021, 03, 18, 11, 22, 33)
                }
            };

        var actual = MatchHelper.GetNewLinkedMatches(GetUnlinkedOWPMatches(), GetAdminSiteMatches());

        DeepAssert.Equal(expected, actual.ToList());
    }

    [Fact]
    public void GetUpdatedMatches_ReturnLinkedMatches()
    {
        var expected = new List<LinkedMatch>
            {
                new()
                {
                    OsMatchId = 444,
                    OwpLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    OwpHome = "Dalian Pro FC (N)1",
                    OwpAway = "Guangzhou R&F1",
                    OldAdminSiteMatchId = 111,
                    OldEventDate = new DateTime(2021, 03, 18, 20, 22, 33)
                }
            };

        var actual = MatchHelper.GetRemovedMatches(GetLinkedOWPMatches(), GetAdminSiteMatches());
        DeepAssert.Equal(expected, actual.ToList());
    }

    private IEnumerable<Match> GetUnlinkedOWPMatches()
        => new List<Match>
        {
                new()
                {
                    OsMatchId = 1,
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)1",
                    AwayName = "Guangzhou R&F1",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                },
                new()
                {
                    OsMatchId = 53,
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)2",
                    AwayName = "Guangzhou R&F2",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                },
                new()
                {
                    OsMatchId = 54,
                    Sport = "Basketball",
                    LeagueName = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18, 11, 22, 33),
                    AdminSiteMatchId = null
                },
                new()
                {
                    OsMatchId = 114,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)15",
                    HomeName = "Dalian Pro FC (N)12",
                    AwayName = "Guangzhou R&F12",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                }
        };

    private IEnumerable<Match> GetLinkedOWPMatches()
        => new List<Match>
        {
                new()
                {
                    OsMatchId = 79,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    HomeName = "Dalian Pro FC (N)11",
                    AwayName = "Guangzhou R&F11",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 1
                },
                new()
                {
                    OsMatchId = 113,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    HomeName = "Dalian Pro FC (N)12",
                    AwayName = "Guangzhou R&F12",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 2
                },
                new()
                {
                    OsMatchId = 444,
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)1",
                    AwayName = "Guangzhou R&F1",
                    EventDateTime = new DateTime(2021, 03, 18, 20, 22, 33),
                    AdminSiteMatchId = 111
                }
        };

    private IEnumerable<Match> GetAdminSiteMatches()
        => new List<Match>
        {
                new()
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)1",
                    AwayName = "Guangzhou R&F1",
                    EventDateTime = new DateTime(2021, 03, 18, 20, 22, 33),
                    AdminSiteMatchId = 123
                },
                new()
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)2",
                    AwayName = "Guangzhou R&F2",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 234
                },
                new()
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18, 11, 22, 33),
                    AdminSiteMatchId = 345
                },
                new()
                {
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)113",
                    HomeName = "Dalian Pro FC (N)11",
                    AwayName = "Guangzhou R&F11",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 1
                },
                new()
                {
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)114",
                    HomeName = "Dalian Pro FC (N)12",
                    AwayName = "Guangzhou R&F12",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 2
                }
        };
}