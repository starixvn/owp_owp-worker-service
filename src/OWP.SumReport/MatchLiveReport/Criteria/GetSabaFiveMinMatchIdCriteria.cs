namespace OWP.SumReport.MatchLiveReport.Criteria;

using Fanex.Data.Repository;

public class GetSabaFiveMinMatchIdCriteria : NonQueryCommand
{
    public string Id { get; set; }

    public override string GetSettingKey() => "OWP_MatchLiveReport_GetSabaFiveMinMatchId";

    public override bool IsValid() => true;
}