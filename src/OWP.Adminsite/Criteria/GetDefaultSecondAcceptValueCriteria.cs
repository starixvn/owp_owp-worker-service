namespace OWP.Adminsite.Criteria;

using Fanex.Data.Repository;

public class GetDefaultSecondAcceptValueCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_SyncMatchInfoToAdminSite_GetDefaultSecondAcceptValue";

    public override bool IsValid() => true;
}