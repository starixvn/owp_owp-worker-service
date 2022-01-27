namespace OWP.MatchReadyNotification.Models;

public class MessageModel
{
    public string MessageContent { get; set; }

    public int MessageType { get; set; }

    public bool IsImportant { get; set; }

    public bool IsWatcher { get; set; }

    public string ReceivedUserIds { get; set; }

    public string ReceivedUserNames { get; set; }
}