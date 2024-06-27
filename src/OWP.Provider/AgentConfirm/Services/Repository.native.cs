using Fanex.Data.Repository;
using Newtonsoft.Json;
using OWP.Provider.AgentConfirm.Model;

namespace OWP.Provider.AgentConfirm.Services;

public interface Repository
{
    Task<List<AgentMatchInfo>> GetOwpMainMatchesByDate(DateTime eventDate);

    Task SaveAgentConfirm(List<MatchAgentConfirm> agentConfirms);
}

public class RepositoryImp : Repository
{
    private readonly IDynamicRepository dynamicRepository;

    public RepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<List<AgentMatchInfo>> GetOwpMainMatchesByDate(DateTime eventDate)
        => (await dynamicRepository.FetchAsync<AgentMatchInfo>(new GetAgentMatchInfoCriteria { EventDate = eventDate })).ToList();

    public async Task SaveAgentConfirm(List<MatchAgentConfirm> agentConfirms)
    {
        var criteria = new SaveAgentConfirmCriteria
        {
            AgentConfirms = JsonConvert.SerializeObject(agentConfirms)
        };
        await dynamicRepository.ExecuteAsync(criteria);
    }
}

public class GetAgentMatchInfoCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_ProviderVerification_GetMainMatchesByDate";

    public override bool IsValid() => true;
}

public class SaveAgentConfirmCriteria : NonQueryCommand
{
    public string AgentConfirms { get; set; }

    public override string GetSettingKey() => "OWP_ProviderVerification_SyncAgentConfirm";

    public override bool IsValid() => true;
}