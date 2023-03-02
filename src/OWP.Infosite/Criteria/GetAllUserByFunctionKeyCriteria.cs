namespace OWP.Infosite.Criteria;

using Fanex.Data.Repository;

public class GetAllUserByFunctionKeyCriteria : NonQueryCommand
{
    public string FunctionKey { get; set; }

    public override string GetSettingKey() => "OWP_GetAllUserByFunctionKey";

    public override bool IsValid() => true;
}