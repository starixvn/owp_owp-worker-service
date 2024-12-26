namespace OWP.Adminsite.Criteria;

using Fanex.Data.Repository;

public class GetMatchOfferSyncToAdminSiteCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    public override string GetSettingKey() => "OWP_SyncMatchInfoToAdminSite_GetMatchOffer";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchIds);
}