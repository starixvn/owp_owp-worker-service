namespace OWP.MatchReadyNotification.Services;

using Extensions;
using Models;

public interface ReceivedUserServices
{
    List<Assignee> GetAllAssignees(AssignmentSnapshot snapshot);

    Task<Dictionary<int, List<Assignee>>> GetAllReceivedUsers(List<Assignee> assignees);

    List<Assignee> GetAssigneesOfMatch(AssignmentSnapshot snapshot, int matchId);

    List<Assignee> GetReceivedUsersOfMatch(List<Assignee> assignees, Dictionary<int, List<Assignee>> allReceivedUsers);
}

public class ReceivedUserServicesImp(MatchReadyRepository matchReadyRepository) : ReceivedUserServices
{
    public List<Assignee> GetAllAssignees(AssignmentSnapshot snapshot)
    {
        return snapshot.Assignments.SelectMany(o => o.Value).Select(x => new Assignee
        {
            AssigneeId = x.UserId,
            AssigneeName = x.UserName
        })
        .Distinct(new Assignee())
        .ToList();
    }

    public List<Assignee> GetAssigneesOfMatch(AssignmentSnapshot snapshot, int matchId)
    {
        return snapshot.Assignments[matchId].Select(x => new Assignee
        {
            AssigneeId = x.UserId,
            AssigneeName = x.UserName
        })
        .Where(a => a.AssigneeId != 0)
        .Distinct(new Assignee())
        .ToList();
    }

    public async Task<Dictionary<int, List<Assignee>>> GetAllReceivedUsers(List<Assignee> assignees)
    {
        var leadersByUserId = await GetManagersOfMatch(assignees);

        foreach (var key in leadersByUserId.Keys.ToList())
        {
            leadersByUserId[key] = leadersByUserId[key].Distinct(new Assignee()).ToList();
        }

        return leadersByUserId;
    }

    private async Task<Dictionary<int, List<Assignee>>> GetManagersOfMatch(IEnumerable<Assignee> assignees)
    {
        var assigneeIdsInString = string.Join(",", assignees.Select(x => x.AssigneeId));
        return await matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession(assigneeIdsInString);
    }

    public List<Assignee> GetReceivedUsersOfMatch(List<Assignee> assignees, Dictionary<int, List<Assignee>> allReceivedUsers)
    {
        return assignees.Select(a => allReceivedUsers.GetValue(a.AssigneeId, []))
            .SelectMany(a => a)
            .Distinct(new Assignee())
            .ToList();
    }
}