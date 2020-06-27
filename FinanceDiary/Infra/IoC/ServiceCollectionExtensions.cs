using FinanceDiary.App;
using FinanceDiary.Client;
using FinanceDiary.Domain.Database;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FinanceDiary.Infra.Options;
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

            // Binds between IConfiguration to DatabaseConfigurtaion.
            serviceCollection.Configure<DatabaseConfiguration>(configuration);
            serviceCollection.AddOptions();

            serviceCollection.AddHostedService<ConsoleUI>();

            return serviceCollection;
        }
    }
}