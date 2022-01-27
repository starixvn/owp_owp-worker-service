using Fanex.Logging;
using Newtonsoft.Json;
using Olympus.InfoSiteClient;
using OWP.InfositeIdSync.Models;
using System.Text;

namespace OWP.InfositeIdSync.Services;

public interface LogServiceWrapper
{
    Task Log(string header, params object[] parameters);

    Task Error(string header, params object[] parameters);

    Task LogSynIsLive(
         List<SyncIsLiveModel> unsynchorizedMatches,
        IEnumerable<ValueTuple<int, int?>> deletedMatches,
        IEnumerable<ValueTuple<int, int?>> liveMatches,
        IEnumerable<ValueTuple<int, int?>> nonLiveMatches,
        IsLiveSynchronizedResult synchronizedLiveMatchResult,
        IsLiveSynchronizedResult synchronizedNonLiveMatchResult);

    Task ErrorDuplicatedMatch(IEnumerable<int> liveMatches, IEnumerable<int> nonLiveMatches);
}

public class LogServiceWrapperImp : LogServiceWrapper
{
    public async Task Error(string header, params object[] parameters)
    {
        var logMessage = BuildLogMessage(header, parameters);

        await Logger.Log.ErrorAsync(logMessage).ConfigureAwait(false);
    }

    public async Task Log(string header, params object[] parameters)
    {
        var logMessage = BuildLogMessage(header, parameters);

        await Logger.Log.InfoAsync(logMessage).ConfigureAwait(false);
    }

    private static string BuildLogMessage(string header, params object[] parameters)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(header);

        var index = 0;
        foreach (var parameter in parameters)
        {
            stringBuilder.AppendFormat("Parameter {0}-{1}: {2}\r\n", index, parameter.GetType(), JsonConvert.SerializeObject(parameter));
            index++;
        }

        return stringBuilder.ToString();
    }

    public async Task LogSynIsLive(
        List<SyncIsLiveModel> unsynchorizedMatches,
        IEnumerable<(int, int?)> deletedMatches,
        IEnumerable<(int, int?)> liveMatches, IEnumerable<(int, int?)> nonLiveMatches,
        IsLiveSynchronizedResult synchronizedLiveMatchResult,
        IsLiveSynchronizedResult synchronizedNonLiveMatchResult)
    {
        if (!unsynchorizedMatches.Any() &&
            !deletedMatches.Any() &&
            !liveMatches.Any() &&
            !nonLiveMatches.Any())
        {
            return;
        }

        var duplicatedMatches = liveMatches.Intersect(nonLiveMatches);
        var logObject = new
        {
            unsynchorizedMatches = unsynchorizedMatches.ToArray(),
            deletedMatches = deletedMatches.ToArray(),
            liveMatches = liveMatches.ToArray(),
            nonLiveMatches = nonLiveMatches.ToArray(),
            synchronizedLiveMatchResult = synchronizedLiveMatchResult,
            synchronizedNonLiveMatchResult = synchronizedNonLiveMatchResult,
            duplicatedMatches = duplicatedMatches
        };

        var json = JsonConvert.SerializeObject(logObject);
        var logMessage = $"===Sync Is Live to Info Site===\r\n{json}";
        await Logger.Log.InfoAsync(logMessage).ConfigureAwait(false);
    }

    public async Task ErrorDuplicatedMatch(IEnumerable<int> liveMatches, IEnumerable<int> nonLiveMatches)
    {
        var duplicatedMatches = liveMatches.Intersect(nonLiveMatches);
        if (duplicatedMatches.Any())
        {
            await Error($"=== Sync Is Live to Info Site: Duplicated matches ===", duplicatedMatches.ToArray());
        }
    }
}