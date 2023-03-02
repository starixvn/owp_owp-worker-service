namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class SyncNotificationSettingWatcherCriteria : NonQueryCommand
{
    public string Ids { get; set; }

    public override string GetSettingKey() => "OWP_API_SyncNotificationSettingWatcher";

    public override bool IsValid() => true;
}