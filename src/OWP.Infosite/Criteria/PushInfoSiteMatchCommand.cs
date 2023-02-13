namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class PushInfoSiteMatchCommand : NonQueryCommand
{
    public string Xml { get; set; }

    public override string GetSettingKey() => "OWP_API_PushInfoSiteMatch";

    public override bool IsValid() => true;
}