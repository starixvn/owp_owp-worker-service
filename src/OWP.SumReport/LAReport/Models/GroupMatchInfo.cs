namespace OWP.SumReport.LAReport.Models
{
    public class GroupMatchInfo
    {
        public IGrouping<int,Assignment> LinkGroup { get; set; }

        public int MatchCount { get; set; }

        public IList<int> OwpMatchId { get; set; }
    }
}
