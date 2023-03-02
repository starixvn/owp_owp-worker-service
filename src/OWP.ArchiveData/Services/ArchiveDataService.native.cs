namespace OWP.ArchiveData.Services;

public interface ArchiveDataService
{
    Task DeleteOldMessage(DateTime date);

    Task CopyDataToOldTable(DateTime date);

    Task DeleteOldData(DateTime date);

    Task DeleteOldNotify(DateTime date);
        
    Task DeleteOldNoOfferMatch(DateTime today);
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

    public async Task DeleteOldNoOfferMatch(DateTime date)
    {
        var deletingDate = date.AddDays(-40);
        await repository.DeleteOldNoOfferMatch(deletingDate);
    }

    public async Task DeleteOldNotify(DateTime date)
    {
        var deletingDate = date.AddDays(-40);
        await repository.DeleteOldNotify(deletingDate);
    }
}