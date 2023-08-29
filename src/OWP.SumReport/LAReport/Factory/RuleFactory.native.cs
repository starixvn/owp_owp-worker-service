namespace OWP.SumReport.LAReport.Factory;

using Enum;
using Misc;

public interface MainMatchRuleFactory
{
    MainMatchRule Create(RuleType type);
}

public class MainMatchRuleFactoryImp : MainMatchRuleFactory
{
    public MainMatchRule Create(RuleType type) => type switch
    {
        RuleType.Fantasy => new IsFantasyRule(),
        RuleType.ESoccer => new ESoccerRule(),
        RuleType.FiveMajorLeague => new FiveMajorLeagueRule(),
        RuleType.FtCornerFiveMajorLeague => new FtCorner5MlRule(),
        _ => new FtCornerRule()
    };
}

public interface SpecialProductFactory
{
    SpecialProductRule Create(RuleType type);
}

public class SpecialProductFactoryImp : SpecialProductFactory
{
    public SpecialProductRule Create(RuleType type)
    {
        if (type == RuleType.NotCorner)
        {
            return new NotCornerRule();
        }

        return new IsCornerRule();
    }
}

public interface SingleMatchesFactory
{
    IsSingleMatchesRule Create(RuleType type);
}

public class SingleMatchesFactoryImp : SingleMatchesFactory
{
    public IsSingleMatchesRule Create(RuleType type)
    {
        return new IsSingleMatchesRule();
    }
}