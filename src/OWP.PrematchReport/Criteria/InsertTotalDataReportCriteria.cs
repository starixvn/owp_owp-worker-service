namespace OWP.PrematchReport.Criteria
{
    public class InserttotalDataReportCriteria : InsertDataReportCriteriaBase
    {
        public override string GetSettingKey() => "OWP_PrematchReport_InsertTotal";

        public override bool IsValid() => true;
    }
}