namespace OWP.InfositeIdSync.Criteria;

using Fanex.Data.Repository;

public class GetMatchesFromMainDBCriteria : CriteriaBase
{
    public string AdminSiteIds { get; set; }

    public override string GetSettingKey() => "OWP_LiveGameAssignment_GetMatchStatusFromMainDB";

    public override bool IsValid() => true;
}