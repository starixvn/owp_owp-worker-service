namespace OWP.SumReport.PrematchReport.Criteria;

public class InsertLeagueDataReportCriteria : InsertDataReportBaseCriteria
{
    public override string GetSettingKey() => "OWP_PrematchReport_InsertLeague";

    public override bool IsValid() => true;
}