using System.Collections.Immutable;
using System.Collections.Generic;
namespace OWP.SumReportData.LAReport.Constants;

public static class MajorLeague
{
    public static readonly ImmutableList<string> majorLeagueCornerList = new List<string>{
        "*ENGLISH PREMIER LEAGUE - CORNERS",
        "*GERMANY-BUNDESLIGA I - CORNERS",
        "*ITALY SERIE A - CORNERS",
        "*SPAIN PRIMERA LALIGA - CORNERS",
        "*UEFA CHAMPIONS LEAGUE - CORNERS"
    }.ToImmutableList();

    public static readonly ImmutableList<string> majorLeagueList = new List<string>{
        "*ENGLISH PREMIER LEAGUE",
        "*GERMANY-BUNDESLIGA I",
        "*ITALY SERIE A",
        "*SPAIN PRIMERA LALIGA",
        "*UEFA CHAMPIONS LEAGUE"
    }.ToImmutableList();
}