using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class SyncEventDeletedInfoSiteMatchCriteria : NonQueryCommand
{
    public string Ids { get; set; }

    public override string GetSettingKey() => "OWP_API_SyncEventDeletedInfoSiteMatch";

    public override bool IsValid() => true;
}