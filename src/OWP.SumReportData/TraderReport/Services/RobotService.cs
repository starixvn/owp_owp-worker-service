using OWP.Core.Extensions;
using OWP.SumReportData.TraderReport.Models;

namespace OWP.SumReportData.TraderReport.Services;

public interface RobotService
{
    Task<List<RobotByByUser>> SumRobotFields(DateTime eventDate, bool isGenerateData);
}

public class RobotServiceImp : RobotService
{
    private readonly TraderRepository repository;
    private readonly RobotX3Service robotX3Service;
    private readonly RobotX5Service robotX5Service;

    public RobotServiceImp(TraderRepository repository, RobotX3Service robotX3Service,
        RobotX5Service robotX5Service = null)
    {
        this.repository = repository;
        this.robotX3Service = robotX3Service;
        this.robotX5Service = robotX5Service;
    }

    public async Task<List<RobotByByUser>> SumRobotFields(DateTime eventDate, bool isGenerateData)
    {
        var robotX3ByUsers = await GetX3Fields(eventDate, isGenerateData);
        var robotX5ByUsers = await GetX5Fields(eventDate, isGenerateData);

        var aggregatedResult = Aggregate(robotX3ByUsers, robotX5ByUsers);

        return aggregatedResult.ToList();
    }

    private async Task<Dictionary<int, RobotX3ByUser>> GetX3Fields(DateTime eventDate, bool isGenerateData)
    {
        var rawX3Assignments = await repository.GetAssignmentsX3ByDate(eventDate, isGenerateData);
        var robotX3ByUsers = robotX3Service.Aggregate(rawX3Assignments.ToList());

        return robotX3ByUsers.ToDictionary(u => u.UserId, u => u);
    }

    private async Task<Dictionary<int, RobotX5ByUser>> GetX5Fields(DateTime eventDate, bool isGenerateData)
    {
        var rawX5Assignments = await repository.GetAssignmentsX5ByDate(eventDate, isGenerateData);
        var robotX5ByUsers = robotX5Service.Aggregate(rawX5Assignments.ToList());

        return robotX5ByUsers.ToDictionary(u => u.UserId, u => u);
    }

    private IEnumerable<RobotByByUser> Aggregate(Dictionary<int, RobotX3ByUser> robotX3ByUsers,
        Dictionary<int, RobotX5ByUser> robotX5ByUsers)
    {
        var userIds = robotX3ByUsers.Keys.Union(robotX5ByUsers.Keys);

        var defaultRobotX3ByUser = new RobotX3ByUser();
        var defaultRobotX5ByUser = new RobotX5ByUser();

        foreach (var userId in userIds)
        {
            var robotX3ByUser = robotX3ByUsers.GetValue(userId, defaultRobotX3ByUser);
            var robotX5ByUser = robotX5ByUsers.GetValue(userId, defaultRobotX5ByUser);

            yield return new RobotByByUser
            {
                UserId = userId,
                X2 = robotX3ByUser.X2,
                X3 = robotX3ByUser.X3,
                X3Plus = robotX3ByUser.X3Plus,
                X5 = robotX5ByUser.X5
            };
        }
    }
}