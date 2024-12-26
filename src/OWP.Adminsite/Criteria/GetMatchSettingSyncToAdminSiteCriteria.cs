namespace OWP.Adminsite.Criteria;

using Fanex.Data.Repository;

public class GetMatchSettingSyncToAdminSiteCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    public override string GetSettingKey() => "OWP_SyncMatchInfoToAdminSite_GetMatchSetting";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchIds);
}