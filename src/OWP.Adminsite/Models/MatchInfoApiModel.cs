namespace OWP.Adminsite.Models;

[Serializable]
public class MatchInfoApiModel
{
    public int MatchId { get; set; }
    public int? SubType { get; set; }
    public string Provider { get; set; }
    public string OtherProvider { get; set; }
    public string WatchBet { get; set; }
    public int? SecondAcceptValue { get; set; }
    public int? CreditFtLines { get; set; }
    public int? CreditFirstHalfLines { get; set; }
    public int? LicenseeFtLines { get; set; }
    public int? LicenseeFirstHalfLines { get; set; }
    public int? SuspenderUserId { get; set; }
    public string SuspenderUsername { get; set; }
    public IList<AssignmentApiModel> Traders { get; set; } = [];
    public IList<MatchSettingApiModel> MatchSettings { get; set; } = [];
}