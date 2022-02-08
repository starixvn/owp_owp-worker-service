namespace OWP.PrematchReport.Services.League
{
    public interface OULeagueService
    {
        void BuildOULeagueReport(IList<Models.League> leagues);
    }

    public class OULeagueImp : OULeagueService
    {
        public void BuildOULeagueReport(IList<Models.League> leagues)
        {

        }
    }
}