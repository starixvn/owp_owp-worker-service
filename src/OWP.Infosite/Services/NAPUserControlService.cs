namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Services;

public interface NapUserControlService
{
    Task SynDataFunctionPermissionAsync();
}

public class NapUserControlServiceImp : NapUserControlService
{
    private readonly IDynamicRepository repository;

    private readonly IUserService userService;

    private readonly INapSettings napSettings;

    public NapUserControlServiceImp(IDynamicRepository repository, IUserService userService, INapSettings napSettings)
    {
        this.repository = repository;
        this.userService = userService;
        this.napSettings = napSettings;
    }

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