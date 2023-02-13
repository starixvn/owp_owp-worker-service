namespace OWP.Infosite.UnitTests;

using Criteria;
using Fanex.Data.Repository;
using Models;
using NSubstitute;
using Services;
using Xunit;
using Xunit.Asserts.Compare;

public class UpdateInfositeIdServiceUnitTests
{
    private readonly UpdateInfositeIdServiceImp services;

    private readonly ApiService apiService;

    private readonly IDynamicRepository repository;

    public UpdateInfositeIdServiceUnitTests()
    {
        apiService = Substitute.For<ApiService>();
        repository = Substitute.For<IDynamicRepository>();
        services = new UpdateInfositeIdServiceImp(apiService, repository);
    }

    [Fact]
    public async Task QueryMergedMatchEvents_ReturnListMatchRelationEvent()
    {
        var matchRelations = new List<MatchRelation>
            {
                new MatchRelation { match_id = 111, parent_match_id = 222 },
                new MatchRelation{ match_id = 200, parent_match_id = 200 }
            };

        apiService.QueryMatchesRelation(Arg.Compat.Any<ParamsQueryModel>()).Returns(matchRelations);

        var expected = new List<MatchRelationEvent> { new MatchRelationEvent(matchRelations[0]) };

        var actual = await services.QueryMergedMatchEvents(new DateTime(2021, 04, 29));

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public async Task UpdateInfositeId_ReturnListMatchRelationEvent()
    {
        var matchRelations = new List<MatchRelation>
            {
                new MatchRelation { match_id = 111, parent_match_id = 222 },
                new MatchRelation{ match_id = 200, parent_match_id = 200 }
            };

        apiService.QueryMatchesRelation(Arg.Compat.Any<ParamsQueryModel>()).Returns(matchRelations);

        await services.UpdateInfositeId(new DateTime(2021, 04, 29));

        await repository.Received(1).ExecuteAsync(Arg.Compat.Any<UpdateInfositeIdCommand>());
    }
}