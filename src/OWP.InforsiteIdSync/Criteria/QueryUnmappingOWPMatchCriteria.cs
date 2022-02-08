using Fanex.Data.Repository;

namespace OWP.InfositeIdSync.Criteria;

public class QueryUnmappingOWPMatchCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public override string GetSettingKey() => "OWP_API_QueryUnmappingOWPMatch";

    public override bool IsValid() => true;
}