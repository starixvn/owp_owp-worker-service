using Fanex.Data.Repository;

namespace OWP.ArchiveData.Criteria;

public class DeleteOldDataCriteria : INonQueryCommand
{
    public DateTime Date { get; set; }

    public string GetSettingKey() => "OWP_DeleteOldData";

    public bool IsValid() => true;
}