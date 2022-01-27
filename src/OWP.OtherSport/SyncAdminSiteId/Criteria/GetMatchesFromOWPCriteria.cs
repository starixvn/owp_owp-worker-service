using Fanex.Data.Repository;

namespace OWP.OtherSport.SyncAdminSiteId.Criteria;

public class GetMatchesFromOWPCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_OtherSport_GetMatchesFromDate";

    public override bool IsValid() => true;
}