using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class DeletedChildMatch : NonQueryCommand
{
    public string ChildMatchesJson { get; set; }

    public override string GetSettingKey() => "OWP_API_DeletedChildMatch";

    public override bool IsValid() => true;
}