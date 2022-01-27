using Fanex.Data.Repository;

namespace OWP.SumReportData.OvertimeReport.Criteria;

public class GetTradersAssignmentsCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public string UserIds { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_GetTradersAssignments";

    public override bool IsValid() => true;
}