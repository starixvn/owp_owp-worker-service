namespace OWP.SyncLeagueSettings.Criteria;

using Fanex.Data.Repository;

public class GetTvListCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_LeagueManagement_GetAllTVCondition";

    public override bool IsValid() => true;
}