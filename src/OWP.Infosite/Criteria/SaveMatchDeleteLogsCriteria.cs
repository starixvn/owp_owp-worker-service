
using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class SaveMatchDeleteLogsCriteria : NonQueryCommand
{
    public string Xml { get; set; }

    public override string GetSettingKey() => "OWP_EventPicking_SaveMatchDeleteLogs";

    public override bool IsValid() => true;
}
