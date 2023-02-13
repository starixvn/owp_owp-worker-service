namespace OWP.InfositeIdSync.Criteria;

using Fanex.Data.Repository;

public class UpdateInfositeIdCommand : NonQueryCommand
{
    public string Xml { get; set; }

    public override string GetSettingKey() => "OWP_API_UpdateInfositeId";

    public override bool IsValid() => true;
}