namespace OWP.InfositeIdSync.Criteria;

using Fanex.Data.Repository;

public class QueryInfositeMatchCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public override string GetSettingKey() => "OWP_API_QueryInfositeMatch";

    public override bool IsValid() => true;
}