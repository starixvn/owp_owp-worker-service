namespace OWP.Core.Extensions;

public static class StringExtension
{
    public static bool IsContainsIgnoreCase(this string @this, string value)
    {
        return @this.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static string ConvertNullToEmpty(this string value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Trim();
    }

    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source?.IndexOf(toCheck, comp) >= 0;
    }
}