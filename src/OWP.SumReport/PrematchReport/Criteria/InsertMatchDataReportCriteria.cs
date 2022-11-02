namespace OWP.SumReport.PrematchReport.Criteria;

public class InsertMatchDataReportCriteria : InsertDataReportBaseCriteria
{
    public override string GetSettingKey() => "OWP_PrematchReport_InsertMatch";

    public override bool IsValid() => true;
}