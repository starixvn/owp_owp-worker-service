using Fanex.Data.Repository;

namespace OWP.ArchiveData.Criteria;

public class DeleteOldMessageCriteria : NonQueryCommand
{
    public DateTime Date { get; set; }

    public override string GetSettingKey() => "OWP_DeleteOldMessage";

    public override bool IsValid() => true;
}