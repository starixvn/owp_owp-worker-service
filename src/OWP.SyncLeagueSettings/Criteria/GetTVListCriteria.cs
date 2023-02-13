namespace OWP.SyncLeagueSettings.Criteria;

using Fanex.Data.Repository;

public class GetTVListCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_LeagueManagement_GetAllTVCondition";

    public override bool IsValid() => true;
}