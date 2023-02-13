namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class UpdateSoloMatchCriteria : NonQueryCommand
{
    public override string GetSettingKey() => "OWP_InfoSiteMatch_DetectSoloMatch";

    public override bool IsValid() => true;
}