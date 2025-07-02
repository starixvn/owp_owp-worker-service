using OWP.AutoGenerateMaxbetSetting.Models;

namespace OWP.AutoGenerateMaxbetSetting.Services
{
    public interface SpreadGeneratorService
    {
        Task Generate(IList<EventInfo> eventInfo);
    }

    public class SpreadGeneratorServiceImp : SpreadGeneratorService
    {
        public Task Generate(IList<EventInfo> eventInfo)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
