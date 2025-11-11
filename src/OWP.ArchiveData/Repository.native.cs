namespace OWP.ArchiveData;

using Criteria;
using Fanex.Data.Repository;

public interface Repository
{
    Task ArchiveOldMessage(DateTime date);

    Task ArchiveOldEventPickingLog(DateTime date);

    Task CopyDataToOldTable(DateTime date);

    Task DeleteOldData(DateTime date);

    Task DeleteOldNotify(DateTime date);

    Task DeleteOldNoOfferMatch(DateTime date);
}

public class RepositoryImp(IDynamicRepository repository) : Repository
{
    public async Task CopyDataToOldTable(DateTime date)
    {
        var criteria = new CopyDataToOldTableCriteria
        {
            Date = date
        };

        await repository.ExecuteAsync(criteria);
    }

    public async Task DeleteOldData(DateTime date)
    {
        if (date >= DateTime.Today.AddDays(-35))
        {
            throw new ArgumentOutOfRangeException($"Cannot delete not too old data at {date}");
        }

        var criteria = new DeleteOldDataCriteria { Date = date };
        await repository.ExecuteAsync(criteria);
    }

    public async Task ArchiveOldMessage(DateTime date)
    {
        var criteria = new ArchiveOldMessageCriteria { Date = date };
        await repository.ExecuteAsync(criteria);
    }

    public async Task ArchiveOldEventPickingLog(DateTime date)
    {
        var criteria = new ArchiveOldEventPickingLogCriteria { Date = date };
        await repository.ExecuteAsync(criteria);
    }

    public async Task DeleteOldNoOfferMatch(DateTime date)
    {
        var criteria = new DeleteOldNoOfferMatchCriteria { Date = date };
        await repository.ExecuteAsync(criteria);
    }

    public async Task DeleteOldNotify(DateTime date)
    {
        if (date >= DateTime.Today.AddDays(-35))
        {
            throw new ArgumentOutOfRangeException($"Cannot delete not too old data at {date}");
        }

        var criteria = new DeleteOldNotifyCriteria { Date = date };
        await repository.ExecuteAsync(criteria);
    }
}