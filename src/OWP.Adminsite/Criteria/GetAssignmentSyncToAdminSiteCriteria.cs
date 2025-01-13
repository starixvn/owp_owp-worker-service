namespace OWP.Adminsite.Criteria;

using Fanex.Data.Repository;

public class GetAssignmentSyncToAdminSiteCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    public override string GetSettingKey() => "OWP_SyncMatchInfoToAdminSite_GetAssignment";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchIds);
}