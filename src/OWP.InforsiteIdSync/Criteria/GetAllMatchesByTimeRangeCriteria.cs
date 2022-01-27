using Fanex.Data.Repository;

namespace OWP.InfositeIdSync.Criteria;

public class GetAllMatchesByTimeRangeCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_AssignmentUpload_AllMatchesByTimeRange";

    public override bool IsValid() => true;
}