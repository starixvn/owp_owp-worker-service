using Fanex.Data.Repository;

namespace OWP.SumReportData.MatchLiveReport.Criteria;

public class GetSabaFiveMinMatchIdCriteria : NonQueryCommand
{
    public string Id { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_GetSabaFiveMinMatchId";

    public override bool IsValid() => true;
}