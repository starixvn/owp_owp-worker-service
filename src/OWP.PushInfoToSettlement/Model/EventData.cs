namespace OWP.PushInfoToSettlement.Model
{
    public class EventData
    {
        public int OWPMatchId { get; set; }
        public int AdminSiteMatchId { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}
