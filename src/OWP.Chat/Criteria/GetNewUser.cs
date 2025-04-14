using Fanex.Data.Repository;

namespace OWP.Chat.Criteria
{
    public class GetNewUserCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "OWP_GetNewUser";


        public override bool IsValid() => true;
    }
}
