using Fanex.Data.Repository;

namespace OWP.SumReportData.MatchLiveReport.Criteria;

public class InsertReportDataCriteria : NonQueryCommand
{
    public string ReportData { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_InsertReportData";

    public override bool IsValid() => true;
}