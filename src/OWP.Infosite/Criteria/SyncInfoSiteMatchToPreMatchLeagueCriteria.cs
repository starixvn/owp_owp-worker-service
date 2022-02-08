using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class SyncInfoSiteMatchToPreMatchLeagueCriteria : NonQueryCommand
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_PreMatchAssignment_SyncFromInfoSiteMatch";

    public override bool IsValid() => true;
}