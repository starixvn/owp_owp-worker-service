namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Services;

public interface NapUserControlService
{
    Task SynDataFunctionPermissionAsync();
}

public class NapUserControlServiceImp(IDynamicRepository repository, IUserService userService, INapSettings napSettings)
    : NapUserControlService
{
    public async Task SynDataFunctionPermissionAsync()
    {
        const string functionKey = "NotificationSettingWatcher";
        var data = (await userService.GetBySiteAndFunctionKeyAsync(napSettings.SiteId, functionKey)).ToList();

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
}