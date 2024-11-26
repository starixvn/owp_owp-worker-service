namespace OWP.Infosite.Helper;

public static class ProviderHelper
{
    public static string BuildProviderByOtherProvider(List<string> priorityProviders, string otherProviderInString)
    {
        var otherProviders = otherProviderInString?.Split("/").ToList();
        foreach (var value in priorityProviders)
        {
            var provider = otherProviders?.Find(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(provider))
            {
                return provider;
            }
        }

        return BuildProviderTv(priorityProviders, otherProviders);
    }

    public static string BuildProviderTv(List<string> priorityProviders, List<string> otherProviders)
    {
        foreach (var value in priorityProviders)
        {
            var provider = otherProviders?.Find(x => x.IsContains(value) && x.IsContains("TV"));
            if (!string.IsNullOrWhiteSpace(provider))
            {
                return provider;
            }
        }

        return string.Empty;
    }

    private static bool IsContains(this string toBeSearchedString, string searchString)
    {
        if (string.IsNullOrEmpty(toBeSearchedString))
        {
            return false;
        }

        var notNullSearchString = searchString ?? string.Empty;
        return toBeSearchedString.Contains(notNullSearchString, StringComparison.OrdinalIgnoreCase);
    }
}