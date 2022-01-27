using Fanex.Data.Repository;

namespace OWP.SumReportData.LAReport.Criteria;

public class GetLAAssignmentCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "OWP_LAAssignmentsSel";

    public override bool IsValid() => true;
}