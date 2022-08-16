namespace OWP.WorkerService.Constants;

#pragma warning disable S125 // Sections of code should not be commented out

public static class CronExpressionJob
{
    public const string SyncTodayInfoSiteMatch = "0 0/1 * 1/1 * ? *"; //1min

    public const string SyncFutureInfoSiteMatch = "0 0/15 * 1/1 * ? *"; //15mins

    public const string Sync180DayInfoSiteMatch = "0 0 * ? * *"; //1hour

    public const string SyncEventDeleted = "0 0/5 * 1/1 * ? *"; //5mins

    public const string SyncDataNotificationSetting = "0 59 12 1/1 * ? *"; //at 12:59:00pm, every day

    public const string MatchReadyNotification = "0 0/1 * 1/1 * ? *"; //1min

    public const string SyncInfositeId = "0 0/3 * 1/1 * ? *"; //3mins

    public const string SyncPreMatchLeague = "0 0/5 * 1/1 * ? *"; //5mins

    public const string SyncLeagueSetting = "0 0 2 ? * MON *"; //2AM every Monday GMT-4

    public const string LinkLeagueAndMatch = "0 0 2 ? * MON *"; //2AM every Monday GMT-4

    //public const string SumDataReport = "0 0 2 ? * * *"; //2AM everyday GMT-4
    public const string SumDataReport = "0 0/15 * 1/1 * ? *"; //15mins

    public const string SyncOtherSportAdminSiteJob = "0 0/2 * 1/1 * ? *"; //2mins

    public const string DeleteOldMessage = "0 0 4 ? * * *"; //4AM everyday GMT-4;

    public const string ArchiveData = "0 10/59 0/12 ? * * *"; //at 12:10 am/pm everyday GMT-4
}

#pragma warning restore S125