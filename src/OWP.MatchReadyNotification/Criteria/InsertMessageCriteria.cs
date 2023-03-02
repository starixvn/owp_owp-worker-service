namespace OWP.MatchReadyNotification.Criteria;

using Fanex.Data.Repository;

public class InsertMessageCriteria : NonQueryCommand
{
    public string Messages { get; set; }

    public int CreatedByUserId { get; set; }

    public string SessionId { get; set; }

    public override string GetSettingKey() => "OWP_SyncNotification_MessageInsert";

    public override bool IsValid() => true;
}