namespace OWP.AutoGenerateMaxbetSetting;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

public interface LeagueSettingsRepository
{
    Task<Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>> GetMatchesAndSettings(DateTime eventDate);

    Task InsertNewLeaguesSettings(List<LeagueSetting> leaguesSettings);

}

public class LeagueSettingsRepositoryImp(IDynamicRepository dynamicRepository) : LeagueSettingsRepository
{
    public async Task<Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>> GetMatchesAndSettings(DateTime eventDate)
    {
        var criteria = new GetMatchesCriteria
        {
            EventDate = eventDate
        };

        var result = await dynamicRepository.FetchMultipleAsync<Match, MatchSetting>(criteria);

        return new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(result.Item1, result.Item2);
    }


    public async Task InsertNewLeaguesSettings(List<LeagueSetting> leaguesSettings)
    {
        var criteria = new InsertNewLeagueSettingsCriteria
        {
            LeagueSettings = XmlConverter.Serialize(leaguesSettings)
        };

        await dynamicRepository.ExecuteAsync(criteria);
    }
}