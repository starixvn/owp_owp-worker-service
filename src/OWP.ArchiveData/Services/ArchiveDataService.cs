using System;
using System.Threading.Tasks;

namespace OWP.ArchiveData.Services
{
    public interface ArchiveDataService
    {
        Task DeleteOldMessage(DateTime date);
    }

    public class ArchiveDataServiceImp: ArchiveDataService
    {
        private readonly Repository repository;

        public ArchiveDataServiceImp(Repository repository){
            this.repository = repository;
        }

        public async Task DeleteOldMessage(DateTime date)
        {
            await repository.DeleteOldMessage(date);
        }
    }
}