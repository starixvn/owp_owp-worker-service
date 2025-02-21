namespace OWP.MatchReadyNotification;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;
using System.Globalization;

public interface MatchReadyRepository
{
    Task<AssignmentSnapshot> GetNotReadyMatches(string lastTimeNotReadyNotification);

    Task<Dictionary<int, List<Assignee>>> GetLeaderSupervisorSectionHeadOfSession(string userIds);

    Task InsertMessage(List<MessageModel> messages, string sessionId);
}

public class MatchReadyRepositoryImp(IDynamicRepository repository) : MatchReadyRepository
{
    public async Task<AssignmentSnapshot> GetNotReadyMatches(string lastTimeNotReadyNotification)
    {
        var getAllAssignmentsByXMinsCriteria = new GetAllAssignmentsByXMinsCriteria
        {
            LastTimeInCache = lastTimeNotReadyNotification
        };
        var assignmentsTask = await repository.FetchMultipleAsync<DateTime, MatchAssignment>(getAllAssignmentsByXMinsCriteria);
        var lastTimeNotification = assignmentsTask.Item1.FirstOrDefault();
        var assignment = assignmentsTask.Item2.GroupBy(a => a.MatchId).ToDictionary(a => a.Key, a => a.ToList());

        var matchIds = assignment.Select(x => x.Key).ToList();
        var matchIdsInString = string.Join(",", matchIds);
        var getAllMatchesByXMinsCriteria = new GetAllMatchesByXMinsCriteria
        {
            MatchIds = matchIdsInString
        };

        var matchesTask = await repository.FetchAsync<MatchInfo>(getAllMatchesByXMinsCriteria);

        return new AssignmentSnapshot
        {
            Matches = matchesTask.ToDictionary(m => m.MatchId),
            Assignments = assignment,
            LastDateTimeNotification = lastTimeNotification.ToString(CultureInfo.InvariantCulture)
        };
    }

    public async Task<Dictionary<int, List<Assignee>>> GetLeaderSupervisorSectionHeadOfSession(string userIds)
    {
        var criteria = new GetLeaderSupervisorSectionHeadOfSessionCriteria
        {
            UserIds = userIds
        };

        var leaders = (await repository.FetchAsync<SessionManager>(criteria)).ToList();

        return leaders.GroupBy(a => a.UserId)
                      .ToDictionary(key => key.Key, value => value.Select(i => new Assignee
                      {
                          AssigneeId = i.ReceivedUserId,
                          AssigneeName = i.ReceivedUserName
                      })
                      .Distinct(new Assignee())
                      .ToList());
    }

    public async Task InsertMessage(List<MessageModel> messages, string sessionId)
    {
        var criteria = new InsertMessageCriteria
        {
            Messages = XmlConverter.Serialize(messages),
            CreatedByUserId = 0,
            SessionId = sessionId
        };

        await repository.ExecuteAsync(criteria);
    }
}