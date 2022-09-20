using Fanex.Data.Repository;

namespace OWP.SumReportData.LAReport.Criteria;

public class SaveOtherProductMatchIdsCommand : INonQueryCommand
{
    public DateTime EventDate { get; set; }

    public string OWPMatchIds { get; set; }

    public string GetSettingKey() => "OWP_LAReport_SaveOtherProductMatchIds";

    public bool IsValid() => true;
}