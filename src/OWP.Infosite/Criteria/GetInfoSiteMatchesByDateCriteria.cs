namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class GetInfoSiteMatchesByDateCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_API_GetInfoSiteMatchesByDate";

    public override bool IsValid() => true;
}