using Fanex.Data.Repository;

namespace OWP.Adminsite.Criteria
{
    public class InsertProviderMatchIdCriteria: NonQueryCommand
    {
        public string ProviderMatchIdJson { get; set; }

        public override string GetSettingKey() => "Adminsite_ProviderMatchId_Insert";

        public override bool IsValid() => true;
        
    }
}
