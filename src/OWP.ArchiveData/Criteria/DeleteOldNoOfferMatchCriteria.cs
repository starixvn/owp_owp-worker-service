namespace OWP.ArchiveData.Criteria;

using Fanex.Data.Repository;

public class DeleteOldNoOfferMatchCriteria : INonQueryCommand
{
    public DateTime Date { get; set; }

    public string GetSettingKey() => "OWP_DeleteOldData_NoOfferMatch";

    public bool IsValid() => true;
}