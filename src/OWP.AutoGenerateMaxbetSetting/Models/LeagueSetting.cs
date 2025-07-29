namespace OWP.AutoGenerateMaxbetSetting.Models;

using Olympus.Xml;
public class LeagueSetting
{
    [XmlName("OWPLeagueId")] public int OwpLeagueId { get; set; }

    [XmlName("HasTV")] public bool? HasTv { get; set; }

    [XmlName("MaxbetWD_1H")] public decimal? MaxbetWd1H { get; set; }

    [XmlName("MaxbetWD_HDP")] public decimal? MaxbetWdHdp { get; set; }

    [XmlName("MaxbetWD_OU")] public decimal? MaxbetWdOu { get; set; }

    [XmlName("MaxbetWK_1H")] public decimal? MaxbetWk1H { get; set; }

    [XmlName("MaxbetWK_HDP")] public decimal? MaxbetWkHdp { get; set; }

    [XmlName("MaxbetWK_OU")] public decimal? MaxbetWkOu { get; set; }

    [XmlName("Spread_1HHDP")] public decimal? Spread1Hhdp { get; set; }

    [XmlName("Spread_1HOU")] public decimal? Spread1Hou { get; set; }

    [XmlName("Spread_HDP")] public decimal? SpreadHdp { get; set; }

    [XmlName("Spread_OU")] public decimal? SpreadOu { get; set; }
    
    [XmlName("WD_NoOfHDP")] public string WDNoOfHDP { get; set; }

    [XmlName("WK_NoOfHDP")] public string WKNoOfHDP { get; set; }
}