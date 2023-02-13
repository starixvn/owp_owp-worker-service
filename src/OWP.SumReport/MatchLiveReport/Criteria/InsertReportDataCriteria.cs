namespace OWP.SumReport.MatchLiveReport.Criteria;

using Fanex.Data.Repository;

public class InsertReportDataCriteria : NonQueryCommand
{
    public string Data { get; set; }

    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_InsertReportData";

    public override bool IsValid() => true;
}