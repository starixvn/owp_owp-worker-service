using System.Text;

namespace OWP.OtherSport.SyncAdminSiteId.Helpers;

public static class ExtensionMethod
{
    private const byte BYTE_SYMBOL_X = 63;

    private const byte BYTE_CHARACTER_X = 120;

    public static string ReplaceSymbol_X(this string teamName)
    {
        var bytes = Encoding.ASCII.GetBytes(teamName);
        for (var index = 0; index < bytes.Length; index++)
        {
            if (bytes[index] == BYTE_SYMBOL_X)
            {
                bytes[index] = BYTE_CHARACTER_X;
            }
        }
        return Encoding.ASCII.GetString(bytes);
    }

    public static string RemoveSpace(this string teamName)
    {
        while (teamName.Contains("  ", StringComparison.CurrentCulture))
        {
            teamName = teamName.Remove(teamName.IndexOf("  ", StringComparison.CurrentCulture), 1);
        }

        return teamName;
    }

    public static string ReplaceHiddenCharacters(this string value)
    {
        const string HiddenChars = " ";

        while (value.Contains(HiddenChars, StringComparison.CurrentCulture))
        {
            value = value.Replace(HiddenChars, " ");
        }

        return value;
    }
}