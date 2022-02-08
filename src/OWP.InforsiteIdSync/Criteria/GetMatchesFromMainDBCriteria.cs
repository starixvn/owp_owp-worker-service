using Fanex.Data.Repository;

namespace OWP.InfositeIdSync.Criteria;

public class GetMatchesFromMainDBCriteria : CriteriaBase
{
    public string AdminSiteIds { get; set; }

    public override string GetSettingKey() => "OWP_LiveGameAssignment_GetMatchStatusFromMainDB";

    public override bool IsValid() => true;
}