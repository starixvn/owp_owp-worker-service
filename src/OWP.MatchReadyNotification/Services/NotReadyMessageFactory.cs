using OWP.MatchReadyNotification.Models;
using System.Globalization;

namespace OWP.MatchReadyNotification.Services;

public interface NotReadyMessageFactory
{
    Task<List<MessageModel>> Create(AssignmentSnapshot snapshot);
}

public class NotReadyMessageFactoryImp : NotReadyMessageFactory
{
    private readonly ReceivedUserServices receivedUserServices;

    private const string messageTemplate = "<UsernameList> are NOT READY for match <Match.No> - <League> - <Event> - K.O Time <KO Time>";

    public NotReadyMessageFactoryImp(ReceivedUserServices receivedUserServices)
    {
        this.receivedUserServices = receivedUserServices;
    }

    public async Task<List<MessageModel>> Create(AssignmentSnapshot snapshot)
    {
        var messageList = new List<MessageModel>();
        var allAssignees = receivedUserServices.GetAllAssignees(snapshot);
        var allReceivedUsers = await receivedUserServices.GetAllReceivedUsers(allAssignees);

        foreach (var match in snapshot.Matches.Values)
        {
            var assignees = receivedUserServices.GetAssigneesOfMatch(snapshot, match.MatchId);
            var receivedUsers = receivedUserServices.GetReceivedUsersOfMatch(assignees, allReceivedUsers);

            var model = new NotReadyMatchInfo
            {
                MatchNo = match.MatchNo,
                LeagueName = match.LeagueName,
                Event = match.Home + " -vs- " + match.Away,
                KOTime = match.KOTime,
                Assignees = assignees
            };

            messageList.Add(new MessageModel
            {
                MessageContent = Transform(model),
                IsImportant = true,
                IsWatcher = true,
                MessageType = 0,
                ReceivedUserIds = string.Join(",", receivedUsers.Select(a => a.AssigneeId)),
                ReceivedUserNames = string.Join(",", receivedUsers.Select(a => a.AssigneeName))
            });
        }

        return messageList;
    }

    public static string Transform(NotReadyMatchInfo model)
    {
        var content = messageTemplate.Replace("<Match.No>", GetFormattedMatchNo(model.MatchNo));
        content = content.Replace("<League>", model.LeagueName);
        content = content.Replace("<Event>", model.Event);
        content = content.Replace("<KO Time>", ToKOTime(model.KOTime));
        content = content.Replace("<UsernameList>", string.Join(", ", model.Assignees.Select(z => z.AssigneeName).ToList()));

        return content;
    }

    public static string GetFormattedMatchNo(string matchNo)
    {
        return !string.IsNullOrEmpty(matchNo) ? $"#{matchNo}" : string.Empty;
    }

    private static string ToKOTime(TimeSpan koTime)
    {
        var parsedTime = new DateTime(1, 1, 1, koTime.Hours, koTime.Minutes, 0);

        return parsedTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
    }
}