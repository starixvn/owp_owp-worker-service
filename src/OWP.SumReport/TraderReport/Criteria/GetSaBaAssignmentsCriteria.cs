using Fanex.Data.Repository;

namespace OWP.SumReport.TraderReport.Criteria;

public class GetSaBaAssignmentsCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_TraderReport_GetSaBaAssignments";
}

public class GetOldAndNewSaBaAssignmentsCriteria : CriteriaBase // from old and new table
{
    public DateTime EventDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_TraderReport_GetOldAndNewSaBaAssignments";
}