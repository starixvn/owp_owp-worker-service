using Fanex.Data.Repository;

namespace OWP.PrematchReport.Criteria
{
    public class GetLeagueCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "OWP_PrematchReport_GetLeagueByDate";

        public override bool IsValid() => true;
    }
}