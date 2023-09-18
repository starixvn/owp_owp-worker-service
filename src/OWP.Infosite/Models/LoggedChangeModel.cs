namespace OWP.Infosite.Models;

public class LoggedChangeModel
{
    public int InfoSiteMatchId { get; set; }

    public DateTime UpDateTime { get; set; }

    public int? UpdateTypeId { get; set; }

    public string  Action { get; set; }

    public string NewValue { get; set; }

    public string OldValue { get; set; }
}

