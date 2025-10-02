namespace OWP.Provider.Share.AgentConfirm;

using Constants;
using Extensions;
using Model;
using OWP.Constants;
using Services;

public interface SyncAgentConfirmHandler
{
    Task Handle(DateTime eventDate);
}

public class SyncAgentConfirmHandlerImp(Repository repository) : SyncAgentConfirmHandler
{
    public async Task Handle(DateTime eventDate)
    {
        var agentMatches = await repository.GetOwpMainMatchesByDate(eventDate);
        var agentConfirms = agentMatches.SelectMany(CalculateProvider).Distinct().ToList();
        await repository.SaveAgentConfirm(agentConfirms);
    }

    private static IEnumerable<MatchAgentConfirm> CalculateProvider(AgentMatchInfo match)
    {
        if (match.IsSaba)
        {
            if (IsProviderCancelled(match.Provider))
            {
                yield return CreateAgentConfirm(Agent.Cancel, match);
                yield break;
            }

            yield return CreateAgentConfirm(Agent.Streaming, match);
            yield break;
        }

        if (match.MatchStatus is MatchStatus.Cancel)
        {
            yield return CreateAgentConfirm(Agent.Cancel, match);
            yield break;
        }

        if (match.MatchStatus is MatchStatus.Postponed or MatchStatus.Suspended)
        {
            if (!string.IsNullOrWhiteSpace(match.InhouseProvider))
            {
                var matchingInhouseProviders = Agent.InhouseProviderList.Where(provider => match.InhouseProvider.IsContainsIgnoreCase(provider)).ToList();

                if (matchingInhouseProviders.Any())
                {
                    foreach (var provider in matchingInhouseProviders)
                    {
                        yield return CreateAgentConfirm(provider, match);
                    }

                    yield break;
                }
            }

            yield return CreateAgentConfirm(Agent.Cancel, match);
            yield break;
        }

        if (!string.IsNullOrEmpty(match.Provider))
        {
            var isBetradar = IsBetradar(match);
            var isBetradar2 = IsBetradar2(match);
            var matchingProviders = Agent.ProviderList.Where(provider => match.Provider.IsContainsIgnoreCase(provider)).ToList();

            if (matchingProviders.Any() || isBetradar || isBetradar2)
            {
                if (isBetradar2)
                {
                    yield return CreateAgentConfirm(Agent.Betradar2, match);
                }

                if (isBetradar)
                {
                    yield return CreateAgentConfirm(Agent.Betradar, match);
                }

                foreach (var provider in matchingProviders)
                {
                    yield return CreateAgentConfirm(provider, match);
                }

                yield break;
            }

            if (IsStreamingProvider(match.Provider))
            {
                yield return CreateAgentConfirm(Agent.Streaming, match);
            }
        }

        if (string.IsNullOrEmpty(match.Provider))
        {
            yield return CreateAgentConfirm(null, match);
        }
    }

    private static bool IsProviderCancelled(string provider)
    {
        return provider?.Contains("cancel", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsBetradar(AgentMatchInfo match)
    {
        return !string.IsNullOrEmpty(match.Provider) &&
               match.Provider.IsContainsIgnoreCase("BET") &&
               !match.Provider.IsContainsIgnoreCase("BET365");
    }

    private static bool IsBetradar2(AgentMatchInfo match)
    {
        return !string.IsNullOrEmpty(match.Provider) &&
               match.Provider.IsContainsIgnoreCase("BET2") &&
               !match.Provider.IsContainsIgnoreCase("BET365");
    }

    private static bool IsStreamingProvider(string provider)
    {
        return !string.IsNullOrEmpty(provider) &&
               (provider.IsContainsIgnoreCase("BET365") || provider.IsContainsIgnoreCase("TV"));
    }

    private static MatchAgentConfirm CreateAgentConfirm(string agent, AgentMatchInfo match)
    {
        return new MatchAgentConfirm
        {
            OWPMatchId = match.OWPMatchId,
            MatchId = match.MatchId,
            Agent = agent
        };
    }
}

public class MatchAgentConfirm
{
    public int OWPMatchId { get; set; }

    public int MatchId { get; set; }

    public string Agent { get; set; }
}

