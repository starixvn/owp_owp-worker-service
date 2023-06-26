namespace OWP.SumReport;

using AutoMapper;
using LAReport;
using LAReport.Factory;
using LAReport.Services;
using MatchLiveReport.Repository;
using MatchLiveReport.Services;
using Microsoft.Extensions.DependencyInjection;
using OvertimeReport;
using OvertimeReport.Services;
using PrematchReport;
using PrematchReport.Services;
using PrematchReport.Services.League;
using PrematchReport.Services.Match;
using Services;
using TraderReport;
using TraderReport.Services;
using Handler = TraderReport.Handler;
using HandlerImp = TraderReport.HandlerImp;

public static class DependenceInjection
{
    public static IServiceCollection AddSumReportServices(this IServiceCollection services)
    {
        services.AddTransient<TraderRepository, TraderRepositoryImp>();
        services.AddTransient<Handler, HandlerImp>();
        services.AddTransient<RobotService, RobotServiceImp>();
        services.AddTransient<RobotX3Service, RobotX3ServiceImp>();
        services.AddTransient<RobotX5Service, RobotX5ServiceImp>();
        services.AddTransient<ESoccerService, ESoccerServiceImp>();

        services.AddTransient<LAReport.Handler, LAReport.HandlerImp>();
        services.AddTransient<LaRepository, LaRepositoryImp>();
        services.AddTransient<MainMatchRuleFactory, MainMatchRuleFactoryImp>();
        services
            .AddTransient<SingleMatchesFactory,
                SingleMatchesFactoryImp>();
        services.AddTransient<SpecialProductFactory, SpecialProductFactoryImp>();
        services.AddTransient<CalculationService, CalculationServiceImp>();
        services.AddTransient<SumUpService, SumUpServiceImp>();
        services.AddTransient<EsoccerProductService, EsoccerProductServiceImp>();
        services.AddTransient<MixedProductService, MixedProductServiceImp>();
        services.AddTransient<SpecialProductService, SpecialProductServiceImp>();
        services.AddTransient<FiveMlProductService, FiveMlProductServiceImp>();
        services.AddTransient<SpecialAndMixedAndESoccerService, SpecialAndMixedAndESoccerServiceImp>();
        services.AddTransient<SingleProductService, SingleProductServiceImp>();
        services.AddTransient<MultipleProductService, MultipleProductServiceImp>();

        services.AddTransient<OvertimeRepository, OvertimeRepositoryImp>();
        services
            .AddTransient<ReportCalculationService,
                ReportCalculationServiceImp>();

        services.AddTransient<CacheManipulator, CacheManipulatorImp>();

        services.AddTransient<ReportRepository, ReportRepositoryImp>();
        services.AddTransient<ReportService, ReportServiceImp>();

        // Prematch Report
        services.AddTransient<Repository, RepositoryImp>();
        services.AddTransient<DataReportService, DataReportBuilderServiceImp>();
        services.AddTransient<DataMatchReportService, DataMatchReportServiceImp>();
        services.AddTransient<LeagueService, LeagueServiceImp>();
        services.AddTransient<OuLeagueService, OuLeagueServiceImp>();
        services.AddTransient<HdpLeagueService, HdpLeagueServiceImp>();
        services.AddTransient<MatchService, MatchServiceImp>();
        services.AddTransient<OuMatchService, OuMatchServiceImp>();
        services.AddTransient<HdpMatchService, HdpMatchServiceImp>();
        services.AddTransient<DataReportService, DataReportBuilderServiceImp>();
        services.AddTransient<DbWriterService, DbWriterServiceImp>();
        services.AddTransient<PrematchReportService, PrematchReportServiceImp>();

        //Auto mapper
        services.AddAutoMapperServices();

        return services;
    }

    private static void AddAutoMapperServices(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperProfile());
        });

        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

    }
}