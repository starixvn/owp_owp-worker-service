namespace OWP.ArchiveData.Criteria;

using Fanex.Data.Repository;

public class ArchiveOldEventPickingLogCriteria : NonQueryCommand
{
    public DateTime Date { get; set; }

    public override string GetSettingKey() => "OWP_EventPickingLog_Archive";

    public override bool IsValid() => true;
}