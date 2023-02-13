namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class UpdateInfositeIdCommand : NonQueryCommand
{
    public string MatchRelationEvents { get; set; }

    public override string GetSettingKey() => "OWP_API_UpdateInfositeIdForMergedMatch";

    public override bool IsValid() => true;
}