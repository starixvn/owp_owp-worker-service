namespace OWP.Infosite.Models;

public class InfoSiteDb
{
    public int InfoSiteMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public string Tv { get; set; }

    public string IPTV { get; set; }

    public string OtherProvider { get; set; }

    public string Round { get;  set; }

    public string Remark { get; set; }
    public string Remark2 { get; set; }

    public string HomeRemark { get; set; }

    public string AwayRemark { get; set; }

    public string Division { get;  set; }

    public byte? MatchStatus { get; set; }

    public string RoundConfirm { get; set; }

    public int AdminSiteId { get; set; }

    public string Website1 { get; set; }

    public string Website2 { get; set; }

    public string Website3 { get; set; }

    public string Website4 { get; set; }

    public string WatchBet { get; set; }

    public string Channel { get; set; }

    public string Agent { get; set; }

    public string Venue { get; set; }

    public string Etpk { get; set; }

    public bool IsNeutral { get; set; }
    public bool IsPostponed { get; set; }
}

public class InfoSiteDbInfo : InfoSiteDb
{
   public bool IsCheck { get; set; }
}