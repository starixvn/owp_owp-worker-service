namespace OWP.InfositeIdSync.Criteria;

using Fanex.Data.Repository;

public class QueryUnmappingOwpMatchCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public override string GetSettingKey() => "OWP_API_QueryUnmappingOWPMatch";

    public override bool IsValid() => true;
}