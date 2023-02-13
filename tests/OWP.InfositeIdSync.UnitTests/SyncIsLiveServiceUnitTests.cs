using Microsoft.Extensions.Options;
using NSubstitute;
using Olympus.InfoSiteClient;
using OWP.Infosite.Settings;
using OWP.InfositeIdSync.Models;
using OWP.InfositeIdSync.Services;
using Xunit;

namespace OWP.InfositeIdSync.UnitTests;

public class SyncIsLiveServiceUnitTests
{
    [Fact]
    public async Task Sync_OnlySetIsLiveForNewMatches_OnlySetNonLiveForDeletedMatches()
    {
        // Arrange
        var repository = Substitute.For<InfositeIdMappingRepository>();

        var syncIsLiveModels = new List<SyncIsLiveModel>
            {
                new SyncIsLiveModel { InfoSiteMatchId = 1, IsLive = true, MinorBetTypeId = -2, AdminSiteMatchId = 200 },
                new SyncIsLiveModel { InfoSiteMatchId = 1, IsLive = true, MinorBetTypeId = null, AdminSiteMatchId = 200 },
                new SyncIsLiveModel { InfoSiteMatchId = 2, IsLive = false, MinorBetTypeId = null, AdminSiteMatchId = 300 }
            };
        repository.GetInfositeMatchIsLive(DateTime.Today, DateTime.Today).Returns(syncIsLiveModels);

        var mainDbMatches = new List<MainDBMatch>
            {
                new MainDBMatch { MatchId = 202, OfferLive = true, SubType = 1900, ParentId = 200 },
                new MainDBMatch { MatchId = 201, OfferLive = false, SubType = 1910, ParentId = 200 },
                new MainDBMatch { MatchId = 200, OfferLive = true, SubType = null, ParentId = null },
                new MainDBMatch { MatchId = 300, OfferLive = true, SubType = null, ParentId = null }
            };
        repository.GetOfferLiveStatusFromMainDB(Arg.Any<string>()).Returns(mainDbMatches);

        var cacheManipulator = Substitute.For<CacheManipulator>();
        cacheManipulator.GetDeletedMatches().Returns(new List<MatchKickoffDate> { new MatchKickoffDate { MatchId = 3, MinorBettypeId = 1 } });

        var emptySynchronizedResult = new IsLiveSynchronizedResult
        {
            ErrorMessage = string.Empty,
            FailedMatches = new List<(int, int?)>()
        };
        var infositeSettings = Options.Create(new InfoSiteSettings());
        var liveMatchApi = Substitute.For<IsLiveMatchApi>();
        liveMatchApi.SetLiveMatches(infositeSettings.Value.InfositeAPI, Arg.Compat.Any<IEnumerable<(int, int?)>>())
                    .Returns(emptySynchronizedResult);
        liveMatchApi.SetNonLiveMatches(infositeSettings.Value.InfositeAPI, Arg.Compat.Any<IEnumerable<(int, int?)>>())
                    .Returns(emptySynchronizedResult);

        var logService = Substitute.For<LogServiceWrapper>();

        var syncIsLiveService = new SyncIsLiveServiceImp(repository, logService, cacheManipulator, liveMatchApi, infositeSettings);

        // Act
        await syncIsLiveService.Sync(DateTime.Today, DateTime.Today);

        // Assert
        await liveMatchApi.Received().SetLiveMatches(infositeSettings.Value.InfositeAPI, Arg.Compat.Is<IEnumerable<(int, int?)>>(
            x => x.Count() == 1 && x.FirstOrDefault().Item1 == 1 && x.FirstOrDefault().Item2 == 1900));
        await liveMatchApi.Received().SetNonLiveMatches(infositeSettings.Value.InfositeAPI, Arg.Compat.Is<IEnumerable<(int, int?)>>(
            x => x.Count() == 2 && x.FirstOrDefault().Item1 == 2 && x.LastOrDefault().Item1 == 3));
        await cacheManipulator.Received().SaveDeletedMatches(Arg.Compat.Any<List<(int, int?)>>());
    }
}