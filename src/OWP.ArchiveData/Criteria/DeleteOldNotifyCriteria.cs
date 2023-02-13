using Fanex.Data.Repository;

namespace OWP.ArchiveData.Criteria;

internal class DeleteOldNotifyCriteria : INonQueryCommand
{
    public DateTime Date { get; set; }

    public string GetSettingKey() => "OWP_DeleteOldData_Message";

    public bool IsValid() => true;
}