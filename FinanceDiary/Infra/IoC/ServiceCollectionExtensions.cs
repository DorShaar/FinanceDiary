using FinanceDiary.App;
using FinanceDiary.Domain.Database;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FinanceDiary.Domain.Options;
using FinanceDiary.Infra.ClientApi;
using FinanceDiary.Infra.CommandParsers;
using FinanceDiary.Infra.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceDiary.Infra.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFinanceDiaryDependencies(
            this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IFinanceDiaryManager, FinanceDiaryManager>();
            serviceCollection.AddSingleton<IOperationsFactory, OperationsFactory>();
            serviceCollection.AddSingleton<IIdGenerator, IdGenerator>();
            serviceCollection.AddSingleton<IFinanceDiaryDatabase, FinanceDiaryDatabase>();
            serviceCollection.AddSingleton<ICommandLineRunner, CommandLineParserAdapter>();
            serviceCollection.AddSingleton<ICommandParser, CommandParser>();

            // Binds between IConfiguration to DatabaseConfigurtaion.
            serviceCollection.Configure<DatabaseConfiguration>(configuration);
            serviceCollection.AddOptions();

            serviceCollection.AddHostedService<FinanceDiaryHostedService>();

            return serviceCollection;
        }
    }
}