using Fanex.Data.Repository;

namespace OWP.LinkLeagueAndTeam.Criteria;

public class InsertOrUpdateCriteria : INonQueryCommand
{
    public int LeagueId { get; set; }

    public int TeamId { get; set; }

    public bool IsValid() => true;

    public string GetSettingKey()
        => "OWP_LinkLeagueAndTeam_InsertOrUpdate";
}