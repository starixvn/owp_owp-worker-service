namespace OWP.SumReport.PrematchReport.Criteria;

public class InsertMatchDataReportCriteria : InsertDataReportCriteriaBase
{
    public override string GetSettingKey() => "OWP_PrematchReport_InsertMatch";

    public override bool IsValid() => true;
}