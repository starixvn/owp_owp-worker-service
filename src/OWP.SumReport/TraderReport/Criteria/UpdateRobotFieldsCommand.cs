using Fanex.Data.Repository;

namespace OWP.SumReport.TraderReport.Criteria;

public class UpdateRobotFieldsCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string Xml { get; set; }

    public string GetSettingKey() => "OWP_TraderReportUpdateRobotFields";

    public bool IsValid() => true;
}