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
                new LinkedMatch
                {
                    AdminSiteMatchId = 123,
                    OSMatchId = 1,
                    OWPLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    OWPHome = "Dalian Pro FC (N)1",
                    OWPAway = "Guangzhou R&F1",
                    AdminSiteLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    AdminSiteHome = "Dalian Pro FC (N)1",
                    AdminSiteAway = "Guangzhou R&F1",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18, 20, 22, 33),
                    OldEventDate = new DateTime(2021, 03, 18)
                },
                new LinkedMatch
                {
                    AdminSiteMatchId = 234,
                    OSMatchId = 53,
                    OWPLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    OWPHome = "Dalian Pro FC (N)2",
                    OWPAway = "Guangzhou R&F2",
                    AdminSiteLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    AdminSiteHome = "Dalian Pro FC (N)2",
                    AdminSiteAway = "Guangzhou R&F2",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18),
                    OldEventDate = new DateTime(2021, 03, 18)
                },
                new LinkedMatch
                {
                    AdminSiteMatchId = 345,
                    OSMatchId = 54,
                    OWPLeague = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    OWPHome = "Dalian Pro FC (N)3",
                    OWPAway = "Guangzhou R&F3",
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
                new LinkedMatch
                {
                    OSMatchId = 444,
                    OWPLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    OWPHome = "Dalian Pro FC (N)1",
                    OWPAway = "Guangzhou R&F1",
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
                new Match
                {
                    OSMatchId = 1,
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)1",
                    AwayName = "Guangzhou R&F1",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                },
                new Match
                {
                    OSMatchId = 53,
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)2",
                    AwayName = "Guangzhou R&F2",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                },
                new Match
                {
                    OSMatchId = 54,
                    Sport = "Basketball",
                    LeagueName = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18, 11, 22, 33),
                    AdminSiteMatchId = null
                },
                new Match
                {
                    OSMatchId = 114,
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
                new Match
                {
                    OSMatchId = 79,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    HomeName = "Dalian Pro FC (N)11",
                    AwayName = "Guangzhou R&F11",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 1
                },
                new Match
                {
                    OSMatchId = 113,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    HomeName = "Dalian Pro FC (N)12",
                    AwayName = "Guangzhou R&F12",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 2
                },
                new Match
                {
                    OSMatchId = 444,
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
                new Match
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)1",
                    AwayName = "Guangzhou R&F1",
                    EventDateTime = new DateTime(2021, 03, 18, 20, 22, 33),
                    AdminSiteMatchId = 123
                },
                new Match
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)2",
                    AwayName = "Guangzhou R&F2",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 234
                },
                new Match
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA SABA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18, 11, 22, 33),
                    AdminSiteMatchId = 345
                },
                new Match
                {
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)113",
                    HomeName = "Dalian Pro FC (N)11",
                    AwayName = "Guangzhou R&F11",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = 1
                },
                new Match
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