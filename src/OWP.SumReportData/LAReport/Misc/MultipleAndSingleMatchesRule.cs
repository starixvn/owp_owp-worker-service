using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Models;

namespace OWP.SumReportData.LAReport.Misc;

public class MultipleAndSingleMatchesRule
{
    public MultipleAndSingleMatchesRule NextRule { get; set; }

    public virtual ReportType Execute(Assignment assignment)
    {
        return NextRule?.Execute(assignment) ?? ReportType.OTHERS;
    }
}

public class MultipleMatchesRule : MultipleAndSingleMatchesRule
{
    private readonly string[] multipleMatchesProviders = { "RB2", "IPSD", "RTS", "BG", "BET", "RB2tv", "RB2(tv)", "BG(tv)", "BGtv", "BET(tv)", "BET(s)", "BETtv" };

    private readonly string[] notContainsMatchesProvider = { "TV" };

    private const string reporterSubfield = "Reporter";

    public override ReportType Execute(Assignment assignment)
    {
        if (string.IsNullOrEmpty(assignment.ProviderName))
        {
            return base.Execute(assignment);
        }

        var matchProviders = assignment.ProviderName.Split('/');

        if (matchProviders.Any(x => multipleMatchesProviders.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase))
            && !matchProviders.Any(x => notContainsMatchesProvider.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase))
            && !matchProviders.Any(x => IsContainsSubfield(x, reporterSubfield)))
        {
            return ReportType.SINGLE_LINK_LOWER_THAN3;
        }

        return base.Execute(assignment);
    }

    private static bool IsContainsSubfield(string provider, string subfield)
        => provider.IndexOf(subfield, 0, StringComparison.OrdinalIgnoreCase) >= 0;
}

public class SingleMatchesRule : MultipleAndSingleMatchesRule
{
    private const string reporterSubfield = "Reporter";

    private const string gliveSubfield = "Glive";

    private readonly string[] providerSubfield = { "MRB", "FC", "Bet outside" };

    private readonly string[] tvSubfield = { "TV", "WB", "BET365" };

    private readonly string[] singleMatchesProviders =
    {
        "RB2", "RTS", "BG", "BET", "FC", "MRB", "BC", "SLR", "AL", "Woo", "BET(TV)", "BET(tv)", "BET(s)",
        "BETtv", "RB2tv", "RB2(tv)", "BG(tv)", "BGtv"
    };

    private readonly string[] tvMatchesProvider = { "TV" };

    public override ReportType Execute(Assignment assignment)
    {
        if (string.IsNullOrEmpty(assignment.ProviderName))
        {
            return base.Execute(assignment);
        }

        var matchProviders = assignment.ProviderName.Split('/');

        if (matchProviders.Any(x => singleMatchesProviders.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase))
            && (matchProviders.Any(x => tvMatchesProvider.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase))
                || matchProviders.Any(x => IsContainsSubfield(x, reporterSubfield))))
        {
            return ReportType.PROVIDER_TV;
        }

        if (matchProviders.Length == 1)
        {
            if (providerSubfield.Contains(assignment.ProviderName.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                return ReportType.PROVIDER;
            }

            if (tvSubfield.Contains(assignment.ProviderName.Trim(), StringComparer.OrdinalIgnoreCase)
                || IsContainsSubfield(assignment.ProviderName.Trim(), gliveSubfield))
            {
                return ReportType.TV;
            }

            if (IsContainsSubfield(assignment.ProviderName.Trim(), reporterSubfield))
            {
                return ReportType.REPORTER;
            }
        }

        return base.Execute(assignment);
    }

    private static bool IsContainsSubfield(string provider, string subfield)
        => provider.IndexOf(subfield, 0, StringComparison.OrdinalIgnoreCase) >= 0;
}