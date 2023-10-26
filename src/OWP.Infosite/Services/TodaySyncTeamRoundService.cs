namespace OWP.Infosite.Services;
public interface TodaySyncTeamRoundService
{
    Task Sync();
}

public class TodaySyncTeamRoundServiceImp : TodaySyncTeamRoundService
{
    private readonly SyncTeamRoundService roundService;

    public TodaySyncTeamRoundServiceImp(SyncTeamRoundService roundService)
    {
        this.roundService = roundService;
    }

    public async Task Sync()
    => await roundService.Sync(DateTime.Today);
}