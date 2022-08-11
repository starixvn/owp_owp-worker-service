using Microsoft.Extensions.DependencyInjection;
using OWP.SumReportData.PrematchReport;
using OWP.SumReportData.PrematchReport.Services;
using OWP.SumReportData.PrematchReport.Services.League;
using OWP.SumReportData.PrematchReport.Services.Match;
using OWP.SumReportData.Services;

namespace OWP.SumReportData;

public static class DependenceInjection
{
    public static IServiceCollection AddSumReportDataServices(this IServiceCollection services)
    {
        services.AddTransient<TraderReport.TraderRepository, TraderReport.TraderRepositoryImp>();
        services.AddTransient<TraderReport.Handler, TraderReport.HandlerImp>();
        services.AddTransient<TraderReport.Services.RobotService, TraderReport.Services.RobotServiceImp>();
        services.AddTransient<TraderReport.Services.RobotX3Service, TraderReport.Services.RobotX3ServiceImp>();
        services.AddTransient<TraderReport.Services.RobotX5Service, TraderReport.Services.RobotX5ServiceImp>();
        services.AddTransient<TraderReport.Services.ESoccerService, TraderReport.Services.ESoccerServiceImp>();

        services.AddTransient<LAReport.Handler, LAReport.HandlerImp>();
        services.AddTransient<LAReport.LARepository, LAReport.LARepositoryImp>();
        services.AddTransient<LAReport.Factory.MainMatchRuleFactory, LAReport.Factory.MainMatchRuleFactoryImp>();
        services
            .AddTransient<LAReport.Factory.MultipleAndSingleMatchesFactory,
                LAReport.Factory.MultipleAndSingleMatchesFactoryImp>();
        services.AddTransient<LAReport.Factory.SpecialProductFactory, LAReport.Factory.SpecialProductFactoryImp>();
        services.AddTransient<LAReport.Services.CalculationService, LAReport.Services.CalculationServiceImp>();

        services.AddTransient<OvertimeReport.OvertimeRepository, OvertimeReport.OvertimeRepositoryImp>();
        services
            .AddTransient<OvertimeReport.Services.ReportCalculationService,
                OvertimeReport.Services.ReportCalculationServiceImp>();

        services.AddTransient<CacheManipulator, CacheManipulatorImp>();

        services.AddTransient<MatchLiveReport.Repository.ReportRepository, MatchLiveReport.Repository.ReportRepositoryImp>();
        services.AddTransient<MatchLiveReport.Services.ReportService, MatchLiveReport.Services.ReportServiceImp>();

        // Prematch Report
        services.AddTransient<Repository, RepositoryImp>();
        services.AddTransient<DataReportService, DataReportBuilderServiceImp>();
        services.AddTransient<DataMatchReportService, DataMatchReportServiceImp>();
        services.AddTransient<LeagueService, LeagueServiceImp>();
        services.AddTransient<OULeagueService, OULeagueServiceImp>();
        services.AddTransient<HDPLeagueService, HDPLeagueServiceImp>();
        services.AddTransient<MatchService, MatchServiceImp>();
        services.AddTransient<OUMatchService, OUMatchServiceImp>();
        services.AddTransient<HDPMatchService, HDPMatchServiceImp>();
        services.AddTransient<DataReportService, DataReportBuilderServiceImp>();
        services.AddTransient<DbWriterService, DbWriterServiceImp>();
        services.AddTransient<PrematchReportService, PrematchReportServiceImp>();

        return services;
    }
}