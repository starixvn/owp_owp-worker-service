using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class SyncNotificationSettingWatcherCriteria : NonQueryCommand
{
    public string Ids { get; set; }

    public override string GetSettingKey() => "OWP_API_SyncNotificationSettingWatcher";

    public override bool IsValid() => true;
}