using Fanex.Data.Repository;

namespace OWP.SumReportData.LAReport.Criteria;

public class GetUserSectionCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_UserSectionsSel";

    public override bool IsValid() => true;
}