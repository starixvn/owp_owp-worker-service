using System;
using Fanex.Data.Repository;
using OWP.ArchiveData.Criteria;

namespace OWP.ArchiveData;

public interface Repository
{
    Task DeleteOldMessage(DateTime date);
}

public class RepositoryImp : Repository
{
    private readonly IDynamicRepository repository;

    public RepositoryImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task DeleteOldMessage(DateTime date)
    {
        var criteria = new DeleteOldMessageCriteria { Date = date };
        await repository.ExecuteAsync(criteria);
    }
}