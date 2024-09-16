namespace OWP.ArchiveData.Criteria;

using Fanex.Data.Repository;

public class ArchiveOldMessageCriteria : NonQueryCommand
{
    public DateTime Date { get; set; }

    public override string GetSettingKey() => "OWP_ArchiveOldMessage";

    public override bool IsValid() => true;
}