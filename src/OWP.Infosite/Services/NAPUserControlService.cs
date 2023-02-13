namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Models;

public interface NAPUserControlService
{
    IEnumerable<UserFunctionModel> GetUserFunctionModels(string functionKey);

    Task SynDataFunctionPermissionAsync();
}

public class NAPUserControlServiceImp : NAPUserControlService
{
    private readonly IDynamicRepository repository;

    public NAPUserControlServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task SynDataFunctionPermissionAsync()
    {
        const string functionKey = "NotificationSettingWatcher";
        var data = GetUserFunctionModels(functionKey);

        if (!data.Any())
        {
            return;
        }

        var userIds = data.Select(x => x.UserId).ToArray();
        if (userIds.Any())
        {
            var ids = string.Join(",", userIds);
            var criteria = new SyncNotificationSettingWatcherCriteria
            {
                Ids = ids
            };

            await repository.ExecuteAsync(criteria);
        }
    }

    public IEnumerable<UserFunctionModel> GetUserFunctionModels(string functionKey)
    {
        var criteria = new GetAllUserByFunctionKeyCriteria
        {
            FunctionKey = functionKey
        };

        return repository.Fetch<UserFunctionModel>(criteria);
    }
}