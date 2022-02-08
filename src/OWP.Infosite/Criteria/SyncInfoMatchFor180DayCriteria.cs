using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class SyncInfoMatchFor180DayCriteria : NonQueryCommand
{
    public string InfoSiteMatchResultDoc { get; set; }

    public override string GetSettingKey() => "OWP_API_SyncFromInfoSiteMatch180Day";

    public override bool IsValid() => true;
}