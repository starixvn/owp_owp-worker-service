using Fanex.Data.Repository;

namespace OWP.SumReport.OvertimeReport.Criteria;

public class GetTradersAssignmentsCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public string UserIds { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_GetTradersAssignments";

    public override bool IsValid() => true;
}

public class GetGetOldAndNewTradersAssignmentsCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public string UserIds { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_GetOldAndNewTradersAssignments";

    public override bool IsValid() => true;
}
