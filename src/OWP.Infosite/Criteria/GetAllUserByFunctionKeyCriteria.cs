using Fanex.Data.Repository;

namespace OWP.Infosite.Criteria;

public class GetAllUserByFunctionKeyCriteria : NonQueryCommand
{
    public string FunctionKey { get; set; }

    public override string GetSettingKey() => "OWP_GetAllUserByFunctionKey";

    public override bool IsValid() => true;
}