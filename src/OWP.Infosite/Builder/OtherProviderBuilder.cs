namespace OWP.Infosite.Builder;

using Models;

public static class OtherProviderBuilder
{
    public static string Build(CommonMatchModel infositeMatch)
    {
        var otherProviderItems = new List<string>();
        var coverageBuilder = new CoverageBuilder();
        otherProviderItems.Add(infositeMatch.TvBuild());
        otherProviderItems.Add(coverageBuilder.CoverageBuild(infositeMatch));
        otherProviderItems.Add(infositeMatch.ext_info?.provider.FormatProviders());

        return string.Join("/", otherProviderItems.Where(item => !string.IsNullOrEmpty(item)));
    }
}