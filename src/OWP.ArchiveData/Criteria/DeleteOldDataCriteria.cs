namespace OWP.ArchiveData.Criteria;

using Fanex.Data.Repository;

public class DeleteOldDataCriteria : INonQueryCommand
{
    public DateTime Date { get; set; }

    public string GetSettingKey() => "OWP_DeleteOldData";

    public bool IsValid() => true;
}