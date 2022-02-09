using Fanex.Data.Repository;
using System;

namespace OWP.PrematchReport.Criteria
{
    public class BuildReportIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "OWP_PrematchReport_BuildReportId";

        public override bool IsValid() => true;
    }
}