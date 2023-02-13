namespace OWP.SyncLeagueSettings.Models;

using Attributes;

[XmlName("LeagueSetting")]
public class LeagueSetting
{
    [XmlName("OWPLeagueId")] public int OWPLeagueId { get; set; }

    [XmlName("HasTV")] public bool HasTv { get; set; }

    [XmlName("MaxbetWD_1H")] public decimal? MaxbetWD_1H { get; set; }

    [XmlName("MaxbetWD_HDP")] public decimal? MaxbetWD_HDP { get; set; }

    [XmlName("MaxbetWD_OU")] public decimal? MaxbetWD_OU { get; set; }

    [XmlName("MaxbetWK_1H")] public decimal? MaxbetWK_1H { get; set; }

    [XmlName("MaxbetWK_HDP")] public decimal? MaxbetWK_HDP { get; set; }

    [XmlName("MaxbetWK_OU")] public decimal? MaxbetWK_OU { get; set; }

    [XmlName("Spread_1HHDP")] public decimal? Spread_1HHDP { get; set; }

    [XmlName("Spread_1HOU")] public decimal? Spread_1HOU { get; set; }

    [XmlName("Spread_HDP")] public decimal? Spread_HDP { get; set; }

    [XmlName("Spread_OU")] public decimal? Spread_OU { get; set; }
}