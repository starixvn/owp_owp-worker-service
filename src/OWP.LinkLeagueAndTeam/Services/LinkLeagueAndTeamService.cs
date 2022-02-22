using OWP.LinkLeagueAndTeam.Models;

namespace OWP.LinkLeagueAndTeam.Services;

public interface LinkLeagueAndTeamService
{
    Task Execute();
}

public class LinkLeagueAndTeamServiceImp : LinkLeagueAndTeamService
{
    private readonly CacheManipulator cache;
    private readonly LinkLeagueAndTeamRepository repository;

    public LinkLeagueAndTeamServiceImp(CacheManipulator cache, LinkLeagueAndTeamRepository repository)
    {
        this.cache = cache;
        this.repository = repository;
    }

    public async Task Execute()
    {
        var timeRange = await PreExecute();
        var startTimeLatest = await Execute(timeRange);

        await PostExecute(startTimeLatest);
    }

    public async Task<TimeRange> PreExecute()
    {
        var startTime = await cache.GetStartTimeFromCache();
        var endTime = BuildEndTime(startTime);

        return new TimeRange
        {
            StartTime = startTime,
            EndTime = endTime,
            FinishRunTime = BuildFinishRunTime()
        };
    }

    public async Task<DateTime> Execute(TimeRange timeRange)
    {
        var startTime = timeRange.StartTime;
        var endTime = timeRange.EndTime;
        var finishRunTime = timeRange.FinishRunTime;

        if (DateTime.Compare(startTime, finishRunTime) <= 0 &&
            DateTime.Compare(endTime, finishRunTime) >= 0)
        {
            endTime = finishRunTime;
        }

        if (DateTime.Compare(endTime, finishRunTime) >= 0 &&
            DateTime.Compare(startTime, finishRunTime) >= 0)
        {
            return finishRunTime;
        }

        var teamInfo = await GetTeamInfoByTimRange(startTime, endTime);
        await UpdateTeamInfo(teamInfo);

        timeRange.StartTime = endTime;
        timeRange.EndTime = BuildEndTime(timeRange.StartTime);

        return await Execute(timeRange);
    }

    private async Task PostExecute(DateTime endTime)
        => await cache.SaveStartTimeQueryTeamInfo(endTime);

    public async Task<IList<TeamInfo>> GetTeamInfoByTimRange(DateTime startTime, DateTime endTime)
    {
        var temInfo = await repository.GetTeamInfoInTimeRange(startTime, endTime);
        var teamInfoDic = temInfo
            .GroupBy(t => t.TeamId)
            .ToDictionary(x => x.Key, v => v.ToList());

        var teamInfoResult = teamInfoDic
            .Select(t => teamInfoDic[t.Key]
                .OrderByDescending(k => k.EventDateTime)
                .First());

        return teamInfoResult.ToList();
    }

    public async Task UpdateTeamInfo(IList<TeamInfo> teamInfo)
    {
        foreach (var info in teamInfo)
        {
            await repository.InsertOrUpdateTeamInfo(info);
        }
    }

    public static DateTime BuildFinishRunTime()
    {
        var finishRunTimeGmt8 = DateTime.Now.AddHours(12);

        return new DateTime(finishRunTimeGmt8.Year, finishRunTimeGmt8.Month, finishRunTimeGmt8.Day, 0, 0, 0);
    }

    public static DateTime BuildEndTime(DateTime startTime)
    {
        const int dateRange = 7;
        return startTime.AddDays(dateRange);
    }
}