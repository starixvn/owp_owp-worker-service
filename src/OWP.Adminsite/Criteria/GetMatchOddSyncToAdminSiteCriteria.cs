namespace OWP.Adminsite.Criteria;

using Fanex.Data.Repository;

public class GetMatchOddSyncToAdminSiteCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    public override string GetSettingKey() => "OWP_SyncMatchInfoToAdminSite_GetMatchOdd";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchIds);
}