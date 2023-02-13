namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class SyncInfoMatchFor180DayCriteria : NonQueryCommand
{
    public string InfoSiteMatchResultDoc { get; set; }

    public override string GetSettingKey() => "OWP_API_SyncFromInfoSiteMatch180Day";

    public override bool IsValid() => true;
}