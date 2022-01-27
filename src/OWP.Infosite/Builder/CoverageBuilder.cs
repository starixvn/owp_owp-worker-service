using OWP.Core.Extensions;
using OWP.Infosite.Models;

namespace OWP.Infosite.Builder;

public class CoverageBuilder
{
    private readonly int[] validWebsiteId = { 17, 18, 19, 20 };

    private readonly Dictionary<int, string> websitePair = new()
    {
        { 17, "RB2" },
        { 18, "RTS" },
        { 19, "BG" },
        { 20, "BET" },
    };

    private readonly Dictionary<int, string> coveragePair = new()
    {
        { 0, string.Empty },
        { 1, "TV" }
    };

    public string CoverageBuild(CommonMatchModel infoSiteMatch)
    {
        var coveragesMainMatch = BuildCoverageMainMatch(infoSiteMatch);
        var coveragesMergeMatch = BuildCoverageMergeMatch(infoSiteMatch);
        coveragesMainMatch.AddRange(coveragesMergeMatch);
        coveragesMainMatch = coveragesMainMatch
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .ToList();
        return string.Join("/", coveragesMainMatch);
    }

    public List<string> BuildCoverageMainMatch(CommonMatch infoSiteMatch)
    {
        List<string> coverages = new List<string>();
        var isExistsInWebsiteIdValid = validWebsiteId.Any(item => item == infoSiteMatch.website_id);
        if (isExistsInWebsiteIdValid)
        {
            var coverage = BuildCoverage(infoSiteMatch.coverage_id, infoSiteMatch.website_id);
            coverages.Add(coverage);
        }
        return coverages;
    }

    public List<string> BuildCoverageMergeMatch(CommonMatchModel infoSiteMatch)
    {
        List<string> coverages = new List<string>();
        infoSiteMatch.ls_merge_match?.ForEach(match =>
        {
            var isExistsInWebsiteIdValid = validWebsiteId.Any(
                item => item == match.website_id
            );
            if (isExistsInWebsiteIdValid)
            {
                var coverage = BuildCoverage(match.coverage_id, match.website_id);
                coverages.Add(coverage);
            }
        });
        return coverages;
    }

    public string BuildCoverage(int? coverageId, int? websiteId)
    {
        var website = string.Empty;
        var coveragePairHasCoverageId = coverageId.HasValue
             && coveragePair.ContainsKey(coverageId.Value);

        if (coveragePairHasCoverageId)
        {
            website = GetValueFromDict(websiteId, websitePair);
        }
        var coverage = GetValueFromDict(coverageId, coveragePair);

        return $"{website}{coverage}";
    }

    public static string GetValueFromDict<T>(int? key, T dict) where T : Dictionary<int, string>
        => key.HasValue
        ? dict.GetValue(key.Value, string.Empty)
        : string.Empty;
}