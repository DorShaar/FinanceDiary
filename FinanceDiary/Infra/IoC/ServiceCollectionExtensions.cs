﻿using FinanceDiary.App;
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

            // Binds between IConfiguration to DatabaseConfigurtaion.
            serviceCollection.Configure<DatabaseConfiguration>(configuration);
            serviceCollection.AddOptions();

            return serviceCollection;
        }
    }
}