using Fanex.Data.Repository;
using OWP.Criteria;

namespace OWP.Services
{
    public interface LogMonitoringService
    {
        public Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload);

    }

    public class LogMonitoringServiceImp: LogMonitoringService
    {
        private readonly IDynamicRepository dynamicRepository;

        public LogMonitoringServiceImp(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
        {
            await dynamicRepository.ExecuteAsync(new LogMonitoringCriteria()
            {
                CreatedDate = CreatedDate,
                Action = Action,
                Payload = Payload
            });
        }
    }
}
