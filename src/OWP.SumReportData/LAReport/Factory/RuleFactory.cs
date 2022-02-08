using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Misc;

namespace OWP.SumReportData.LAReport.Factory;

public interface MainMatchRuleFactory
{
    MainMatchRule Create(RuleType type);
}

public class MainMatchRuleFactoryImp : MainMatchRuleFactory
{
    public MainMatchRule Create(RuleType type) => type switch
    {
        RuleType.FANTASY => new IsFantasyRule(),
        RuleType.ESOCCER => new ESoccerRule(),
        _ => new FTCornerRule()
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
        if (type == RuleType.NOT_CORNER)
        {
            return new NotCornerRule();
        }

        return new IsCornerRule();
    }
}

public interface MultipleAndSingleMatchesFactory
{
    MultipleAndSingleMatchesRule Create(RuleType type);
}

public class MultipleAndSingleMatchesFactoryImp : MultipleAndSingleMatchesFactory
{
    public MultipleAndSingleMatchesRule Create(RuleType type)
    {
        if (type == RuleType.MULTIPLE)
        {
            return new MultipleMatchesRule();
        }

        return new SingleMatchesRule();
    }
}