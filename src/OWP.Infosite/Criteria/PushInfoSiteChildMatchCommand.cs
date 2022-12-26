using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class PushInfoSiteChildMatchCommand : NonQueryCommand
{
    public string ChildMatchesJson { get; set; }

    public override string GetSettingKey() => "OWP_API_PushInfoSiteChildMatches";

    public override bool IsValid() => true;
}