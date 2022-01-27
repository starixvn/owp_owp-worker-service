using Fanex.Data.Repository;

namespace OWP.SumReportData.OvertimeReport.Criteria;

public class SaveOvertimeReportCriteria : NonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string Data { get; set; }

    public override string GetSettingKey() => "OWP_OvertimeReport_SaveOvertimeReport";

    public override bool IsValid() => true;
}