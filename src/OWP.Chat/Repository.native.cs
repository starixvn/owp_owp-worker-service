using OWP.Chat.Models;

namespace OWP.Chat;

using Criteria;
using Fanex.Data.Repository;

public interface Repository
{
    Task<IList<UserInfo>> GetNewUser();

    Task UpdateChatUserId(string UserJson);
}

public class RepositoryImp(IDynamicRepository repository) : Repository
{
    public async Task<IList<UserInfo>> GetNewUser()
    {
        var newUser=await repository.FetchAsync<UserInfo>(new GetNewUserCriteria());
        return newUser.Where(n=>n.UserName is not null && !n.UserName.Trim().Contains(' ')).ToList();
    }

    public async Task UpdateChatUserId(string UserJson)
    {
        await repository.ExecuteAsync(new ChatUserInfoCriteria()
        {
            UserJson = UserJson
        });
    }
}