namespace OWP.ArchiveData.Criteria;

using Fanex.Data.Repository;

public class CopyDataToOldTableCriteria : NonQueryCommand
{
    public DateTime Date { get; set; }

    public override string GetSettingKey() => "OWP_CopyDataToOldTable";

    public override bool IsValid() => true;
}