namespace OWP.PrematchReport.Criteria
{
    using Fanex.Data.Repository;

    public class GetTotalMatchCriteria : CriteriaBase
    {
        public string LeagueIds { get; set; }

        public DateTime FromDateTime { get; set; }

        public DateTime ToDateTime { get; set; }

        public DateTime FromDateTimeNightShift { get; set; }

        public override string GetSettingKey()
         => "OWP_PreMatchReport_GetToTalMatch";

        public override bool IsValid()
         => true;
    }
}