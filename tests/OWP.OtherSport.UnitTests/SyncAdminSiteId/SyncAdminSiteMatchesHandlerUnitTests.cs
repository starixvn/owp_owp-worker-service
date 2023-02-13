namespace OWP.OtherSport.UnitTests.SyncAdminSiteId;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using NSubstitute;
using Olympus.Xml;
using OtherSport.SyncAdminSiteId.Criteria;
using OtherSport.SyncAdminSiteId.Handlers;
using OtherSport.SyncAdminSiteId.Models;
using OtherSport.SyncLive;
using OtherSport.SyncLive.Services;
using Xunit;

public class SyncAdminSiteMatchesHandlerUnitTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly SyncAdminSiteMatchesHandlerImp service;

    public SyncAdminSiteMatchesHandlerUnitTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        var syncLiveService = Substitute.For<SyncLiveToAdminSiteHandler>();
        var cacheManipulator = Substitute.For<CacheManipulator>();

        dynamicRepository.FetchAsync<Match>(Arg.Any<GetMatchesFromOwpCriteria>()).Returns(GetOWPMatches());
        dynamicRepository.FetchAsync<Match>(Arg.Any<GetMatchesFromAdminSiteCriteria>()).Returns(GetAdminSiteMatches());

        service = new SyncAdminSiteMatchesHandlerImp(dynamicRepository, syncLiveService, cacheManipulator);
    }

    [Fact]
    public async Task Handle_WithValidEventDate_UpdateMatches()
    {
        await service.Handle(new DateTime());

        await dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<UpdateUnlinkedMatchesCriteria>(u
                => u.UnlinkedMatches == XmlConverter.Serialize(GetUnlinkedMatches())));
    }

    private IEnumerable<LinkedMatch> GetUnlinkedMatches()
        => new List<LinkedMatch>
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
                    NewEventDate = new DateTime(2021, 03, 18),
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
                    OwpLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    OwpHome = "Dalian Pro FC (N)3",
                    OwpAway = "Guangzhou R&F3",
                    AdminSiteLeague = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    AdminSiteHome = "Dalian Pro FC (N)3",
                    AdminSiteAway = "Guangzhou R&F3",
                    OldAdminSiteMatchId = null,
                    NewEventDate = new DateTime(2021, 03, 18),
                    OldEventDate = new DateTime(2021, 03, 18)
                }
        };

    private IEnumerable<Match> GetOWPMatches()
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
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18),
                    AdminSiteMatchId = null
                },
                new()
                {
                    OsMatchId = 79,
                    Sport = "Netball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)11",
                    HomeName = "Dalian Pro FC (N)11",
                    AwayName = "Guangzhou R&F11",
                    EventDateTime = new DateTime(2021, 03, 18, 20, 05, 0),
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
                    OsMatchId = 114,
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
                new()
                {
                    Sport = "Basketball",
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)1",
                    HomeName = "Dalian Pro FC (N)1",
                    AwayName = "Guangzhou R&F1",
                    EventDateTime = new DateTime(2021, 03, 18),
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
                    LeagueName = "CHINA FOOTBALL SUPER LEAGUE (PLAY OFF)3",
                    HomeName = "Dalian Pro FC (N)3",
                    AwayName = "Guangzhou R&F3",
                    EventDateTime = new DateTime(2021, 03, 18),
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