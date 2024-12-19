namespace OWP.MatchReadyNotification;

using System.Globalization;
using Services;

public interface Handler
{
    Task Handle();
}

public class HandlerImp(
    MatchReadyRepository matchReadyRepository,
    CacheManipulator cacheManipulator,
    NotReadyMessageFactory messageFactory)
    : Handler
{
    public async Task Handle()
    {
        var lastTimeNotReadyNotification = await cacheManipulator.GetLastTimeNotReadyNotification();

        var snapshot = await matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification);

        await cacheManipulator.SaveLastTimeNotification(snapshot.LastDateTimeNotification);

        var messageModels = await messageFactory.Create(snapshot);

        var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);

        await matchReadyRepository.InsertMessage(messageModels, sessionId);
    }
}