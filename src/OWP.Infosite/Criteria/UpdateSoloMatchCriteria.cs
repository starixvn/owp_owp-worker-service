using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class UpdateSoloMatchCriteria : NonQueryCommand
{
    public override string GetSettingKey() => "OWP_InfoSiteMatch_DetectSoloMatch";

    public override bool IsValid() => true;
}