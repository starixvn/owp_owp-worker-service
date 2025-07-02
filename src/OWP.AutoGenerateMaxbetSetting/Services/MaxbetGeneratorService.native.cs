using OWP.AutoGenerateMaxbetSetting.Models;

namespace OWP.AutoGenerateMaxbetSetting.Services
{
    public interface MaxbetGeneratorService
    {
        Task Generate(IList<EventInfo> eventInfo);
    }

    public class MaxbetGeneratorServiceImp: MaxbetGeneratorService
    {
        public Task Generate(IList<EventInfo> eventInfo)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
