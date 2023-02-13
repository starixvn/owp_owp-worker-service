namespace OWP.InfositeIdSync.Criteria;

using Fanex.Data.Repository;

public class GetAllMatchesByTimeRangeCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_AssignmentUpload_AllMatchesByTimeRange";

    public override bool IsValid() => true;
}