using Fanex.Data.Repository;

namespace OWP.SumReportData.OvertimeReport.Criteria;

public class GetEmployeeWorkingShiftCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_GetEmployeeWorkingShift";

    public override bool IsValid() => true;
}