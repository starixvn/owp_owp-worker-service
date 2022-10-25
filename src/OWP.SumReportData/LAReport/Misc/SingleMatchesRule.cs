using OWP.Core.Extensions;
using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Models;

namespace OWP.SumReportData.LAReport.Misc;

public class SingleMatchesRule
{
    public IsSingleMatchesRule NextRule { get; set; }

    public virtual ReportType Execute(Assignment assignment)
    {
        return NextRule?.Execute(assignment) ?? ReportType.OTHERS;
    }
}

public class IsSingleMatchesRule : SingleMatchesRule
{
    private readonly string[] reporterContain = { "Reporter", "AL" };
    private readonly string[] reporterEqual = { "WOO", "SLR", "AL", "BC" };

    private readonly string[] onlyTVContain = { "Glive", "IPTV", "Inhouse", "WB" };
    private readonly string[] onlyTVTupleContain = { "BET365", "TV" };
    private readonly string[] onlyTVEqual = { "TV", "BET365" };
    private const string Futsal = "Futsal";
    private const string Saba = "SABA";

    private readonly string[] providersContain =
    {
        "RB2", "RTS", "IPSD", "BG", "BET", "outside", "MRB", "accept", "FC",
    };
    private readonly string[] providersEqual = { "SM", "VI" };

    public override ReportType Execute(Assignment assignment)
    {
        if (IsFutsalLeague(assignment.OWPLeagueName))
        {
            return ReportType.ONLY_TV;
        }
        if (string.IsNullOrEmpty(assignment.ProviderName))
        {
            return base.Execute(assignment);
        }

        var matchProviders = assignment.ProviderName.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var isOnlyProvider = IsOnlyProvider(matchProviders);
        var isOnlyTV = IsOnlyTV(matchProviders, assignment.OWPLeagueName);
        var isOnlyReporter = IsOnlyReporter(matchProviders);
        var mixProductList = new List<bool> { isOnlyTV, isOnlyReporter, isOnlyProvider };

        if (mixProductList.Where(x => x).Count() >= 2)
        {
            return ReportType.MIXED_PROVIDER_TV_REPORTER;
        }
        if (isOnlyProvider)
        {
            return ReportType.ONLY_PROVIDER;
        }
        if (isOnlyTV)
        {
            return ReportType.ONLY_TV;
        }
        if (isOnlyReporter)
        {
            return ReportType.ONLY_REPORTER;
        }

        return base.Execute(assignment);
    }

    private bool IsOnlyProduct(string[] matchProviders, string[] equals, string[] contains)
        => matchProviders.Any(x => IsContainsExactElement(equals, x.Trim())) ||
           matchProviders.Any(x => IsElementOfListContainText(contains, x.Trim()));

    private bool IsOnlyTV(string[] matchProviders, string leagueName)
        => IsOnlyProduct(matchProviders, onlyTVEqual, onlyTVContain) ||
           matchProviders.Any(x => IsTextContainAllElementOfList(onlyTVTupleContain, x.Trim())) ||
           IsFutsalLeague(leagueName);

    private bool IsOnlyReporter(string[] matchProviders)
        => IsOnlyProduct(matchProviders, reporterEqual, reporterContain);

    private bool IsOnlyProvider(string[] matchProviders)
        => IsOnlyProduct(matchProviders, providersEqual, providersContain);

    private bool IsFutsalLeague(string leagueName)
        => leagueName.IsContainsIgnoreCase(Futsal) &&
          !leagueName.IsContainsIgnoreCase(Saba);

    private bool IsElementOfListContainText(string[] list, string text)
    {
        foreach (var element in list)
        {
            if (text.IsContainsIgnoreCase(element.Trim()))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTextContainAllElementOfList(string[] list, string text)
    {
        foreach (var element in list)
        {
            if (!text.IsContainsIgnoreCase(element))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsContainsExactElement(string[] providerLists, string item)
        => providerLists.Contains(item, StringComparer.OrdinalIgnoreCase);
}