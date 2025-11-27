namespace OWP.ArchiveData.Services;

public interface ArchiveDataService
{
    Task ArchiveOldMessage(DateTime date);

    Task CopyDataToOldTable(DateTime date);

    Task DeleteOldData(DateTime date);

    Task DeleteOldNotify(DateTime date);

    Task ArchiveOldEventPickingLog(DateTime date);

    Task DeleteOldNoOfferMatch(DateTime today);
}

public class ArchiveDataServiceImp(Repository repository) : ArchiveDataService
{
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

    public async Task ArchiveOldMessage(DateTime date)
    {
        await repository.ArchiveOldMessage(date);
    }

    public async Task ArchiveOldEventPickingLog(DateTime date)
    {
        await repository.ArchiveOldEventPickingLog(date);
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