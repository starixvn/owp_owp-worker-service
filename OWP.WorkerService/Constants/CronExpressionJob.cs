namespace OWP.WorkerService.Constants;

#pragma warning disable S125 // Sections of code should not be commented out

public static class CronExpressionJob
{
    public const string SyncTodayInfoSiteMatch = "0 0/1 * 1/1 * ? *";

    public const string SyncFutureInfoSiteMatch = "0 0/15 * 1/1 * ? *";

    public const string Sync180DayInfoSiteMatch = "0 0 * ? * *";

    public const string SyncEventDeleted = "0 0/5 * 1/1 * ? *";

    public const string SyncDataNotificationSetting = "0 59 12 1/1 * ? *";

    public const string SumReportData = "0 5/59 0/12 ? * * *";

    public const string MatchReadyNotification = "0 0/1 * 1/1 * ? *";

    public const string SyncInfositeId = "0 0/3 * 1/1 * ? *";

    public const string SyncPreMatchLeague = "0 0/5 * 1/1 * ? *";

    public const string SyncLeagueSetting = "0 0 2 ? * MON *"; //2AM every Monday GMT-4

    public const string LinkLeagueAndMatch = "0 0 2 ? * MON *"; //2AM every Monday GMT-4

    public const string SumOvertimeReport = "0 0 2 ? * * *"; //2AM everyday GMT-4

    public const string SyncOtherSportAdminSiteJob = "0 0/2 * 1/1 * ? *";

    public const string PrematchReport = "0 0 2 ? * * *"; //2AM everyday GMT-4;

    public const string ArchiveData = "0 0 4 ? * * *"; //4AM everyday GMT-4;
}

#pragma warning restore S125