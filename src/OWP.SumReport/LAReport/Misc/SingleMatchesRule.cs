namespace OWP.SumReport.LAReport.Misc;

using Enum;
using Extensions;
using Models;

public class SingleMatchesRule
{
    public IsSingleMatchesRule NextRule { get; set; }

    public virtual ReportType Execute(Assignment assignment)
    {
        return NextRule?.Execute(assignment) ?? ReportType.Others;
    }
}

public class IsSingleMatchesRule : SingleMatchesRule
{
    private readonly string[] reporterContain = ["Reporter", "AL"];
    private readonly string[] reporterEqual = ["WOO", "SLR", "AL", "BC"];

    private readonly string[] onlyTvContain = ["Glive", "IPTV", "Inhouse", "WB"];
    private readonly string[] onlyTvTupleContain = ["BET365", "TV"];
    private readonly string[] onlyTvEqual = ["TV", "BET365"];
    private const string Futsal = "Futsal";
    private const string Saba = "SABA";

    private readonly string[] providersContain =
    [
        "RB2","RB3","IMG","RTS", "IPSD", "BG", "outside", "MRB", "follow", "FC", "365GV"
    ];
    private readonly string[] providersEqual = ["SM", "VI"];

    private const string ProviderBet = "BET";

    public override ReportType Execute(Assignment assignment)
    {
        if (IsFutsalLeague(assignment.OwpLeagueName))
        {
            return ReportType.OnlyTv;
        }
        if (string.IsNullOrEmpty(assignment.ProviderName))
        {
            return base.Execute(assignment);
        }

        var matchProviders = assignment.ProviderName.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var isOnlyProvider = IsOnlyProvider(matchProviders);
        var isOnlyTv = IsOnlyTV(matchProviders, assignment.OwpLeagueName);
        var isOnlyReporter = IsOnlyReporter(matchProviders);
        var mixProductList = new List<bool> { isOnlyTv, isOnlyReporter, isOnlyProvider };

        if (mixProductList.Count(x => x) >= 2)
        {
            return ReportType.MixedProviderTvReporter;
        }
        if (isOnlyProvider)
        {
            return ReportType.OnlyProvider;
        }
        if (isOnlyTv)
        {
            return ReportType.OnlyTv;
        }

        return isOnlyReporter ? ReportType.OnlyReporter : base.Execute(assignment);
    }

    private static bool IsOnlyProduct(string[] matchProviders, string[] equals, string[] contains)
        => Array.Exists(matchProviders, x => IsContainsExactElement(equals, x.Trim())) ||
           Array.Exists(matchProviders, x => IsElementOfListContainText(contains, x.Trim()));

    private bool IsOnlyTV(string[] matchProviders, string leagueName)
        => IsOnlyProduct(matchProviders, onlyTvEqual, onlyTvContain) ||
           Array.Exists(matchProviders, x => IsTextContainAllElementOfList(onlyTvTupleContain, x.Trim())) ||
           IsFutsalLeague(leagueName);

    private bool IsOnlyReporter(string[] matchProviders)
        => IsOnlyProduct(matchProviders, reporterEqual, reporterContain);

    private bool IsOnlyProvider(string[] matchProviders)
        => IsOnlyProduct(matchProviders, providersEqual, providersContain) ||
           Array.Exists(matchProviders, x => IsElementContainBetProvider(x.Trim()));

    private static bool IsFutsalLeague(string leagueName)
        => leagueName.IsContainsIgnoreCase(Futsal) &&
          !leagueName.IsContainsIgnoreCase(Saba);

    private static bool IsElementOfListContainText(IEnumerable<string> list, string text)
    {
        return list.Any(element => text.IsContainsIgnoreCase(element.Trim()));
    }

    private static bool IsElementContainBetProvider(string text)
    {
        return text.IsContainsIgnoreCase(ProviderBet) && !text.IsContainsIgnoreCase("BET365");
    }

#pragma warning disable S1172
    private static bool IsTextContainAllElementOfList(IEnumerable<string> list, string text)
    {
        return list.All(text.IsContainsIgnoreCase);
    }
#pragma warning restore S1172

    private static bool IsContainsExactElement(IEnumerable<string> providerLists, string item)
        => providerLists.Contains(item, StringComparer.OrdinalIgnoreCase);
}