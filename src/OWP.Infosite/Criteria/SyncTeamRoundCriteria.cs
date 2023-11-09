namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class SyncTeamRoundCriteria : NonQueryCommand
{
    public string Xml { get; set; }

    public override string GetSettingKey() => "OWP_API_SyncTeamRound";

    public override bool IsValid() => true;
}