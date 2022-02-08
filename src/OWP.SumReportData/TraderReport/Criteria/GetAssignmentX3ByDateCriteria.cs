using Fanex.Data.Repository;

namespace OWP.SumReportData.TraderReport.Criteria;

public class GetAssignmentX3ByDateCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_LSTReport_ListTraderAssigmmentsX3";
}