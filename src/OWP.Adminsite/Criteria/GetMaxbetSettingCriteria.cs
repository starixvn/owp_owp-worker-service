using Fanex.Data.Repository;

namespace OWP.Adminsite.Criteria;

public class GetMaxbetSettingCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_MaxbetSetting_Get";

    public override bool IsValid() => true;
}