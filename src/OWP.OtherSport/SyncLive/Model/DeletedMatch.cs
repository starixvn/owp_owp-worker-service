namespace OWP.OtherSport.SyncLive.Model;

public class DeletedMatch
{
    public int OSMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    public string Sport { get; set; }

    public DateTime EventDate { get; set; }
}