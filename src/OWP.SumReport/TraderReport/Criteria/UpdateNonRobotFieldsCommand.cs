namespace OWP.SumReport.TraderReport.Criteria;

using Fanex.Data.Repository;

public class UpdateNonRobotFieldsCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string GetSettingKey() => "OWP_TraderReportUpdateNonRobotFields";

    public bool IsValid() => true;
}