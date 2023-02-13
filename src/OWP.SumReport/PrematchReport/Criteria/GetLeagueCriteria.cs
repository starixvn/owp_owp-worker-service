namespace OWP.SumReport.PrematchReport.Criteria;

using Fanex.Data.Repository;

public class GetLeagueCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public int Bettype { get; set; }

    public override string GetSettingKey() => "OWP_PrematchReport_GetLeagueByDateAndBettype";

    public override bool IsValid() => true;
}