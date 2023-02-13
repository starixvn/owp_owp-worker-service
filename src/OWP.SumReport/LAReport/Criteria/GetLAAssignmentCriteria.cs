namespace OWP.SumReport.LAReport.Criteria;

using Fanex.Data.Repository;

public class GetLaAssignmentCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_LAAssignmentsSel";

    public override bool IsValid() => true;
}

public class GetOldAndNewLaAssignmentCriteria : CriteriaBase // from old and new table
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_LAAssignmentsOldSel";

    public override bool IsValid() => true;
}