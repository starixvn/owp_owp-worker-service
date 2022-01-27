using Fanex.Data.Repository;
using NSubstitute;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using Xunit;

namespace OWP.Infosite.UnitTests;

public class NapUserControlServiceUnitTests
{
    private readonly IDynamicRepository repository;

    public NapUserControlServiceUnitTests()
    {
        repository = Substitute.For<IDynamicRepository>();
    }

    [Fact]
    public async Task SynDataFunctionPermission()
    {
        // Arrange
        var userFunctionModels = new List<UserFunctionModel>
            {
                new UserFunctionModel
                {
                     FunctionId=1,
                      Policy=2,
                       UserId=8
                }
            };

        var syncService = new NAPUserControlServiceImp(repository);

        repository.Fetch<UserFunctionModel>(Arg.Compat.Any<GetAllUserByFunctionKeyCriteria>()).Returns(userFunctionModels);
        syncService.GetUserFunctionModels("funtionKey").Returns(userFunctionModels);

        await syncService.SynDataFunctionPermissionAsync();

        await repository.Received().ExecuteAsync(Arg.Compat.Any<SyncNotificationSettingWatcherCriteria>());
    }

    [Fact]
    public void GetUserFunctionModels_FunctionKey_UserFunctionModels()
    {
        // Arrange
        var userFunctionModels = new List<UserFunctionModel>
            {
                new UserFunctionModel
                {
                     FunctionId=1,
                     Policy=2,
                     UserId=8
                }
            };
        var syncService = new NAPUserControlServiceImp(repository);

        repository.Fetch<UserFunctionModel>(Arg.Compat.Any<GetAllUserByFunctionKeyCriteria>()).Returns(userFunctionModels);

        var result = syncService.GetUserFunctionModels("funtionKey");
        repository.Received().Fetch<UserFunctionModel>(Arg.Compat.Any<GetAllUserByFunctionKeyCriteria>());

        // Assert
        Assert.Equal(userFunctionModels, result);
    }
}