using Fanex.Data.Repository;

namespace OWP.SumReport.LAReport.Criteria;

public class GetLAAssignmentCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_LAAssignmentsSel";

    public override bool IsValid() => true;
}

public class GetOldAndNewLAAssignmentCriteria : CriteriaBase // from old and new table
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_LAAssignmentsOldSel";

    public override bool IsValid() => true;
}