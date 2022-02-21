using Fanex.Data.Repository;
using Newtonsoft.Json;
using Olympus.Xml;
using OWP.SyncLeagueSettings.Criteria;
using OWP.SyncLeagueSettings.Models;

namespace OWP.SyncLeagueSettings;

public interface LeagueSettingsRepository
{
    Task<Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>> GetMatchesAndSettings(DateTime eventDate);

    Task<IEnumerable<TvCondition>> GetAllTvConditions();

    Task<IEnumerable<InsertLeagueSettingsResponse>> InsertNewLeaguesSettings(List<LeagueSetting> leaguesSettings);

    Task UpdateLeagueSettingFullCondition(LeagueSettingCondition model);
}

public class LeagueSettingsRepositoryImp : LeagueSettingsRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public LeagueSettingsRepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>> GetMatchesAndSettings(DateTime eventDate)
    {
        var criteria = new GetMatchesCriteria
        {
            EventDate = eventDate
        };

        var result = await dynamicRepository.FetchMultipleAsync<Match, MatchSetting>(criteria);

        return new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(result.Item1, result.Item2);
    }

    public async Task<IEnumerable<TvCondition>> GetAllTvConditions()
       => await dynamicRepository.FetchAsync<TvCondition>(new GetTVListCriteria());

    public async Task<IEnumerable<InsertLeagueSettingsResponse>> InsertNewLeaguesSettings(List<LeagueSetting> leaguesSettings)
    {
        var criteria = new InsertNewLeagueSettingsCriteria
        {
            LeagueSettings = XmlConverter.Serialize(leaguesSettings)
        };

        return await dynamicRepository.FetchAsync<InsertLeagueSettingsResponse>(criteria);
    }

    public async Task UpdateLeagueSettingFullCondition(LeagueSettingCondition model)
    {
        var criteria = new UpdateConditionCriteria
        {
            ActorId = 0,
            LeagueSettingId = model.LeagueSettingId,
            HasTV = model.HasTV,
            FullConditionDescription = JsonConvert.SerializeObject(model)
        };

        await dynamicRepository.ExecuteAsync(criteria);
    }
}