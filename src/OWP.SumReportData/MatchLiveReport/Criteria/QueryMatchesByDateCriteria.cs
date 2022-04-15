using Fanex.Data.Repository;

namespace OWP.SumReportData.MatchLiveReport.Criteria;

public class QueryMatchesByDateCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_QueryMatchesByDate";
    
    public override bool IsValid() => true;
}