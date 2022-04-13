namespace OWP.ArchiveData.Services
{
    public interface ArchiveDataService
    {
        Task DeleteOldMessage(DateTime date);

        Task CopyDataToOldTable(DateTime date);

        Task DeleteOldData(DateTime date);
    }

    public class ArchiveDataServiceImp : ArchiveDataService
    {
        private readonly Repository repository;

        public ArchiveDataServiceImp(Repository repository)
        {
            this.repository = repository;
        }

        public async Task CopyDataToOldTable(DateTime date)
        {
            var copyingDate = date.AddDays(-35);
            await repository.CopyDataToOldTable(copyingDate);
        }

        public async Task DeleteOldData(DateTime date)
        {
            var deletingDate = date.AddDays(-40);
            await repository.DeleteOldData(deletingDate);
        }

        public async Task DeleteOldMessage(DateTime date)
        {
            await repository.DeleteOldMessage(date);
        }
    }
}