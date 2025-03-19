namespace OWP.Provider.AgentConfirm;

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
            yield return CreateAgentConfirm(Agent.Streaming, match);
        }
        else if (match.MatchStatus is MatchStatus.Postponed or MatchStatus.Cancel or MatchStatus.Suspended)
        {
            yield return CreateAgentConfirm(Agent.Cancel, match);
        }
        else if (Agent.ProviderList.Exists(match.Provider.IsContainsIgnoreCase) || IsBetradar(match))
        {
            if (IsBetradar(match))
            {
                yield return CreateAgentConfirm(Agent.Betradar, match);
            }

            foreach (var provider in Agent.ProviderList.Where(match.Provider.IsContainsIgnoreCase))
            {
                yield return CreateAgentConfirm(provider, match);
            }
        }
        else if (match.Provider.IsContainsIgnoreCase("BET365") || match.Provider.IsContainsIgnoreCase("TV"))
        {
            yield return CreateAgentConfirm(Agent.Streaming, match);
        }
    }

    private static bool IsBetradar(AgentMatchInfo match)
    {
        return match.Provider.IsContainsIgnoreCase("BET") && !match.Provider.IsContainsIgnoreCase("BET365");
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

