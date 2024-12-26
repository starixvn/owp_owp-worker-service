namespace OWP.Adminsite.Criteria;

using Fanex.Data.Repository;

public class GetMatchInfoSyncToAdminSiteCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_SyncMatchInfoToAdminSite_GetMatchInfo";

    public override bool IsValid() => true;
}