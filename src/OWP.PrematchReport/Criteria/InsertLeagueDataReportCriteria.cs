namespace OWP.PrematchReport.Criteria
{
    public class InsertLeagueDataReportCriteria : InsertDataReportCriteriaBase
    {
        public override string GetSettingKey() => "OWP_PrematchReport_InsertLeague";

        public override bool IsValid() => true;
    }
}