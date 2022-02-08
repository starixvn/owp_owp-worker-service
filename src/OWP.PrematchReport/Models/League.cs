namespace OWP.PrematchReport.Models
{
    public class League
    {
        public DateTime EventDate { get; set; }

        public int OWPLeagueId { get; set; }

        public int UserId { get; set; }

        public int Bettype { get; set; }

        public bool IsTakeOver { get; set; }

        public int AssignmentType { get; set; }
    }
}