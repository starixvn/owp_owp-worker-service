namespace OWP.LinkLeagueAndTeam.Criteria;

using Fanex.Data.Repository;

public class GetTeamInfoInTimeRangeCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_LinkLeagueAndTeam_GetTeamInfo_InTimeRange";

    public override bool IsValid() => true;
}