namespace OWP.OtherSport.SyncAdminSiteId.Criteria;

using Fanex.Data.Repository;

public class GetMatchesFromAdminSiteCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_OtherSport_GetAdminSiteMatchesFromDate";

    public override bool IsValid() => true;
}