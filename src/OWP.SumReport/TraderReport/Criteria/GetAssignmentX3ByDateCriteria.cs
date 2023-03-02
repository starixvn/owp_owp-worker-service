namespace OWP.SumReport.TraderReport.Criteria;

using Fanex.Data.Repository;

public class GetAssignmentX3ByDateCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_LSTReport_ListTraderAssignmentsX3";
}

public class GetOldAndNewAssignmentX3ByDateCriteria : CriteriaBase // from old and new table
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "OWP_LSTReport_OldAndNewListTraderAssignmentsX3";
}