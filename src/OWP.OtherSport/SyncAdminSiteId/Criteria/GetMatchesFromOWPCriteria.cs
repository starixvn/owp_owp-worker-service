namespace OWP.OtherSport.SyncAdminSiteId.Criteria;

using Fanex.Data.Repository;

public class GetMatchesFromOwpCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_OtherSport_GetMatchesFromDate";

    public override bool IsValid() => true;
}