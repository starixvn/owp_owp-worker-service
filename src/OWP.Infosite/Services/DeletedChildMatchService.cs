namespace OWP.Infosite.Services;

public interface SyncDeletedChildMatchService
{
    Task Sync(DateTime date);
}

public class SyncDeletedChildMatchServiceImp : SyncDeletedChildMatchService
{
    public async Task Sync(DateTime date)
    {
    }
}