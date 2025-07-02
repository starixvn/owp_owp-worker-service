using OWP.AutoGenerateMaxbetSetting.Models;

namespace OWP.AutoGenerateMaxbetSetting;

using Fanex.Data.Repository;

public interface Repository
{
    Task<IList<EventInfo>> GetEventsInTimeRange(DateTime dateFrom, DateTime dateTo);

}

public class RepositoryImp(IDynamicRepository repository) : Repository
{
    
    public async Task<IList<EventInfo>> GetEventsInTimeRange(DateTime dateFrom, DateTime dateTo)
    {
        // TODO
        throw new NotImplementedException();
    }
}