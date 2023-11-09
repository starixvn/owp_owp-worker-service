namespace OWP.Infosite.Services;
public interface FutureSyncRoundService
{
    Task Sync();
}

public class FutureSyncRoundServiceImp : FutureSyncRoundService
{
     private readonly SyncTeamRoundService roundService;

    public FutureSyncRoundServiceImp(SyncTeamRoundService roundService)
    {
        this.roundService = roundService;
    }

    public async Task Sync()
    => await roundService.Sync(DateTime.Today.AddDays(1));
}