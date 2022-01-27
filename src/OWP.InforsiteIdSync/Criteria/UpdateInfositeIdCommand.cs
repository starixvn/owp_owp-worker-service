using Fanex.Data.Repository;

namespace OWP.InfositeIdSync.Criteria;

public class UpdateInfositeIdCommand : NonQueryCommand
{
    public string Xml { get; set; }

    public override string GetSettingKey() => "OWP_API_UpdateInfositeId";

    public override bool IsValid() => true;
}