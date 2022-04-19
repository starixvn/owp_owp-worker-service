using Fanex.Data.Repository;

namespace OWP.ArchiveData.Criteria;

public class CopyDataToOldTableCriteria : NonQueryCommand
{
    public DateTime Date { get; set; }

    public override string GetSettingKey() => "OWP_CopyDataToOldTable";

    public override bool IsValid() => true;
}