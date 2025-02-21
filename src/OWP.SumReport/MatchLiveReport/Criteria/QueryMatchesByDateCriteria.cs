namespace OWP.SumReport.MatchLiveReport.Criteria;

using Fanex.Data.Repository;

public class QueryMatchesByDateCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_QueryMatchesByDate";

    public override bool IsValid() => true;
}

public class QueryFullMatchesByDateCriteria : CriteriaBase // from old and new table
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_QueryOldMatchesByDate";

    public override bool IsValid() => true;
}