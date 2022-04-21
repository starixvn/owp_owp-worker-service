namespace OWP.OtherSport.SyncLive.Model
{
    public class BodyModel
    {
        public string From { get; set; }

        public int UserId { get; set; }

        public List<EditEvent> EditEventList { get; set; }
    }
}
