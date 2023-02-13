namespace OWP.Infosite.UnitTests;

using Criteria;
using Fanex.Data.Repository;
using Models;
using NSubstitute;
using Services;
using Xunit;

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
                new()
                {
                     FunctionId=1,
                      Policy=2,
                       UserId=8
                }
            };

        var syncService = new NapUserControlServiceImp(repository);

        repository.Fetch<UserFunctionModel>(Arg.Compat.Any<GetAllUserByFunctionKeyCriteria>()).Returns(userFunctionModels);
        syncService.GetUserFunctionModels("functionKey").Returns(userFunctionModels);

        await syncService.SynDataFunctionPermissionAsync();

        await repository.Received().ExecuteAsync(Arg.Compat.Any<SyncNotificationSettingWatcherCriteria>());
    }

    [Fact]
    public void GetUserFunctionModels_FunctionKey_UserFunctionModels()
    {
        // Arrange
        var userFunctionModels = new List<UserFunctionModel>
            {
                new()
                {
                     FunctionId=1,
                     Policy=2,
                     UserId=8
                }
            };
        var syncService = new NapUserControlServiceImp(repository);

        repository.Fetch<UserFunctionModel>(Arg.Compat.Any<GetAllUserByFunctionKeyCriteria>()).Returns(userFunctionModels);

        var result = syncService.GetUserFunctionModels("functionKey");
        repository.Received().Fetch<UserFunctionModel>(Arg.Compat.Any<GetAllUserByFunctionKeyCriteria>());

        // Assert
        Assert.Equal(userFunctionModels, result);
    }
}