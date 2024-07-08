namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class GetExistingInfoSiteMatchesCriteria : CriteriaBase
{
    public string Ids { get; set; }

    public override string GetSettingKey() => "OWP_API_GetExistingInfoSiteMatches";

    public override bool IsValid() => true;
}