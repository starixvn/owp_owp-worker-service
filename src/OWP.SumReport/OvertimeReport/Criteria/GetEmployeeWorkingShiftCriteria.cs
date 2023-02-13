namespace OWP.SumReport.OvertimeReport.Criteria;

using Fanex.Data.Repository;

public class GetEmployeeWorkingShiftCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_GetEmployeeWorkingShift";

    public override bool IsValid() => true;
}

public class GetOldAndNewEmployeeWorkingShiftCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_GetOldAndNewEmployeeWorkingShift";

    public override bool IsValid() => true;
}