using Fanex.Data.Repository;

namespace OWP.OtherSport.SyncAdminSiteId.Criteria;

public class UpdateUnlinkedMatchesCriteria : NonQueryCommand
{
    public string UnlinkedMatches { get; set; }

    public override string GetSettingKey() => "OWP_OtherSport_UpdateUnlinkedMatches";

    public override bool IsValid() => true;
}