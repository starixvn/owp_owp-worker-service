using OWP.Chat.Models;

namespace OWP.Chat;

using Criteria;
using Fanex.Data.Repository;

public interface Repository
{
    Task<IList<UserInfo>> GetNewUser();
}

public class RepositoryImp(IDynamicRepository repository) : Repository
{
    public async Task<IList<UserInfo>> GetNewUser()
    {
        var newUser=await repository.FetchAsync<UserInfo>(new GetNewUserCriteria());
        return newUser.ToList();
    }
}