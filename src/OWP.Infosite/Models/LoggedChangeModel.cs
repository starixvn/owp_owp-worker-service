namespace OWP.Infosite.Models;

public class LoggedChangeModel
{
    public int InfoSiteMatchId { get; set; }

    public DateTime UpDateTime { get; set; }

    public int Action { get; set; }

    public string NewValue { get; set; }

    public string OldValue { get; set; }
}

