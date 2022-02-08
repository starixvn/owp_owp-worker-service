using OWP.Core.Extensions;
using OWP.MatchReadyNotification.Models;

namespace OWP.MatchReadyNotification.Services;

public interface ReceivedUserServices
{
    List<Assignee> GetAllAssignees(AssignmentSnapshot snapshot);

    Task<Dictionary<int, List<Assignee>>> GetAllReceivedUsers(List<Assignee> assignees);

    List<Assignee> GetAssigneesOfMatch(AssignmentSnapshot snapshot, int matchId);

    List<Assignee> GetReceivedUsersOfMatch(List<Assignee> assignees, Dictionary<int, List<Assignee>> allReceivedUsers);
}

public class ReceivedUserServicesImp : ReceivedUserServices
{
    private readonly MatchReadyRepository matchReadyRepository;

    public ReceivedUserServicesImp(MatchReadyRepository matchReadyRepository)
    {
        this.matchReadyRepository = matchReadyRepository;
    }

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
        var sessionLST = await GetSessionLST();

        foreach (var key in leadersByUserId.Keys.ToList())
        {
            leadersByUserId[key].AddRange(sessionLST);
            leadersByUserId[key] = leadersByUserId[key].Distinct(new Assignee()).ToList();
        }

        return leadersByUserId;
    }

    private async Task<Dictionary<int, List<Assignee>>> GetManagersOfMatch(List<Assignee> assignees)
    {
        var assigneeIdsInString = string.Join(",", assignees.Select(x => x.AssigneeId));
        return await matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession(assigneeIdsInString);
    }

    private async Task<List<Assignee>> GetSessionLST()
    {
        var sessionLST = await matchReadyRepository.GetAllSessionLST();

        return sessionLST.Distinct(new Assignee()).ToList();
    }

    public List<Assignee> GetReceivedUsersOfMatch(List<Assignee> assignees, Dictionary<int, List<Assignee>> allReceivedUsers)
    {
        var list = new List<Assignee>();
        foreach (var assignee in assignees)
        {
            var receivedUsers = allReceivedUsers.GetValue(assignee.AssigneeId, new List<Assignee>());
            list.AddRange(receivedUsers);
        }

        return list.Distinct(new Assignee()).ToList();
    }
}