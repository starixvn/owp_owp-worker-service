namespace OWP.InfositeIdSync.Criteria;

using Fanex.Data.Repository;

public class QueryUnmappingOWPMatchCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public override string GetSettingKey() => "OWP_API_QueryUnmappingOWPMatch";

    public override bool IsValid() => true;
}