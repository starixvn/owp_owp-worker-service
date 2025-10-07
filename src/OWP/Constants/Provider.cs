using System.Collections.Immutable;

namespace OWP.Constants
{
    public static class ProviderConst
    {
        public static readonly IImmutableDictionary<int, string> Provider = new Dictionary<int, string>
        {
            { 3, "365GV"},
            { 15, "IMG"},
            { 17, "RB2" },
            { 18, "RTS" },
            { 19, "BG" },
            { 20, "BET" },
            { 202, "BET2" }
        }.ToImmutableDictionary();
    }
}
