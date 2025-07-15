namespace OWP.InfositeIdSync.Services;

using Azure.Core;
using Fanex.Logging;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.Infosite.Models;
using System;
using System.Text;

public interface LogServiceWrapper
{
    Task Log(string header, params object[] parameters);

    Task Error(string header, params object[] parameters);

    Task LogSynIsLive(
        List<SyncIsLiveModel> unsynchronizedMatches,
        List<ValueTuple<int, int?>> deletedMatches,
        List<ValueTuple<int, int?>> liveMatches,
        List<ValueTuple<int, int?>> nonLiveMatches,
        IsLiveSynchronizedResult synchronizedLiveMatchResult,
        IsLiveSynchronizedResult synchronizedNonLiveMatchResult);

    Task ErrorDuplicatedMatch(IEnumerable<int> liveMatches, IEnumerable<int> nonLiveMatches);
}

public class LogServiceWrapperImp(LogWriterService logger) : LogServiceWrapper
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
            stringBuilder.Append(
                $"Parameter {index}-{parameter.GetType()}: {JsonConvert.SerializeObject(parameter)}\r\n");
            index++;
        }

        return stringBuilder.ToString();
    }

    public async Task LogSynIsLive(
        List<SyncIsLiveModel> unsynchronizedMatches,
        List<ValueTuple<int, int?>> deletedMatches,
        List<ValueTuple<int, int?>> liveMatches,
        List<ValueTuple<int, int?>> nonLiveMatches,
        IsLiveSynchronizedResult synchronizedLiveMatchResult,
        IsLiveSynchronizedResult synchronizedNonLiveMatchResult)
    {
        if (!unsynchronizedMatches.Any() &&
            !deletedMatches.Any() &&
            !liveMatches.Any() &&
            !nonLiveMatches.Any())
        {
            await logger.Execute(new OwpLogCriteria
            {
                CreatedDate = DateTime.Now.AddHours(12),
                Action = "SyncIsLiveToInfoSite",
                Payload = "No matches to synchronize."
            });
            return;
        }

        var duplicatedMatches = liveMatches.Intersect(nonLiveMatches);
        var logObject = new
        {
            unsynchronizedMatches = unsynchronizedMatches.ToArray(),
            deletedMatches = deletedMatches.ToArray(),
            liveMatches = liveMatches.ToArray(),
            nonLiveMatches = nonLiveMatches.ToArray(),
            synchronizedLiveMatchResult,
            synchronizedNonLiveMatchResult,
            duplicatedMatches
        };

        var json = JsonConvert.SerializeObject(logObject);
        var logMessage = $"===Sync Is Live to Info Site===\r\n{json}";

        await logger.Execute(new OwpLogCriteria
        {
            CreatedDate = DateTime.Now.AddHours(12),
            Action = "SyncIsLiveToInfoSite",
            Payload = json
        });

        await Logger.Log.InfoAsync(logMessage).ConfigureAwait(false);
    }

    public async Task ErrorDuplicatedMatch(IEnumerable<int> liveMatches, IEnumerable<int> nonLiveMatches)
    {
        var duplicatedMatches = liveMatches.Intersect(nonLiveMatches).ToList();
        if (duplicatedMatches.Any())
        {
            await Error("=== Sync Is Live to Info Site: Duplicated matches ===", duplicatedMatches.ToArray());
        }
    }
}