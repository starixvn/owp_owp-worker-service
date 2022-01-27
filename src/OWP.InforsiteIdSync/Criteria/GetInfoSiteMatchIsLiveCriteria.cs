using Fanex.Data.Repository;

namespace OWP.InfositeIdSync.Criteria;

public class GetInfoSiteMatchIsLiveCriteria : CriteriaBase
{
    public DateTime FromTime { get; set; }

    public DateTime ToTime { get; set; }

    public override string GetSettingKey() => "OWP_SyncIsLiveSel";

    public override bool IsValid() => true;
}