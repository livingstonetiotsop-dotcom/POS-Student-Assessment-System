using Microsoft.Extensions.DependencyInjection;
using POS.Application.Ports;
using POS.Application.Services;
using POS.Domain.Factories;
using POS.Domain.Services;
using POS.Domain.Strategies;
using POS.Infrastructure.Export;
using POS.Infrastructure.Repositories;
using POS.Infrastructure.Storage;

namespace POS.Console.Configuration;

public static class DependencyInjection
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Infrastructure
        services.AddSingleton<JsonDataStore>();
        services.AddSingleton<IStudentRepository, JsonStudentRepository>();
        services.AddSingleton<IRubricRepository, JsonRubricRepository>();
        services.AddSingleton<IEvaluationRepository, JsonEvaluationRepository>();
        services.AddSingleton<IResultExporter, CsvResultExporter>();

        // Domain
        services.AddSingleton<EvaluationFactory>();
        services.AddSingleton<IResultCalculationStrategy, WeightedAverageStrategy>();
        services.AddSingleton<FinalResultCalculator>();

        // Application
        services.AddSingleton<POSFacade>();

        return services.BuildServiceProvider();
    }
}
