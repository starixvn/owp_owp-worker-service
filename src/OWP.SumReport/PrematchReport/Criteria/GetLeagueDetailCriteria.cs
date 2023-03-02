namespace OWP.SumReport.PrematchReport.Criteria;

using Fanex.Data.Repository;

public class GetLeagueDetailCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public int Bettype { get; set; }

    public override string GetSettingKey() => "OWP_PrematchReport_GetLeagueDetailByDateAndBettype";

    public override bool IsValid() => true;
}