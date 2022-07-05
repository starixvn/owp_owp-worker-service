using Fanex.Data.Repository;
using NSubstitute;
using Olympus.Xml;
using OWP.OtherSport.SyncAdminSiteId.Criteria;
using OWP.OtherSport.SyncAdminSiteId.Handlers;
using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OWP.OtherSport.UnitTests.SyncAdminSiteId;

public class SyncAdminSiteMatchesHandlerUnitTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly SyncLiveToAdminSiteHandler syncLiveService;
    private readonly SyncAdminSiteMatchesHandlerImp service;

    public SyncAdminSiteMatchesHandlerUnitTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        syncLiveService = Substitute.For<SyncLiveToAdminSiteHandler>();

        dynamicRepository.FetchAsync<Match>(Arg.Any<GetMatchesFromOWPCriteria>()).Returns(GetOWPMatches());
        dynamicRepository.FetchAsync<Match>(Arg.Any<GetMatchesFromAdminSiteCriteria>()).Returns(GetAdminSiteMatches());

        service = new SyncAdminSiteMatchesHandlerImp(dynamicRepository, syncLiveService);
    }

    [Fact]
    public async Task Handle_WithValidEventDate_UpdateMatches()
    {
        await service.Handle(new DateTime());

        await dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<UpdateUnlinkedMatchesCriteria>(u
                => u.UnlinkedMatches == XmlConverter.Serialize(GetUnlinkedMatches())));
        await dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<UpdateLinkedMatchesCriteria>(u
                => u.LinkedMatches == XmlConverter.Serialize(GetLinkedMatches())));
    }

    private IEnumerable<LinkedMatch> GetUnlinkedMatches()
        => new List<LinkedMatch>
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
                    NewEventDate = new DateTime(2021, 03, 18),
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
                    OWPLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    OWPHome = "Dalian Pro FC (N)3",
                    OWPAway = "Guangzhou R&F3",
                    AdminSiteLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    AdminSiteHome = "Dalian Pro FC (N)3",
                    AdminSiteAway = "Guangzhou R&F3",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18),
                    OldEventDate = new DateTime(2021, 03, 18)
                }
        };

    private IEnumerable<LinkedMatch> GetLinkedMatches()
        => new List<LinkedMatch>
        {
                new LinkedMatch
                {
                    OSMatchId = 79,
                    OWPLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    OWPHome = "Dalian Pro FC (N)11",
                    OWPAway = "Guangzhou R&F11",
                    OldAdminSiteMatchId = 1,
                    OldEventDate = new DateTime(2021, 03, 18, 20, 30, 0)
                },
                new LinkedMatch
                {
                    OSMatchId = 113,
                    OWPLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    OWPHome = "Dalian Pro FC (N)12",
                    OWPAway = "Guangzhou R&F12",
                    OldAdminSiteMatchId = 2,
                    OldEventDate = new DateTime(2021, 03, 18)
                }
        };

    private IEnumerable<Match> GetOWPMatches()
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
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                },
                new Match
                {
                    OSMatchId = 79,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    HomeName = "Dalian Pro FC (N)11",
                    AwayName = "Guangzhou R&F11",
                    EventDateTime = new DateTime(2021, 03, 18, 20, 30, 0),
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
                    OSMatchId = 114,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)15",
                    HomeName = "Dalian Pro FC (N)12",
                    AwayName = "Guangzhou R&F12",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
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
                    EventDateTime = new DateTime(2021, 03, 18),
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
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18),
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