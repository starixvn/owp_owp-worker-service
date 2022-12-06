using Fanex.Data.Repository;

namespace OWP.ArchiveData.Criteria
{
    internal class DeleteOldNoOfferMatchCriteria : INonQueryCommand
    {
        public DateTime Date { get; set; }

        public string GetSettingKey() => "OWP_DeleteOldData_NoOfferMatch";

        public bool IsValid() => true;
    }
}
