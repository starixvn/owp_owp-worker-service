namespace OWP.Infosite.Builder;

public static class ProviderBuilder
{
    public static string FormatProviders(this string providers)
    {
        if (string.IsNullOrWhiteSpace(providers))
        {
            return string.Empty;
        }

        var providerFilter = providers
            .Replace("/", string.Empty)
            .Split(',')
            .Where(a => !string.IsNullOrEmpty(a));

        return (string.Join("/", providerFilter));
    }
}