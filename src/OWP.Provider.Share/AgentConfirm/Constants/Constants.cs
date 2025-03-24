using System.Collections.Immutable;

namespace OWP.Provider.Share.AgentConfirm.Constants;

public static class Agent
{
    public const string Streaming = "Streaming";
    public const string Cancel = "Cancel";
    public const string Betradar = "Betradar";
    public static readonly ImmutableList<string> ProviderList = new List<string> {
        "365GV",
        "BG",
        "FC",
        "IMG",
        "RB2",
        "RB3",
        "RTS",
        "SLR",
        "WOO"
    }.ToImmutableList();
}
