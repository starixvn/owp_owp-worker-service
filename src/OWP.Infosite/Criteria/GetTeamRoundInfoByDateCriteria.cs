using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class GetTeamRoundInfoByDateCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_API_GetTeamRoundInfosByDate";

    public override bool IsValid() => true;
}