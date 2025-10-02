namespace OWP.AutoGenerateMaxbetSetting.Models;
public class LeagueSettingChangeModel
{
    public decimal? MaxbetWdHdp { get; set; }
    public decimal? MaxbetWkHdp { get; set; }
    public decimal? SpreadHdp { get; set; }
    public decimal? SpreadOu { get; set; }
    public string WDNoOfHDP { get; set; }
    public string WKNoOfHDP { get; set; }
    public string SpreadShow => AnalyzeSpreadSourceFT(SpreadHdp, SpreadOu);

    private static string AnalyzeSpreadSourceFT(decimal? spreadHdp, decimal? spreadOu)
    {
        if (!spreadHdp.HasValue || !spreadOu.HasValue)
        {
            return null;
        }
        var spreadHdpString = (spreadHdp * 100).ToString().TrimEnd('0').TrimEnd('.');
        var spreadOuString = (spreadOu * 100).ToString().TrimEnd('0').TrimEnd('.');

        return $"{spreadHdpString}%-{spreadOuString}%";
    }
}
