using Fanex.Data.Repository;

namespace OWP.SumReportData.TraderReport.Criteria;

public class UpdateESoccerFieldsCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string ESoccerFieldsXml { get; set; }

    public string GetSettingKey() => "OWP_TraderReport_UpdateESoccerFields";

    public bool IsValid() => true;
}