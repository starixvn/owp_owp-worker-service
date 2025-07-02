using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace OWP.AutoGenerateMaxbetSetting.Services
{
    public interface GeneratorService
    {
        Task GenerateMaxbetSetting();
    }

    public class GeneratorServiceImp(Repository repository, MaxbetGeneratorServiceImp maxbetGeneratorService, SpreadGeneratorService spreadGeneratorService): GeneratorService
    {
        public async Task GenerateMaxbetSetting()
        {
           

        }

        private async Task GenerateMaxbet()
        {
            var dateFrom = DateTime.Now.AddDays(-7).Date.AddHours(11).AddMinutes(59).AddSeconds(59);
            var eventsWeekday = await repository.GetEventsInTimeRange(DateTime.Now.AddDays(-7), DateTime.UtcNow.AddDays(7));
            await maxbetGeneratorService.Generate(eventsWeekday);
        } 
        
        private async Task GenerateSpread()
        {
            var eventsWeekend = await repository.GetEventsInTimeRange(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7));
            await spreadGeneratorService.Generate(eventsWeekend);
        }
       
    }
}
