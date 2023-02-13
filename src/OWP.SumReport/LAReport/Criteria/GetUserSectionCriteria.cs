namespace OWP.SumReport.LAReport.Criteria;

using Fanex.Data.Repository;

public class GetUserSectionCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_UserSectionsSel";

    public override bool IsValid() => true;
}