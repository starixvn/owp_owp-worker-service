using Fanex.Data.Repository;

namespace OWP.ArchiveData.Criteria
{
    public class GetNewUser : CriteriaBase
    {
        public override string GetSettingKey() => "OWP_GetNewUser";


        public override bool IsValid() => true;
    }
}
