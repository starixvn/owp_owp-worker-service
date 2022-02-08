using Fanex.Data.Repository;

namespace OWP.SumReportData.TraderReport.Criteria;

public class UpdateNonRobotFieldsCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string GetSettingKey() => "OWP_TraderReportUpdateNonRobotFields";

    public bool IsValid() => true;
}