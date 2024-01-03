namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class GetInfoSiteMatchesCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_API_GetInfoSiteMatches";

    public override bool IsValid() => true;
}