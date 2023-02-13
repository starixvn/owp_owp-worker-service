namespace OWP.Infosite.Builder;

using Models;

public static class TVBuilder
{
    public static string TvBuild(this CommonMatchModel infositeMatch)
    {
        var extInfo = infositeMatch.ext_info;
        var condition = new[]
        {
                extInfo.ip_tv,
                extInfo.local_tv,
                extInfo.tv_live,
                WatchBetBuilder.Build(extInfo)
        };

        var websiteIdValid = new[] { 17, 18, 19, 20 };

        return websiteIdValid.Any(w => w == infositeMatch.website_id)
            && infositeMatch.coverage_id == 1
            && condition.Any(c => !string.IsNullOrEmpty(c))
                ? "TV"
                : string.Empty;
    }
}