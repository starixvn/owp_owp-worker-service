using Fanex.Data.Repository;

namespace OWP.SumReportData.TraderReport.Criteria;

public class GetAssignmentX5ByDateCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_LSTReport_ListTraderAssigmmentsX5";
}

public class GetOldAndNewAssignmentX5ByDateCriteria : CriteriaBase // from old and new table
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_LSTReport_OldAndNewListTraderAssigmmentsX5";
}