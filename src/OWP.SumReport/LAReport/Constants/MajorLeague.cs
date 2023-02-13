namespace OWP.SumReport.LAReport.Constants;

using System.Collections.Immutable;

public static class MajorLeague
{
    public static readonly ImmutableList<string> MajorLeagueCornerList = new List<string>{
        "*ENGLISH PREMIER LEAGUE - CORNERS",
        "*GERMANY-BUNDESLIGA I - CORNERS",
        "*ITALY SERIE A - CORNERS",
        "*SPAIN PRIMERA LALIGA - CORNERS",
        "*UEFA CHAMPIONS LEAGUE - CORNERS"
    }.ToImmutableList();

    public static readonly ImmutableList<string> MajorLeagueList = new List<string>{
        "*ENGLISH PREMIER LEAGUE",
        "*GERMANY-BUNDESLIGA I",
        "*ITALY SERIE A",
        "*SPAIN PRIMERA LALIGA",
        "*UEFA CHAMPIONS LEAGUE"
    }.ToImmutableList();
}