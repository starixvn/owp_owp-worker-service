namespace OWP.OtherSport.SyncAdminSiteId.Criteria;

using Fanex.Data.Repository;

public class UpdateLinkedMatchesCriteria : NonQueryCommand
{
    public string LinkedMatches { get; set; }

    public override string GetSettingKey() => "OWP_OtherSport_UpdateLinkedMatches";

    public override bool IsValid() => true;
}