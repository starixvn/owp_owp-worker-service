using OWP.MatchReadyNotification.Services;
using System.Globalization;

namespace OWP.MatchReadyNotification;

public interface Handler
{
    Task Handle();
}

public class HandlerImp : Handler
{
    private readonly MatchReadyRepository matchReadyRepository;
    private readonly CacheManipulator cacheManipulator;
    private readonly NotReadyMessageFactory messageFactory;

    public HandlerImp(MatchReadyRepository matchReadyRepository, CacheManipulator cacheManipulator, NotReadyMessageFactory messageFactory)
    {
        this.matchReadyRepository = matchReadyRepository;
        this.cacheManipulator = cacheManipulator;
        this.messageFactory = messageFactory;
    }

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