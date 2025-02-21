
using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class SaveLogsChangeCriteria : NonQueryCommand
{
    public string Xml { get; set; }

    public override string GetSettingKey() => "OWP_EventPicking_SaveLogs";

    public override bool IsValid() => true;
}


