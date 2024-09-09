using OWP.Infosite.Constants;

namespace OWP.Infosite.Builder;

using Models;

public static class TvBuilder
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

        return Constant.WebsiteProvider.ContainsKey(infositeMatch.website_id ?? 0)
               && infositeMatch.coverage_id == 1
               && Array.Exists(condition, c => !string.IsNullOrEmpty(c))
            ? "TV"
            : string.Empty;
    }
}