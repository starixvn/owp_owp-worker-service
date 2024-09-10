using OWP.Infosite.Constants;

namespace OWP.Infosite.Builder;

using Extensions;
using Models;
using System.Collections.Immutable;

public class CoverageBuilder
{
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
        var coverages = new List<string>();
        if (!Constant.WebsiteProvider.ContainsKey(infoSiteMatch.website_id ?? 0))
        {
            return coverages;
        }

        var coverage = BuildCoverage(infoSiteMatch.coverage_id, infoSiteMatch.website_id);
        coverages.Add(coverage);
        return coverages;
    }

    public List<string> BuildCoverageMergeMatch(CommonMatchModel infoSiteMatch)
    {
        var coverages = new List<string>();
        infoSiteMatch.ls_merge_match?.ForEach(match =>
        {
            if (!Constant.WebsiteProvider.ContainsKey(match.website_id ?? 0))
            {
                return;
            }

            var coverage = BuildCoverage(match.coverage_id, match.website_id);
            coverages.Add(coverage);
        });
        return coverages;
    }

    public string BuildCoverage(int? coverageId, int? websiteId)
    {
        var website = string.Empty;
        var coveragePairHasCoverageId = coverageId.HasValue
             && Constant.CoveragePair.ContainsKey(coverageId.Value);

        if (coveragePairHasCoverageId)
        {
            website = GetValueFromDict(websiteId, Constant.WebsiteProvider);
        }
        var coverage = GetValueFromDict(coverageId, Constant.CoveragePair);

        return $"{website}{coverage}";
    }

    public static string GetValueFromDict(int? key, IImmutableDictionary<int, string> dict)
        => key.HasValue
        ? dict.ToDictionary().GetValue(key.Value, string.Empty)
        : string.Empty;
}