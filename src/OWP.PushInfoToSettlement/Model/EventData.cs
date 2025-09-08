namespace OWP.PushInfoToSettlement.Model
{
    public class EventData
    {
        public int OWPMatchId { get; set; }
        public int AdminSiteId { get; set; }
        public string Remark { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }
    }
}
