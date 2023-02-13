namespace OWP.MatchReadyNotification.Criteria;

using Fanex.Data.Repository;

internal class GetLeaderSupervisorSectionHeadOfSessionCriteria : CriteriaBase
{
    public string UserIds { get; set; }

    public override string GetSettingKey() => "OWP_GetLeaderSupervisorSectionHeadOfSession";

    public override bool IsValid() => true;
}